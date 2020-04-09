using System.Windows;
using System.Windows.Input;

namespace MVVM.БибдиотекаMVVM.AttachedBehavior
{
    public class MouseBehaviour/*Команды событий мыши для MVVM*/
    {
        /*http://csharpcoding.org/komandy-sobytij-myshi-dlya-mvvm/
         Можно использовать для следующих событий:
            MouseUp
            MouseDown
            MouseEnter
            MouseLeave
            MouseLeftButtonDown
            MouseLeftButtonUp
            MouseMove
            MouseRightButtonDown
            MouseRightButtonUp
            MouseWheel
             */
        public static readonly DependencyProperty MouseMoveCommandProperty =
            DependencyProperty.RegisterAttached("MouseMoveCommand", typeof(ICommand), typeof(MouseBehaviour),
            new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseMoveCommandChanged)));

        static void MouseMoveCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement el = d as FrameworkElement;
            if (el != null)
            {
                el.MouseMove += el_MouseMove;
            }
        }

        public class MouseMoveObject
        {
            public MouseMoveObject(object sender, MouseEventArgs e)
            {
                this.sender = sender;
                this.e = e;
            }
            public object sender;
            public MouseEventArgs e;
        }

        static void el_MouseMove(object sender, MouseEventArgs e)
        {
            FrameworkElement el = (FrameworkElement)sender;
            if (el != null)
            {
                ICommand command = GetMouseMoveCommand(el);

                command.Execute(new MouseMoveObject(sender, e));
            }
        }

        public static void SetMouseMoveCommand(UIElement d, ICommand value)
        {
            d.SetValue(MouseMoveCommandProperty, value);
        }

        public static ICommand GetMouseMoveCommand(UIElement el)
        {
            return (ICommand)el.GetValue(MouseMoveCommandProperty);
        }


        public static readonly DependencyProperty MouseUpCommandProperty =
    DependencyProperty.RegisterAttached("MouseUpCommand", typeof(ICommand), typeof(MouseBehaviour),
    new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseUpCommandChanged)));

        static void MouseUpCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement el = d as FrameworkElement;
            if (el != null)
            {
                el.MouseUp += el_MouseUp;
            }
        }
        public class MouseUuDownObject
        {
            public MouseUuDownObject(object sender, MouseButtonEventArgs e)
            {
                this.sender = sender;
                this.e = e;
            }
            public object sender;
            public MouseButtonEventArgs e;
        }
        static void el_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement el = (FrameworkElement)sender;
            if (el != null)
            {
                ICommand command = GetMouseUpCommand(el);
                command.Execute(new MouseUuDownObject(sender, e));
            }
        }

        public static void SetMouseUpCommand(UIElement d, ICommand value)
        {
            d.SetValue(MouseUpCommandProperty, value);
        }

        public static ICommand GetMouseUpCommand(UIElement el)
        {
            return (ICommand)el.GetValue(MouseUpCommandProperty);
        }


        public static readonly DependencyProperty MouseDownCommandProperty =
    DependencyProperty.RegisterAttached("MouseDownCommand", typeof(ICommand), typeof(MouseBehaviour),
    new FrameworkPropertyMetadata(new PropertyChangedCallback(MouseDownCommandChanged)));

        static void MouseDownCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement el = d as FrameworkElement;
            if (el != null)
            {
                el.MouseDown += el_MouseDown;
            }
        }

        static void el_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement el = (FrameworkElement)sender;
            if (el != null)
            {
                ICommand command = GetMouseDownCommand(el);
                command.Execute(new MouseUuDownObject(sender, e));
            }
        }

        public static void SetMouseDownCommand(UIElement d, ICommand value)
        {
            d.SetValue(MouseDownCommandProperty, value);
        }

        public static ICommand GetMouseDownCommand(UIElement el)
        {
            return (ICommand)el.GetValue(MouseDownCommandProperty);
        }
    }
}