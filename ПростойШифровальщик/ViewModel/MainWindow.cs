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
                OnPropertyChanged();
            }
        }


        ICommand _OpenDecrypt;
        public ICommand OpenDecrypt
        {
            get
            {
                return _OpenDecrypt ?? (_OpenDecrypt = new RelayCommand<object>(a =>
                {
                    UserData = new UserData();
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
                        catch (System.IO.IOException )
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
                            }

                            UserData = DataFile.EncryptedClass?.BytesToClass<UserData>(SerializerFormat.Binary);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Пароль не верен");
                        }
                    }
                    else
                    {
                        DataFile = new DataFile();
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

        ICommand _KeyEnter;
        public ICommand KeyEnter
        {
            get
            {
                return _KeyEnter ?? (_KeyEnter = new RelayCommand<object>(a =>
                {
                    byte[] original = UserData?.ClassToBytes(SerializerFormat.Binary);
                    using (var rijAlg = Rijndael.Create())
                    {
                        var salt = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
                        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Key, salt);
                        rijAlg.Key = pdb.GetBytes(32);
                        rijAlg.IV = pdb.GetBytes(16);

                        // Encrypt the string to an array of bytes.
                        DataFile.EncryptedClass = Crypto.EncryptStringToBytes(original, rijAlg);
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
                    var tr = new InformationGroup();
                    UserData.InformationGroup.Add(tr);
                    SomeCollection.Add(new View.SectionDocument() { DataContext = new ViewModel.SectionDocument(tr, SomeCollection, UserData?.InformationGroup) });
                }));
            }
        }
        #endregion

        #region Работа с файлами

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
                        switch (MessageBox.Show("Фаил был изменен. Произвести сохранение новых данных?", "Фаил был изменен.", MessageBoxButton.YesNoCancel, MessageBoxImage.Error, MessageBoxResult.No))
                        {
                            case MessageBoxResult.Yes:

                                break;
                            case MessageBoxResult.No:
                                break;
                            default:
                                e.Cancel = true;
                                break;
                        }
                    }
                }));
            }
        }
    }
}
