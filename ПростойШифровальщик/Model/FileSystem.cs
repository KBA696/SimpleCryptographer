using MVVM;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ПростойШифровальщик.Model
{
    public class FileSystem : NotificationObject
    {
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
        public ObservableCollection<FileSystem> Items { get; set; } = new ObservableCollection<FileSystem>();


        bool _IsExpanded;
        public bool IsExpanded //для раскрытия узлов дерева
        {
            get { return _IsExpanded; }
            set
            {
                if (value == _IsExpanded) return;
                _IsExpanded = value; OnPropertyChanged();

                /*if (Items != null && Items.Count != 0)
                {
                    if (value && Items[0].Детал.Обозначение == "Загрузка...")
                    {
                        Обновить();
                        OnPropertyChanged(nameof(Items));
                    }
                }*/
                OnPropertyChanged(nameof(IsExpanded));
            }
        }
        bool _IsSelected;
        public bool IsSelected //выделенный(Выбранный) элемент
        {
            get { return _IsSelected; }
            set
            {
                if (value == _IsSelected) return;
                _IsSelected = value; OnPropertyChanged();
            }
        }
    }
}
