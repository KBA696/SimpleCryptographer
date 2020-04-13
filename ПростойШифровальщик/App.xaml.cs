using System;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
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

        public static Window window;

        public App()
        {
            //отлавливаем непойманные исключения
            DispatcherUnhandledException += (sender, e) =>
            {
                MessageBox.Show("Фатальное исключение пойманое App " + e.Exception.GetType().ToString() + " отключено " + string.Format("An error occured: {0}", e.Exception.Message) + @"\r\nDispatcherUnhandledExceptionEventArgs - " + e + @"\r\nsender - " + sender);
                e.Handled = true;
            };

            //Проверяем обновление если есть интернет
            (new System.Threading.Thread(delegate () {
                try
                {
                    WebClient client = new WebClient() { Credentials = CredentialCache.DefaultNetworkCredentials };

                    Version versionSite = new Version(client.DownloadString("https://kba696.github.io/SimpleCryptographer/version"));
                    if (versionSite > Assembly.GetExecutingAssembly().GetName().Version)
                    {
                        if (MessageBox.Show("Доступна новая версия. Скачать её?", "Скачать новую версию???", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            OpenBrowser("https://yadi.sk/d/k3dFaqVrHHADeQ");
                        }
                    }
                }
                catch { }
            })).Start();


            //Запускаем саму программу
            window = new View.MainWindow() { DataContext = new ViewModel.MainWindow() };
            window.Show();
        }

        void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
