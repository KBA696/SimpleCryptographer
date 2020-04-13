using MVVM;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using ПростойШифровальщик.Model;

namespace ПростойШифровальщик.ViewModel
{
    class SectionDocument : NotificationObject
    {
        InformationGroup informationGroup;
        ObservableCollection<ContentControl> SomeCollection;
        List<InformationGroup> InformationGroup;
        public SectionDocument(InformationGroup informationGroup, ObservableCollection<ContentControl> SomeCollection, List<InformationGroup> InformationGroup)
        {
            this.informationGroup = informationGroup;
            this.SomeCollection = SomeCollection;
            this.InformationGroup = InformationGroup;
        }

        public string Name
        {
            get { return informationGroup.Name; }
            set
            {
                if (value == informationGroup.Name) return;
                informationGroup.Name = value;
                WindowCryptography.ChangedFile();
                OnPropertyChanged();
            }
        }

        public string Informations
        {
            get { return informationGroup.Informations; }
            set
            {
                if (value == informationGroup.Informations) return;
                informationGroup.Informations = value;
                WindowCryptography.ChangedFile();
                OnPropertyChanged();
            }
        }

        ICommand _RemoteInformations;
        public ICommand RemoteInformations
        {
            get
            {
                return _RemoteInformations ?? (_RemoteInformations = new RelayCommand<object>(a =>
                {
                    WindowCryptography.ChangedFile();
                    SomeCollection.Remove(SomeCollection.FirstOrDefault(x => x.DataContext == this));
                    InformationGroup.Remove(informationGroup);
                }));
            }
        }
    }
}
