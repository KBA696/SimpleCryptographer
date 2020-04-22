using Microsoft.Win32;
using MVVM;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ПростойШифровальщик.Model;
using System.IO.Compression;
using System.ComponentModel;

namespace ПростойШифровальщик.ViewModel
{
    class WindowCryptography : NotificationObject
    {
        readonly MainWindow mainWindow;
        readonly string addressFile;
        Crypto key;

        WindowCryptography(MainWindow mainWindow, string addressFile, string key)
        {
            this.mainWindow = mainWindow;
            this.addressFile = addressFile;
            this.key = new Crypto(key);
        }

        public static (WindowCryptography name, string error) GetTuple(MainWindow mainWindow, string addressFile, string key)
        {
            WindowCryptography windowCryptography = new WindowCryptography(mainWindow, addressFile, key);

            windowCryptography.root = new ObservableCollection<FilesDataClass>() { new FilesDataClass(windowCryptography.FilesData, null) };

            windowCryptography.UserData = new UserData();

            windowCryptography.DataFile = new DataFile();
            windowCryptography.FilesData = new FilesData() { Name = "0", Items = new ObservableCollection<FilesData>() { new FilesData() { Name = "Главная" } } };
            windowCryptography.root = new ObservableCollection<FilesDataClass>() { new FilesDataClass(windowCryptography.FilesData, null) };
            if (System.IO.File.Exists(addressFile))
            {
                try
                {
                    using (ZipArchive archive = new ZipArchive(File.Open(addressFile, FileMode.Open), ZipArchiveMode.Update))
                    {
                        ZipArchiveEntry readmeEntry = archive.GetEntry("main");
                        if (readmeEntry != null)
                        {
                            using (StreamReader writer = new StreamReader(readmeEntry.Open()))
                            {
                                windowCryptography.DataFile = new DataFile().StreamToClass(writer.BaseStream, SerializerFormat.Binary);
                            }
                        }
                    }
                }
                catch (System.IO.IOException)
                {
                    return (name: null, error: "Фаил уже открыт какой-то программой");
                }
                catch
                {
                    return (name: null, error: "Фаил не удалось прочитать");
                }

                try
                {
                    windowCryptography.DataFile.EncryptedClass = windowCryptography.key.DecryptStringFromBytes(windowCryptography.DataFile.EncryptedClass);

                    if (windowCryptography.DataFile.EncryptedFiles != null)
                    {
                        windowCryptography.DataFile.EncryptedFiles = windowCryptography.key.DecryptStringFromBytes(windowCryptography.DataFile.EncryptedFiles);
                        windowCryptography.FilesData = windowCryptography.DataFile.EncryptedFiles?.BytesToClass<FilesData>(SerializerFormat.Binary);

                        windowCryptography.root = new ObservableCollection<FilesDataClass>() { new FilesDataClass(windowCryptography.FilesData, null) };
                    }


                    windowCryptography.UserData = windowCryptography.DataFile.EncryptedClass?.BytesToClass<UserData>(SerializerFormat.Binary);

                    changedFile = false;
                }
                catch (Exception)
                {
                    return (name: null, error: "Пароль не верен");
                }
            }
            windowCryptography.SomeCollection.Clear();

            if (windowCryptography.UserData.InformationGroup == null)
            {
                windowCryptography.UserData.InformationGroup = new System.Collections.Generic.List<InformationGroup>();
            }

            foreach (var item in windowCryptography.UserData.InformationGroup)
            {
                windowCryptography.SomeCollection.Add(new View.SectionDocument() { DataContext = new ViewModel.SectionDocument(item, windowCryptography.SomeCollection, windowCryptography.UserData?.InformationGroup) });
            }
            windowCryptography.OnPropertyChanged(nameof(GeneralText));

            if (!System.IO.File.Exists(addressFile))
            {
                if (windowCryptography.KeyEnter.CanExecute(null))
                {
                    windowCryptography.KeyEnter.Execute(null);
                }
            }

            return (name: windowCryptography, error: null);
        }

        /// <summary>
        /// Произошли изменения в файле
        /// </summary>
        static bool changedFile { get; set; } = false;

        /// <summary>
        /// При закрытии программы задать вопрос о сохранении файла
        /// </summary>
        public static void ChangedFile() { changedFile = true; }

        #region Верхняя часть



