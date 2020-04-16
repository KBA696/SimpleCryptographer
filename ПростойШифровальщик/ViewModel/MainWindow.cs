using MVVM;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace ПростойШифровальщик.ViewModel
{
    class MainWindow : NotificationObject
    {
        public MainWindow()
        {
            Content = new View.WindowFileSelection() { DataContext = new WindowFileSelection(this) };
        }



        ContentControl _Content;
        public ContentControl Content
        {
            get { return _Content; }
            set
            {
                if (value == _Content) return;
                _Content = value;
                OnPropertyChanged();
            }
        }

        ICommand _Window_Closing1;
        public ICommand Window_Closing1
        {
            get
            {
                return _Window_Closing1 ?? (_Window_Closing1 = new RelayCommand<CancelEventArgs>(e =>
                {
                    var dataContext = Content.DataContext as WindowCryptography;
                    if (dataContext != null)
                    {
                        dataContext.Closing(e);
                    }
                }));
            }
        }
    }
}
