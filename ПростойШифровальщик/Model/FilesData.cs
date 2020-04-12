using System;
using System.Collections.ObjectModel;

namespace ПростойШифровальщик.Model
{
    [Serializable]
    public class FilesData
    {
        public ObservableCollection<FilesData> Items { get; set; } = new ObservableCollection<FilesData>();

        public string Name { get; set; }

        public string Adress { get; set; }
    }
}
