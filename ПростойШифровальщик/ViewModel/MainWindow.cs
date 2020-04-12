using Microsoft.Win32;
using MVVM;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ПростойШифровальщик.Model;
using System.IO.Compression;
using System.ComponentModel;

namespace ПростойШифровальщик.ViewModel
{
    class MainWindow : NotificationObject
    {
        public MainWindow()
        {
            root = new ObservableCollection<FilesDataClass>() { new FilesDataClass(FilesData, null) };
        }


        #region Верхняя часть
        /// <summary>
        /// Произошли изменения в файле
        /// </summary>
        public static bool changedFile { get; private set; } = false;

        /// <summary>
        /// При закрытии программы задать вопрос о сохранении файла
        /// </summary>
        public static void ChangedFile() { changedFile = true; }

        string _AddressFile;
        /// <summary>
        /// Расположение файла с которым производится работа
        /// </summary>
        public string AddressFile
        {
            get { return _AddressFile; }
            set
            {
                if (value == _AddressFile) return;
                _AddressFile = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Достаем из диологового окна путь к файлу с которым надо проработать
        /// </summary>
        ICommand _FileOverview;
        public ICommand FileOverview
        {
            get
            {
                return _FileOverview ?? (_FileOverview = new RelayCommand<object>(a =>
                {
                    SaveFileDialog dialog = new SaveFileDialog();
                    if (dialog.ShowDialog() ==true)
                    {
                        AddressFile = dialog.FileName;
                    }
                }));
            }
        }


        string _Key;
        public string Key
        {
            get { return _Key; }
            set
            {
                if (value == _Key) return;
                _Key = value;
                ИзмененПароль = true;
                OnPropertyChanged();
            }
        }

        string _RepeatPassword;
        public string RepeatPassword
        {
            get { return _RepeatPassword; }
            set
            {
                if (value == _RepeatPassword) return;
                _RepeatPassword = value;

                if (RepeatPassword== Key)
                {
                    ownedWindow.Close();
                }

                OnPropertyChanged();
            }
        }

        bool ИзмененПароль = false;

        ICommand _OpenDecrypt;
        public ICommand OpenDecrypt
        {
            get
            {
                return _OpenDecrypt ?? (_OpenDecrypt = new RelayCommand<object>(a =>
                {
                    UserData = new UserData();

                    DataFile = new DataFile();
                    FilesData = new FilesData() { Name = "0", Items = new ObservableCollection<FilesData>() { new FilesData() { Name = "Главная" } } };
                    root = new ObservableCollection<FilesDataClass>() { new FilesDataClass(FilesData,null) };
                    if (System.IO.File.Exists(AddressFile))
                    {
                        try
                        {
                            using (ZipArchive archive = new ZipArchive(File.Open(AddressFile, FileMode.Open), ZipArchiveMode.Update))
                            {
                                ZipArchiveEntry readmeEntry = archive.GetEntry("main");
                                if (readmeEntry != null)
                                {
                                    using (StreamReader writer = new StreamReader(readmeEntry.Open()))
                                    {
                                        DataFile = new DataFile().StreamToClass(writer.BaseStream, SerializerFormat.Binary);
                                    }
                                }
                            }
                        }
                        catch (System.IO.IOException)
                        {
                            MessageBox.Show("Фаил уже открыт какой-то программой");

                        }
                        catch
                        {
                            MessageBox.Show("Фаил не удалось прочитать");
                            DataFile = new DataFile();
                        }


                        try
                        {
                            using (var rijAlg = Rijndael.Create())
                            {
                                var salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
                                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Key, salt);
                                rijAlg.Key = pdb.GetBytes(32);
                                rijAlg.IV = pdb.GetBytes(16);

                                // Decrypt the bytes to a string.
                                DataFile.EncryptedClass = Crypto.DecryptStringFromBytes(DataFile.EncryptedClass, rijAlg);

                                if (DataFile.EncryptedFiles!=null)
                                {
                                    DataFile.EncryptedFiles = Crypto.DecryptStringFromBytes(DataFile.EncryptedFiles, rijAlg);
                                    FilesData = DataFile.EncryptedFiles?.BytesToClass<FilesData>(SerializerFormat.Binary);

                                    root = new ObservableCollection<FilesDataClass>() { new FilesDataClass(FilesData,null) };
                                }
                            }

                            UserData = DataFile.EncryptedClass?.BytesToClass<UserData>(SerializerFormat.Binary);

                            ИзмененПароль = false;
                            changedFile = false;
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Пароль не верен");
                        }
                    }
                    SomeCollection.Clear();

                    if (UserData.InformationGroup == null)
                    {
                        UserData.InformationGroup = new System.Collections.Generic.List<InformationGroup>();
                    }

                    foreach (var item in UserData.InformationGroup)
                    {
                        SomeCollection.Add(new View.SectionDocument() { DataContext = new ViewModel.SectionDocument(item, SomeCollection, UserData?.InformationGroup) });
                    }
                    OnPropertyChanged(nameof(GeneralText));
                }));
            }
        }


        View.RepeatPassword ownedWindow;
        ICommand _KeyEnter;
        public ICommand KeyEnter
        {
            get
            {
                return _KeyEnter ?? (_KeyEnter = new RelayCommand<object>(a =>
                {
                    if (ИзмененПароль)
                    {
                        RepeatPassword = "";
                        ownedWindow = new View.RepeatPassword() { DataContext = this, Owner = App.window };
                        ownedWindow.ShowDialog();

                        if (RepeatPassword != Key)
                        {
                            MessageBox.Show("Фаил не был сохранен. Пороли не совпали");
                            return;
                        }
                    }
                    

                    byte[] original = UserData?.ClassToBytes(SerializerFormat.Binary);
                    byte[] original1 = FilesData?.ClassToBytes(SerializerFormat.Binary);
                    using (var rijAlg = Rijndael.Create())
                    {
                        var salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
                        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Key, salt);
                        rijAlg.Key = pdb.GetBytes(32);
                        rijAlg.IV = pdb.GetBytes(16);

                        // Encrypt the string to an array of bytes.
                        DataFile.EncryptedClass = Crypto.EncryptStringToBytes(original, rijAlg);
                        DataFile.EncryptedFiles = Crypto.EncryptStringToBytes(original1, rijAlg);
                    }

                    using (ZipArchive archive = new ZipArchive(File.Open(AddressFile, FileMode.OpenOrCreate), ZipArchiveMode.Update))
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
                    ИзмененПароль = false;
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
                MainWindow.ChangedFile();
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
                    if (Обозначение==null)
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
                },b=> string.IsNullOrEmpty(Обозначение?.Adress)));
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
                    Обозначение.Name= names;
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
                            string adr = Обозначение1.Adre + @"\"+ DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-fffffff") + fileInf.Name;
                            var fdd = new FilesData() { Name = fileInf.Name, Adress= adr };
                            Обозначение1.filesData.Items.Add(fdd);
                            Обозначение1.Items.Add(new FilesDataClass(fdd, Обозначение1));

                            byte[] original1 = Serializer.FileToBytes(te);
                            using (var rijAlg = Rijndael.Create())
                            {
                                var salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
                                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Key, salt);
                                rijAlg.Key = pdb.GetBytes(32);
                                rijAlg.IV = pdb.GetBytes(16);

                                // Encrypt the string to an array of bytes.
                                original1 = Crypto.EncryptStringToBytes(original1, rijAlg);
                            }

