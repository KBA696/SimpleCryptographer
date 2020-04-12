using MVVM;
using System.Collections.ObjectModel;
using ПростойШифровальщик.Model;

namespace ПростойШифровальщик.ViewModel
{
    public class FilesDataClass : NotificationObject
    {
        public FilesData filesData;
        public FilesDataClass(FilesData filesData,string st="File")
        {
            this.filesData = filesData;
            Adre = st;
            foreach (var item in this.filesData.Items)
            {
                Items.Add(new FilesDataClass(item, st+@"\"+ filesData.Name));
            }
        }

        public string Adre { get; private set; } 
 
        public string Adress
        {
            get { return filesData.Adress; }
            set
            {
                if (value == filesData.Adress) return;
                filesData.Adress = value;
                MainWindow.ChangedFile();
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
                MainWindow.ChangedFile();
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FilesDataClass> Items { get; set; } = new ObservableCollection<FilesDataClass>();
    }
}
