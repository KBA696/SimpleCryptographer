﻿using MVVM;
using System.ComponentModel;
using System.Windows;
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
    }
}
