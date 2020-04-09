using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MVVM /*выделяет весь текст в TextBox-е при клике на него*/
{
    public class TextBoxProperties
    {
        public static readonly DependencyProperty HighlightTextOnFocusProperty = 
            DependencyProperty.RegisterAttached("HighlightTextOnFocus"
                , typeof(bool)
                , typeof(TextBoxProperties)
                , new PropertyMetadata(false, HighlightTextOnFocusPropertyChanged));

        [AttachedPropertyBrowsableForChildrenAttribute(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(TextBox))]
        public static bool GetHighlightTextOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(HighlightTextOnFocusProperty);
        }

        public static void SetHighlightTextOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(HighlightTextOnFocusProperty, value);
        }

        private static void HighlightTextOnFocusPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var sender = obj as UIElement;
            if (obj != null)
            {
                if ((bool)e.NewValue)
                {
                    sender.GotKeyboardFocus += OnKeyboardFocusSelectText;
                    sender.PreviewMouseLeftButtonDown += OnMouseLeftButtonDownSetFocus;
                }
                else
                {
                    sender.GotKeyboardFocus -= OnKeyboardFocusSelectText;
                    sender.PreviewMouseLeftButtonDown -= OnMouseLeftButtonDownSetFocus;
                }
            }
        }

        private static void OnKeyboardFocusSelectText(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
            {
                textBox.SelectAll();
            }
        }

        private static void OnMouseLeftButtonDownSetFocus(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (sender as TextBox);

            if (tb == null)
                return;

            if (!tb.IsKeyboardFocusWithin)
            {
                tb.Focus();
                e.Handled = true;
            }
        }
    }
}
/*
        private void SelectAddress(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb != null)
            {
                tb.SelectAll();
            }
        }



        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {
            TextBox tb = (sender as TextBox);

            if (tb != null)
            {
                if (!tb.IsKeyboardFocusWithin)
                {
                    e.Handled = true;

                    tb.Focus();
                }
            }
        }*/
