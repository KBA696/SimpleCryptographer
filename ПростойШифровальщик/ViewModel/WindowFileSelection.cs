using MVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ПростойШифровальщик.Model;

namespace ПростойШифровальщик.ViewModel
{
    /// <summary>
    /// Контекст данных для окна WindowFileSelection в котором выбераем или создаем фаил шифрования
    /// </summary>
    class WindowFileSelection : NotificationObject
    {
        readonly MainWindow mainWindow;

        /// <summary>
        /// Держим в памяти все папки, которые открывались
        /// </summary>
        static Dictionary<string, FileSystem> fileSystem = new Dictionary<string, FileSystem>();

        /// <summary>
        /// Извлекаем из памяти или добавляем в память если не было запрашиваемую директорию
        /// </summary>
        /// <param name="fullPath">Полный путь к ДИРЕКТОРИИ</param>
        /// <returns>Директорию с файлами и директориями которые находятся в ней</returns>
        public static FileSystem FileSystemValue(string fullPath)
        {
            if (fileSystem.ContainsKey(fullPath))
            {
                return fileSystem[fullPath];
            }
            else
            {
                var name = Path.GetFileName(fullPath);
                return fileSystem[fullPath] = new FileSystem(string.IsNullOrEmpty(name) ? fullPath : name, fullPath);
            }
        }

        public WindowFileSelection(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            //Рабочий стол по умолчанию делаем открытым в основном ListBox
            var desktop = FileSystemValue(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            UserFiles.Add(desktop);
            SelectedUserFile = desktop;

            //Добовляю остальные диски
            foreach (DriveInfo drive1 in DriveInfo.GetDrives())
            {
                UserFiles.Add(FileSystemValue(drive1.ToString()));
            }
        }

        #region TreeView
        /// <summary>
        /// Перечень файлов на компе пользователя
        /// </summary>
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

        /// <summary>
        /// Выбранный фаил в TreeView
        /// </summary>
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

                if (_SelectedUserFile != null)
                {
                    FullPath = _SelectedUserFile.ПолныйАдрес;
                    ОткрытьВыбранное();
                }

                OnPropertyChanged();
            }
        }
        #endregion

        #region ListBox
        /// <summary>
        /// Основной ListBox с папками и файлами
        /// </summary>
        FileSystem _WindowFilesFolders;
        /// <summary>
        /// Основной ListBox с папками и файлами
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

        /// <summary>
        /// Выбранный фаил или папка из основного ListBox
        /// </summary>
        FileSystem _SelectedWindowFilesFolders;
        /// <summary>
        /// Выбранный фаил или папка из основного ListBox
        /// </summary>
        public FileSystem SelectedWindowFilesFolders
        {
            get { return _SelectedWindowFilesFolders; }
            set
            {
                if (value == _SelectedWindowFilesFolders) return;
                _SelectedWindowFilesFolders = value;
                if (File.Exists(_SelectedWindowFilesFolders?.ПолныйАдрес))
                {
                    FileName = _SelectedWindowFilesFolders.Name;
                }
                OnPropertyChanged();
            }
        }

        ICommand _MouseDoubleClickListBox;
        /// <summary>
        /// Двойной клик в основном ListBox
        /// </summary>
        public ICommand MouseDoubleClickListBox
        {
            get
            {
                return _MouseDoubleClickListBox ?? (_MouseDoubleClickListBox = new RelayCommand<object>(a =>
                {
                    DirectoryInfo dir = new DirectoryInfo(SelectedWindowFilesFolders.ПолныйАдрес);
                    //Если двойной клик был по папке то открываем её
                    if (dir.Attributes == FileAttributes.Directory || dir.Attributes == (FileAttributes.ReadOnly | FileAttributes.Directory))
                    {
                        FullPath = SelectedWindowFilesFolders.ПолныйАдрес;

                        if (SelectedUserFile != null)
                        {
                            SelectedUserFile.IsSelected = false;
                        }
                        ОткрытьВыбранное();
                        SelectedUserFile.IsSelected = true;
                    }


                }));
            }
        }
        #endregion