        ICommand _KeyEnter;
        public ICommand KeyEnter
        {
            get
            {
                return _KeyEnter ?? (_KeyEnter = new RelayCommand<object>(a =>
                {



                    byte[] original = UserData?.ClassToBytes(SerializerFormat.Binary);
                    byte[] original1 = FilesData?.ClassToBytes(SerializerFormat.Binary);

                    // Encrypt the string to an array of bytes.
                    DataFile.EncryptedClass = key.EncryptStringToBytes(original);
                    DataFile.EncryptedFiles = key.EncryptStringToBytes(original1);


                    using (ZipArchive archive = new ZipArchive(File.Open(addressFile, FileMode.OpenOrCreate), ZipArchiveMode.Update))
                    {
                        ZipArchiveEntry readmeEntry = archive.GetEntry("main");
                        /*Вариант дозаписать
                        using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                        {
                            writer.BaseStream.Seek(0, SeekOrigin.End);
                            writer.WriteLine("append line to file");
                        }
                        readmeEntry.LastWriteTime = DateTimeOffset.UtcNow.LocalDateTime;
                        Вариант дозаписать*/
                        readmeEntry?.Delete();

                        readmeEntry = archive.CreateEntry("main");
                        DataFile.ClassToStream(readmeEntry.Open(), SerializerFormat.Binary);
                    }
                    changedFile = false;
                    MessageBox.Show("Фаил успешно сохранен");
                }));
            }
        }
        #endregion

        #region Работа с текстом
        /// <summary>
        /// Данные пользователя
        /// </summary>
        public UserData UserData = new UserData();
        public DataFile DataFile { get; set; } = new DataFile();


        public ObservableCollection<ContentControl> SomeCollection { get; set; } = new ObservableCollection<ContentControl>();

        ICommand _Sort;
        public ICommand Sort
        {
            get
            {
                return _Sort ?? (_Sort = new RelayCommand<object>(a =>
                {
                    SomeCollection = new ObservableCollection<ContentControl>(SomeCollection.OrderBy(i => ((SectionDocument)i.DataContext).Name));
                    OnPropertyChanged(nameof(SomeCollection));
                }));
            }
        }


        public string GeneralText
        {
            get { return UserData.GeneralText; }
            set
            {
                if (value == UserData.GeneralText) return;
                UserData.GeneralText = value;
                ChangedFile();
                OnPropertyChanged();
            }
        }


        ICommand _AddInformations;
        public ICommand AddInformations
        {
            get
            {
                return _AddInformations ?? (_AddInformations = new RelayCommand<object>(a =>
                {
                    ChangedFile();
                    var tr = new InformationGroup();
                    UserData.InformationGroup.Add(tr);
                    SomeCollection.Add(new View.SectionDocument() { DataContext = new ViewModel.SectionDocument(tr, SomeCollection, UserData?.InformationGroup) });
                }));
            }
        }
        #endregion

        #region Работа с файлами

        FilesData FilesData = new FilesData() { Name = "0", Items = new ObservableCollection<FilesData>() { new FilesData() { Name = "Главная" } } };

        ObservableCollection<FilesDataClass> _root;
        public ObservableCollection<FilesDataClass> root
        {
            get { return _root; }
            set
            {
                if (value == _root) return;
                _root = value;
                OnPropertyChanged();
            }
        }
        string _names;
        public string names
        {
            get { return _names; }
            set
            {
                if (value == _names) return;
                _names = value;
                OnPropertyChanged();
            }
        }
        FilesDataClass _Обозначение;
        public FilesDataClass Обозначение
        {
            get { return _Обозначение; }
            set
            {
                if (value == _Обозначение) return;
                _Обозначение = value;
                names = _Обозначение?.Name;
                OnPropertyChanged();
            }
        }

        ICommand _AddFolder;
        public ICommand AddFolder
        {
            get
            {
                return _AddFolder ?? (_AddFolder = new RelayCommand<object>(a =>
                {
                    if (Обозначение == null)
                    {

                        var fdd = new FilesData() { Name = names };
                        root[0].filesData.Items.Add(fdd);
                        root[0].Items.Add(new FilesDataClass(fdd, root[0]));
                    }
                    else
                    {
                        var fdd = new FilesData() { Name = names };
                        Обозначение.filesData.Items.Add(fdd);
                        Обозначение.Items.Add(new FilesDataClass(fdd, Обозначение));
                    }
                    ChangedFile();
                }, b => string.IsNullOrEmpty(Обозначение?.Adress)));
            }
        }

