using System.Windows;
using System.Windows.Input;

namespace MVVM
{
    public static class WindowCloseBehaviour/*закрытие окна нажав esc*/
    {
        public static void SetClose(DependencyObject target, bool value)
        {
            target.SetValue(CloseProperty, value);
        }
        public static readonly DependencyProperty CloseProperty =
                                                        DependencyProperty.RegisterAttached(
                                                        "Close",
                                                        typeof(bool),
                                                        typeof(WindowCloseBehaviour),
                                                        new UIPropertyMetadata(false, OnClose));

        private static void OnClose(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is bool && ((bool)e.NewValue))
            {
                Window window = GetWindow(sender);
                if (window != null)
                {
                    window.KeyUp -= WindowKeyUp;
                    window.KeyUp += WindowKeyUp;
                };
            }
        }

        private static void WindowKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Window window = sender as Window;
                if (window != null)
                    window.Close();
            }
        }

        private static Window GetWindow(DependencyObject sender)
        {
            Window window = null;
            if (sender is Window)
                window = (Window)sender;
            if (window == null)
                window = Window.GetWindow(sender);
            return window;
        }
    }
}

/*
<Window x:Class="WpfApplicationMVVM.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:Общие="clr-namespace:Общие;assembly=System.Windows.Interactivity"
        Title="Library" Height="350" Width="525"
        Общие:WindowCloseBehaviour.Close="True">
    <Grid>
    </Grid>
</Window>
*/
