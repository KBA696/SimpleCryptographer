using MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ПростойШифровальщик.Model;

namespace ПростойШифровальщик.ViewModel
{
    /// <summary>
    /// Контекст данных для окна WindowFileSelection в котором выбераем или создаем фаил шифрования
    /// </summary>
    class WindowFileSelection : NotificationObject
    {
        readonly MainWindow mainWindow;

        static Dictionary<string, FileSystem> FileSystem = new Dictionary<string, FileSystem>();

        public static FileSystem FileSystemS(string ПолныйПуть)
        {
            if (FileSystem.ContainsKey(ПолныйПуть))
            {
                return FileSystem[ПолныйПуть];
            }
            else
            {
                var name = Path.GetFileName(ПолныйПуть);
                return FileSystem[ПолныйПуть] = new FileSystem(string.IsNullOrEmpty(name)? ПолныйПуть : name, ПолныйПуть);
            }
        }

        public WindowFileSelection(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            foreach (DriveInfo drive1 in DriveInfo.GetDrives())
            {

                UserFiles.Add(FileSystemS(drive1.ToString()));

                
            }
        }


        ObservableCollection<FileSystem> _UserFiles = new ObservableCollection<FileSystem>();
        /// <summary>
        /// Перечень файлов на компе пользователя
        /// </summary>
        public ObservableCollection<FileSystem> UserFiles
        {
            get { return _UserFiles; }
            set
            {
                if (value == _UserFiles) return;
                _UserFiles = value;
                OnPropertyChanged();
            }
        }

        FileSystem _SelectedUserFile;
        /// <summary>
        /// Выбранный фаил в TreeView
        /// </summary>
        public FileSystem SelectedUserFile
        {
            get { return _SelectedUserFile; }
            set
            {
                if (value == _SelectedUserFile) return;
                _SelectedUserFile = value;
                if (_SelectedUserFile!=null)
                {
                    FullPath = _SelectedUserFile.ПолныйАдрес;
                    ОткрытьВыбранное();
                }


                OnPropertyChanged();
            }
        }

        string _ДоFullPath;
        string _FullPath;
        /// <summary>
        /// 
        /// </summary>
        public string FullPath
        {
            get { return _FullPath; }
            set
            {
                if (value == _FullPath) return;
                _FullPath = value;
                
                OnPropertyChanged();
            }
        }
        ICommand _TextBoxLostFocus;
        /// <summary>
        /// Двойной клик
        /// </summary>
        public ICommand TextBoxLostFocus
        {
            get
            {
                return _TextBoxLostFocus ?? (_TextBoxLostFocus = new RelayCommand<object>(a =>
                {
                    FullPath = _ДоFullPath;
                }));
            }
        }

        ICommand _KeyEnter;
        /// <summary>
        /// Двойной клик
        /// </summary>
        public ICommand KeyEnter
        {
            get
            {
                return _KeyEnter ?? (_KeyEnter = new RelayCommand<object>(a =>
                {
                    if (Directory.Exists(FullPath))
                    {
                        //в самом конце лишние слэшь надо удолять
                        if (SelectedUserFile != null)
                        {
                            SelectedUserFile.IsSelected = false;
                        }
                        ОткрытьВыбранное();
                        SelectedUserFile.IsSelected = true;
                    }
                    else
                    {
                        MessageBox.Show("Папка не существует");
                        if (TextBoxLostFocus.CanExecute(null))
                        {
                            TextBoxLostFocus.Execute(null);
                        }
                    }
                }));
            }
        }

        void ОткрытьВыбранное()
        {
            SelectedUserFile = WindowFilesFolders = FileSystemS(_FullPath);
            _ДоFullPath = FullPath;
        }

        FileSystem _WindowFilesFolders;
        /// <summary>
        /// Выбранный фаил в TreeView
        /// </summary>
        public FileSystem WindowFilesFolders
        {
            get { return _WindowFilesFolders; }
            set
            {
                if (value == _WindowFilesFolders) return;
                _WindowFilesFolders = value;
                OnPropertyChanged();
            }
        }

        FileSystem _SelectedWindowFilesFolders;
        /// <summary>
        /// Выбранный фаил в TreeView
        /// </summary>
        public FileSystem SelectedWindowFilesFolders
        {
            get { return _SelectedWindowFilesFolders; }
            set
            {
                if (value == _SelectedWindowFilesFolders) return;
                _SelectedWindowFilesFolders = value;
                if (System.IO.File.Exists(_SelectedWindowFilesFolders?.ПолныйАдрес))
                {
                    FileName = _SelectedWindowFilesFolders.Name;
                }
                OnPropertyChanged();
            }
        }
        string _FileName;
        /// <summary>
        /// 
        /// </summary>
        public string FileName
        {
            get { return _FileName; }
            set
            {
                if (value == _FileName) return;
                _FileName = value;
                OnPropertyChanged();
            }
        }

        ICommand _CreateOrOpen;
        /// <summary>
        /// Открываем или создаем новый зашифрованный фаил
        /// </summary>
        public ICommand CreateOrOpen
        {
            get
            {
                return _CreateOrOpen ?? (_CreateOrOpen = new RelayCommand<object>(a =>
                {
                    mainWindow.Content = new View.WindowCryptography() { DataContext = new WindowCryptography(mainWindow) };
                }));
            }
        }


        ICommand _Ms;
        /// <summary>
        /// Двойной клик
        /// </summary>
        public ICommand Ms
        {
            get
            {
                return _Ms ?? (_Ms = new RelayCommand<object>(a =>
                {
                    FullPath = SelectedWindowFilesFolders.ПолныйАдрес;
                    
                    if (SelectedUserFile != null)
                    {
                        SelectedUserFile.IsSelected = false;
                    }
                    ОткрытьВыбранное();
                    SelectedUserFile.IsSelected = true;
                }));
            }
        }



    }
}