        #region TextBox - адрес директории
        /// <summary>
        /// Адрес до ручного ввода и нажатия на enter
        /// </summary>
        string _FullPathBeforeManipulation;
        /// <summary>
        /// Путь до файла
        /// </summary>
        string _FullPath;
        /// <summary>
        /// Путь до файла
        /// </summary>
        public string FullPath
        {
            get { return _FullPath; }
            set
            {
                if (value == _FullPath) return;
                _FullPath = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonName));
            }
        }

        ICommand _TextBoxLostFocus;
        /// <summary>
        /// Возвращаем прежний адрес если небыл нажат enter в текстовом поле с адресной строкой
        /// </summary>
        public ICommand TextBoxLostFocus
        {
            get
            {
                return _TextBoxLostFocus ?? (_TextBoxLostFocus = new RelayCommand<object>(a =>
                {
                    FullPath = _FullPathBeforeManipulation;
                }));
            }
        }

        ICommand _KeyEnter;
        /// <summary>
        /// Действие при нажатии на enter в текстовом поле с адресной строкой
        /// </summary>
        public ICommand KeyEnter
        {
            get
            {
                return _KeyEnter ?? (_KeyEnter = new RelayCommand<object>(a =>
                {
                    DirectoryInfo dir = new DirectoryInfo(FullPath);
                    if (dir.Attributes == FileAttributes.Directory)
                    {
                        //Если выбрана директория то открываем её
                        if (SelectedUserFile != null)
                        {
                            SelectedUserFile.IsSelected = false;
                        }
                        FullPath = dir.FullName.Trim('\\');
                        ОткрытьВыбранное();
                        SelectedUserFile.IsSelected = true;
                    }
                    if (dir.Attributes == FileAttributes.Archive)
                    {
                        //если был введен адрес к файлу то открываем директорию в которой лежал фаил и добовляем имя файла в текстбокс
                        if (SelectedUserFile != null)
                        {
                            SelectedUserFile.IsSelected = false;
                        }

                        FullPath = dir.Parent.ToString().Trim('\\');
                        ОткрытьВыбранное();
                        FileName = dir.Name;
                    }
                    else if ((int)dir.Attributes == -1)
                    {
                        //если не существует фаил или папка возвращаем то что было
                        MessageBox.Show("Папка или фаил не существует");
                        if (TextBoxLostFocus.CanExecute(null))
                        {
                            TextBoxLostFocus.Execute(null);
                        }
                    }
                }));
            }
        }

        /// <summary>
        /// открываем в Основной ListBox папку и в TreeView делаем её выделенной 
        /// </summary>
        void ОткрытьВыбранное()
        {
            SelectedUserFile = WindowFilesFolders = FileSystemValue(_FullPath);
            _FullPathBeforeManipulation = FullPath;
        }
        #endregion

        #region Имя файла, пароль и кнопка открыть
        /// <summary>
        /// Имя файла который надо разшифровать
        /// </summary>
        string _FileName;
        /// <summary>
        /// Имя файла который надо разшифровать
        /// </summary>
        public string FileName
        {
            get { return _FileName; }
            set
            {
                if (value == _FileName) return;
                _FileName = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ButtonName));
            }
        }

        /// <summary>
        /// Пароль от файла
        /// </summary>
        string _FilePassword = "";
        /// <summary>
        /// Пароль от файла
        /// </summary>
        public string FilePassword
        {
            get { return _FilePassword; }
            set
            {
                if (value == _FilePassword) return;
                _FilePassword = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Название кнопки
        /// </summary>
        public string ButtonName
        {
            get
            {
                DirectoryInfo dir = new DirectoryInfo(FullPath + "\\" + FileName);
                if ((int)dir.Attributes == -1 || string.IsNullOrEmpty(FileName))
                {
                    return "Создать зашифрованный\nфаил";
                }
                else
                {
                    return "Открыть зашифрованный\nфаил";
                }
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
                    if (!string.IsNullOrEmpty(FullPath) && !string.IsNullOrEmpty(FileName))
                    {
                        var (name, error) = WindowCryptography.GetTuple(mainWindow, FullPath + "\\" + FileName, FilePassword);
                        if (string.IsNullOrEmpty(error))
                        {
                            mainWindow.Content = new View.WindowCryptography() { DataContext = name };
                        }
                        else
                        {
                            MessageBox.Show(error);
                        }
                        
                    }
                }, b => !string.IsNullOrEmpty(FullPath) && !string.IsNullOrEmpty(FileName)));
            }
        }

        ICommand _FileNameEnter;
        /// <summary>
        /// Нажатие на Enter в текстовом поле с именем файла
        /// </summary>
        public ICommand FileNameEnter
        {
            get
            {
                return _FileNameEnter ?? (_FileNameEnter = new RelayCommand<object>(a =>
                {
                    ((UIElement)a).Focus();
                }));
            }
        }
        #endregion
    }
}
