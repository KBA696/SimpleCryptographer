using System.Windows;

namespace ПростойШифровальщик
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //https://professorweb.ru/my/WPF/base_WPF/level5/5_12.php
        //https://www.codeproject.com/Articles/36412/Drag-and-Drop-ListBox



        public App()
        {
            //отлавливаем непойманные исключения
            DispatcherUnhandledException += (sender, e) =>
            {
                MessageBox.Show("Фатальное исключение пойманое App " + e.Exception.GetType().ToString() + " отключено " + string.Format("An error occured: {0}", e.Exception.Message) + @"\r\nDispatcherUnhandledExceptionEventArgs - " + e + @"\r\nsender - " + sender);
                e.Handled = true;
            };

            new View.MainWindow() { DataContext = new ViewModel.MainWindow() }.Show();
        }
    }
}