        ICommand _KeyEsc;
        public ICommand KeyEsc
        {
            get
            {
                return _KeyEsc ?? (_KeyEsc = new RelayCommand<object>(a =>
                {
                    Обозначение.IsSelected = false;
                    Обозначение = null;
                }));
            }
        }



        ICommand _Rename;
        public ICommand Rename
        {
            get
            {
                return _Rename ?? (_Rename = new RelayCommand<object>(a =>
                {
                    Обозначение.Name = names;
                    ChangedFile();
                }));
            }
        }


        ICommand _AddOp;
        public ICommand AddOp
        {
            get
            {
                return _AddOp ?? (_AddOp = new RelayCommand<object>(a =>
                {
                    var Обозначение1 = this.Обозначение;
                    OpenFileDialog dialog = new OpenFileDialog();
                    dialog.Multiselect = true;
                    if (dialog.ShowDialog() == true)
                    {
                        foreach (var te in dialog.FileNames)
                        {
                            FileInfo fileInf = new FileInfo(te);
                            string adr = Обозначение1.Adre + @"\" + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-fffffff") + fileInf.Name;
                            var fdd = new FilesData() { Name = fileInf.Name, Adress = adr };
                            Обозначение1.filesData.Items.Add(fdd);
                            Обозначение1.Items.Add(new FilesDataClass(fdd, Обозначение1));

                            byte[] original1 = Serializer.FileToBytes(te);

                            // Encrypt the string to an array of bytes.
                            original1 = key.EncryptStringToBytes(original1);


                            //AddressFile = dialog.FileName;
                            using (ZipArchive archive = new ZipArchive(File.Open(addressFile, FileMode.OpenOrCreate), ZipArchiveMode.Update))
                            {
                                ZipArchiveEntry readmeEntry = archive.CreateEntry(adr);
                                using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                                {
                                    writer.BaseStream.Write(original1, 0, original1.Length);
                                }
                            }
                        }
                    }

                    if (KeyEnter.CanExecute(null))
                    {
                        KeyEnter.Execute(null);
                    }
                }, b => string.IsNullOrEmpty(Обозначение?.Adress)));
            }
        }

        ICommand _Del;
        public ICommand Del
        {
            get
            {
                return _Del ?? (_Del = new RelayCommand<object>(a =>
                {
                    var Обозначение1 = this.Обозначение;
                    this.Обозначение = null;
                    Dele(Обозначение1);

                    Обозначение1.Del();

                    if (KeyEnter.CanExecute(null))
                    {
                        KeyEnter.Execute(null);
                    }
                }));
            }
        }

        void Dele(FilesDataClass Обозначение1)
        {
            if (!string.IsNullOrEmpty(Обозначение1.Adress))
            {
                using (ZipArchive archive = new ZipArchive(File.Open(addressFile, FileMode.OpenOrCreate), ZipArchiveMode.Update))
                {
                    ZipArchiveEntry readmeEntry = archive.GetEntry(Обозначение1.Adress);

                    readmeEntry?.Delete();
                }
            }
            foreach (var item in Обозначение1.Items)
            {
                Dele(item);
            }
        }

        ICommand _Extract;
        public ICommand Extract
        {
            get
            {
                return _Extract ?? (_Extract = new RelayCommand<object>(a =>
                {
                    OpenFileDialog dialog = new OpenFileDialog();
                    if (dialog.ShowDialog() == true)
                    {
                        var Обозначение1 = this.Обозначение;

                        FileInfo fileInf = new FileInfo(dialog.FileName);

                        setre(Обозначение1, fileInf.DirectoryName);
                    }




                }, b => Обозначение != null));
            }
        }

