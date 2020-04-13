using MVVM;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
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
        public WindowFileSelection(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;

            foreach (DriveInfo drive1 in DriveInfo.GetDrives())
            {
                UserFiles.Add(new FileSystem() { Name = drive1.ToString(),  });

                string[] allfolders = Directory.GetDirectories(drive1.ToString());
                foreach (string folder in allfolders)
                {
                    Console.WriteLine(folder);
                }
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
    }
}
