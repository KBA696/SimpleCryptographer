using MVVM;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ПростойШифровальщик.ViewModel;

namespace ПростойШифровальщик.Model
{
    public class FileSystem : NotificationObject
    {
        public FileSystem(string Name, string ПолныйАдрес)
        {
            this.Name = Name;
            this.ПолныйАдрес = ПолныйАдрес;

            ItemsFil = Items = Scan();
            ItemsFil = ItemsFil.Concat(Scan1());
        }

        /*async void GetItemsAsync()
        {
            var listItems = new ObservableCollection<FileSystem>();
            try
            {
                string[] allfolders = await Directory.GetDirectories(ПолныйАдрес);
                foreach (string folder in allfolders)
                {
                    DirectoryInfo directoryinfo = new DirectoryInfo(folder);
                    Items.Add(new FileSystem(directoryinfo.Name, folder));
                }
            }
            catch { }

        }*/

        IEnumerable<FileSystem> Scan()
        {
            IEnumerable<string> re = new ObservableCollection<string>();
            try
            {
                re = Directory.EnumerateDirectories(ПолныйАдрес);
            }
            catch { }
                // Папки будут идти в начале
                foreach (var dir in re)
                    yield return WindowFileSelection.FileSystemS(dir);
                // Файлы потом
                /*foreach (var file in Directory.EnumerateFiles(path))
                    yield return new FileSystemEntry(Path.GetFileName(file));*/
        }
        IEnumerable<FileSystem> Scan1()
        {
            IEnumerable<string> re = new ObservableCollection<string>();
            try
            {
                re = Directory.EnumerateFiles(ПолныйАдрес);
            }
            catch { }
            // Файлы потом
            foreach (var file in re)
                yield return WindowFileSelection.FileSystemS(file);
        }

        string _Name;
        public string Name
        {
            get { return _Name; }
            set
            {
                if (value == _Name) return;
                _Name = value;
                OnPropertyChanged();
            }
        }

        string _ПолныйАдрес;
        public string ПолныйАдрес
        {
            get { return _ПолныйАдрес; }
            set
            {
                if (value == _ПолныйАдрес) return;
                _ПолныйАдрес = value;
                OnPropertyChanged();
            }
        }


        /*Bitmap fd = null;
        try
        {
            Icon extractedIcon = Icon.ExtractAssociatedIcon(file.ToString());
            fd = extractedIcon.ToBitmap();
            Иконка = Imaging.CreateBitmapSourceFromBitmap(fd)
        }
        catch { }*/

        BitmapSource _Иконка;
        public BitmapSource Иконка
        {
            get { return _Иконка; }
            set
            {
                if (value == _Иконка) return;
                _Иконка = value;
                OnPropertyChanged();
            }
        }
        public IEnumerable<FileSystem> Items { get; private set; }

        public IEnumerable<FileSystem> ItemsFil { get; private set; }


        bool _IsExpanded;
        public bool IsExpanded //для раскрытия узлов дерева
        {
            get { return _IsExpanded; }
            set
            {
                if (value == _IsExpanded) return;
                _IsExpanded = value; 
                OnPropertyChanged();
            }
        }
        bool _IsSelected;
        public bool IsSelected //выделенный(Выбранный) элемент
        {
            get { return _IsSelected; }
            set
            {
                if (value == _IsSelected) return;
                _IsSelected = value; 
                OnPropertyChanged();
            }
        }
    }
}