        void setre(FilesDataClass s, string st)
        {
            if (string.IsNullOrEmpty(s.Adress))
            {
                Directory.CreateDirectory(st + @"\" + s.Name);
            }
            else
            {
                byte[] plaintext = null;
                using (ZipArchive archive = new ZipArchive(File.Open(addressFile, FileMode.Open), ZipArchiveMode.Update))
                {
                    ZipArchiveEntry readmeEntry = archive.GetEntry(s.Adress);
                    if (readmeEntry != null)
                    {

                        plaintext = ReadAllBytes(readmeEntry.Open());

                    }
                }

                // Encrypt the string to an array of bytes.
                plaintext = key.DecryptStringFromBytes(plaintext);


                System.IO.File.WriteAllBytes(st + @"\" + s.Name, plaintext);
            }

            foreach (var item in s.Items)
            {
                setre(item, st + @"\" + s.Name);
            }

        }

        byte[] ReadAllBytes(Stream instream)
        {
            if (instream is MemoryStream)
                return ((MemoryStream)instream).ToArray();

            using (var memoryStream = new MemoryStream())
            {
                instream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        #endregion

        #region Генератор паролей
        string _GeneratedPassword;
        public string GeneratedPassword
        {
            get { return _GeneratedPassword; }
            set
            {
                if (value == _GeneratedPassword) return;
                _GeneratedPassword = value;
                OnPropertyChanged();
            }
        }

        bool _Symbols = true;
        public bool Symbols
        {
            get { return _Symbols; }
            set
            {
                if (value == _Symbols) return;
                _Symbols = value;
                OnPropertyChanged();
            }
        }

        bool _Numbers = true;
        public bool Numbers
        {
            get { return _Numbers; }
            set
            {
                if (value == _Numbers) return;
                _Numbers = value;
                OnPropertyChanged();
            }
        }

        bool _А_Я = true;
        public bool А_Я
        {
            get { return _А_Я; }
            set
            {
                if (value == _А_Я) return;
                _А_Я = value;
                OnPropertyChanged();
            }
        }

        bool _а_я = true;
        public bool а_я
        {
            get { return _а_я; }
            set
            {
                if (value == _а_я) return;
                _а_я = value;
                OnPropertyChanged();
            }
        }

        bool _A_Z = true;
        public bool A_Z
        {
            get { return _A_Z; }
            set
            {
                if (value == _A_Z) return;
                _A_Z = value;
                OnPropertyChanged();
            }
        }

        bool _a_z = true;
        public bool a_z
        {
            get { return _a_z; }
            set
            {
                if (value == _a_z) return;
                _a_z = value;
                OnPropertyChanged();
            }
        }

        int _NumberCharacters = 16;
        public int NumberCharacters
        {
            get { return _NumberCharacters; }
            set
            {
                if (value == _NumberCharacters) return;
                _NumberCharacters = value;
                OnPropertyChanged();
            }
        }

        string _ItsSymbol = "";
        public string ItsSymbol
        {
            get { return _ItsSymbol; }
            set
            {
                if (value == _ItsSymbol) return;
                _ItsSymbol = value;
                OnPropertyChanged();
            }
        }

        ICommand _StartGeneratedPassword;
        public ICommand StartGeneratedPassword
        {
            get
            {
                return _StartGeneratedPassword ?? (_StartGeneratedPassword = new RelayCommand<object>(a =>
                {
                    GeneratedPassword = "";
                    string abc = ItsSymbol;
                    if (a_z)
                        abc += "abcdefghijklmnopqrstuvwxyz";
                    if (A_Z)//использовать заглавные
                        abc += "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    if (а_я)
                        abc += "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
                    if (А_Я)//использовать заглавные
                        abc += "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
                    if (Symbols)//использовать спецсимволы
                        abc += " !@#$%^&*()";
                    if (Numbers)//юзать цифры
                        abc += "0123456789";
                    Random rnd = new Random();
                    for (int i = 0; i < NumberCharacters; i++)
                    {
                        GeneratedPassword += abc[rnd.Next(abc.Length)];
                    }
                }, b => (Symbols || Numbers || А_Я || а_я || A_Z || a_z || ItsSymbol.Length > 0) && NumberCharacters > 0));
            }
        }
        #endregion

        /// <summary>
        /// Действия перед закрытием окна
        /// </summary>
        /// <param name="e"></param>
        public void Closing(CancelEventArgs e)
        {
            if (changedFile)
            {
                switch (MessageBox.Show("Были внесены или изменены какието данные но они небыли сохронены. Они будут утерены при закрытии. Сохранить фаил?", "Фаил был изменен.", MessageBoxButton.YesNoCancel, MessageBoxImage.Error, MessageBoxResult.No))
                {
                    case MessageBoxResult.Yes:
                        if (KeyEnter.CanExecute(null))
                        {
                            KeyEnter.Execute(null);
                        }
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        ICommand _LaunchWindow;
        public ICommand LaunchWindow
        {
            get
            {
                return _LaunchWindow ?? (_LaunchWindow = new RelayCommand<object>(e =>
                {
                    mainWindow.Content = new View.WindowFileSelection() { DataContext = new WindowFileSelection(mainWindow) };
                }));
            }
        }

        string _Password = "";
        public string Password
        {
            get { return _Password; }
            set
            {
                if (value == _Password) return;
                _Password = value;
                OnPropertyChanged();
            }
        }

        string _RepeatPassword = "";
        public string RepeatPassword
        {
            get { return _RepeatPassword; }
            set
            {
                if (value == _RepeatPassword) return;
                _RepeatPassword = value;
                OnPropertyChanged();
            }
        }

        ICommand _PasswordChange;
        public ICommand PasswordChange
        {
            get
            {
                return _PasswordChange ?? (_PasswordChange = new RelayCommand<object>(e =>
                {
                    new View.RepeatPassword() { DataContext = this, Owner = App.window }.ShowDialog();
                }));
            }
        }

        ICommand _ChangePasswordPermanently;
        public ICommand ChangePasswordPermanently
        {
            get
            {
                return _ChangePasswordPermanently ?? (_ChangePasswordPermanently = new RelayCommand<object>(a =>
                {

                    if (Password != RepeatPassword)
                    {
                        MessageBox.Show("Пороли не совпали");
                        return;
                    }

                    var newKey = new Crypto(Password);

                    using (ZipArchive archive = new ZipArchive(File.Open(addressFile, FileMode.OpenOrCreate), ZipArchiveMode.Update))
                    {
                        //HashSet<string> directories = new HashSet<string>();
                        var fd = archive.Entries.ToArray();
                        foreach (var zipArchiveEntry in fd)
                        {
                            if (zipArchiveEntry.FullName != "main")
                            {
                                string FullName = zipArchiveEntry.FullName;
                                //string zipDir = zipArchiveEntry.FullName.Substring(0, zipArchiveEntry.FullName.Length - zipArchiveEntry.Name.Length);
                                //if (zipDir != "") directories.Add(zipDir);
                                var gf = zipArchiveEntry.Open();
                                var plaintext = ReadAllBytes(gf);
                                gf.Close();
                                plaintext = key.DecryptStringFromBytes(plaintext);

                                plaintext = newKey.EncryptStringToBytes(plaintext);

                                //ZipArchiveEntry readmeEntry1 = archive.GetEntry(zipArchiveEntry.FullName);
                                zipArchiveEntry.Delete();

                                var zipArchiveEntry1 = archive.CreateEntry(FullName);
                                using (StreamWriter writer = new StreamWriter(zipArchiveEntry1.Open()))
                                {
                                    writer.BaseStream.Write(plaintext, 0, plaintext.Length);
                                }
                            }

                        }

                        //Сделать методом
                        byte[] original = UserData?.ClassToBytes(SerializerFormat.Binary);
                        byte[] original1 = FilesData?.ClassToBytes(SerializerFormat.Binary);

                        // Encrypt the string to an array of bytes.
                        DataFile.EncryptedClass = newKey.EncryptStringToBytes(original);
                        DataFile.EncryptedFiles = newKey.EncryptStringToBytes(original1);
                        ZipArchiveEntry readmeEntry = archive.GetEntry("main");
                        readmeEntry?.Delete();

                        readmeEntry = archive.CreateEntry("main");
                        DataFile.ClassToStream(readmeEntry.Open(), SerializerFormat.Binary);

                    }

                    /*void RenameEntry(this ZipArchive archive, string oldName, string newName)
                    {
                        ZipArchiveEntry oldEntry = archive.GetEntry(oldName),
                            newEntry = archive.CreateEntry(newName);

                        using (Stream oldStream = oldEntry.Open())
                        using (Stream newStream = newEntry.Open())
                        {
                            oldStream.CopyTo(newStream);
                        }

                        oldEntry.Delete();
                    }*/
                    key = newKey;
                    RepeatPassword = Password = "";
                    ((Window)a).Close();
                    MessageBox.Show("Пороль изменен");
                }));
            }
        }
    }
}