                            //AddressFile = dialog.FileName;
                            using (ZipArchive archive = new ZipArchive(File.Open(AddressFile, FileMode.OpenOrCreate), ZipArchiveMode.Update))
                            {
                                ZipArchiveEntry readmeEntry = archive.CreateEntry(adr);
                                using (StreamWriter writer = new StreamWriter(readmeEntry.Open()))
                                {
                                    writer.BaseStream.Write(original1,0, original1.Length);
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
                    using (ZipArchive archive = new ZipArchive(File.Open(AddressFile, FileMode.OpenOrCreate), ZipArchiveMode.Update))
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

                    


                }, b => Обозначение!=null));
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
                using (ZipArchive archive = new ZipArchive(File.Open(AddressFile, FileMode.Open), ZipArchiveMode.Update))
                {
                    ZipArchiveEntry readmeEntry = archive.GetEntry(s.Adress);
                    if (readmeEntry != null)
                    {

                        plaintext = ReadAllBytes(readmeEntry.Open());

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
                    }
                }

                using (var rijAlg = Rijndael.Create())
                {
                    var salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Key, salt);
                    rijAlg.Key = pdb.GetBytes(32);
                    rijAlg.IV = pdb.GetBytes(16);

                    // Encrypt the string to an array of bytes.
                    plaintext = Crypto.DecryptStringFromBytes(plaintext, rijAlg);
                }

                System.IO.File.WriteAllBytes(st + @"\" + s.Name, plaintext);
            }

            foreach (var item in s.Items)
            {
                setre(item, st + @"\" + s.Name);
            }

        }

        #endregion

        ICommand _Window_Closing1;
        public ICommand Window_Closing1
        {
            get
            {
                return _Window_Closing1 ?? (_Window_Closing1 = new RelayCommand<CancelEventArgs> ( e =>
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
                }));
            }
        }
    }
}
