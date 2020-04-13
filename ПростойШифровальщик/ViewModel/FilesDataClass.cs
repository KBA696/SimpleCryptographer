using MVVM;
using System.Collections.ObjectModel;
using ПростойШифровальщик.Model;

namespace ПростойШифровальщик.ViewModel
{
    public class FilesDataClass : NotificationObject
    {
        public FilesData filesData;
        public FilesDataClass зкувб;
        public FilesDataClass(FilesData filesData, FilesDataClass зкувб, string st ="File")
        {
            this.зкувб = зкувб;
            this.filesData = filesData;
            Adre = st;
            foreach (var item in this.filesData.Items)
            {
                Items.Add(new FilesDataClass(item,this, st+@"\"+ filesData.Name));
            }
        }

        public void Del()
        {
            зкувб.Dela(this);
        }

        void Dela(FilesDataClass filesDataClass)
        {
            filesData.Items.Remove(filesDataClass.filesData);
            Items.Remove(filesDataClass);
        }

        public string Adre { get; private set; } 
 
        public string Adress
        {
            get { return filesData.Adress; }
            set
            {
                if (value == filesData.Adress) return;
                filesData.Adress = value;
                WindowCryptography.ChangedFile();
                OnPropertyChanged();
            }
        }       

        public string Name
        {
            get { return filesData.Name; }
            set
            {
                if (value == filesData.Name) return;
                filesData.Name = value;
                WindowCryptography.ChangedFile();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FilesDataClass> Items { get; set; } = new ObservableCollection<FilesDataClass>();


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
