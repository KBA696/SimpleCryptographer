using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;

namespace MVVM
{
    /// <summary>
    ///     Behavior to make a connection from the <see cref="Password"/>
    ///     from the <see cref="System.Windows.Controls.PasswordBox"/>
    /// </summary>
    /// <author>Cyril Schumacher</author>
    /// <date>26/06/2014T22:57:46+01:00</date>
    public class PasswordBoxBehavior : Behavior<PasswordBox> /*значение для пароля*/
    {
        #region Поля раздела.

        /// <summary>
        ///    Dependency for the <see cref="Password"/>.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty;

        #endregion Поля раздела.

        #region Свойства раздела.

        /// <summary>
        ///     Gets or sets the password.
        /// </summary>
        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        #endregion Свойства раздела.

        #region Раздел конструкторы.

        /// <summary>
        ///     Static constructor.
        /// </summary>
        static PasswordBoxBehavior()
        {
            PasswordProperty = DependencyProperty.Register("Password", typeof(string), typeof(PasswordBoxBehavior),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    _OnPasswordChanged));
        }

        #endregion Статический конструктор.

        /// <summary>
        ///     Occurs when the value of the <see cref="Password"/> is updated.
        /// </summary>
        /// <param name="d">Sender object.</param>
        /// <param name="e">Parameters.</param>
        private static void _OnPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as PasswordBoxBehavior;
            if ((behavior != null) && (behavior.AssociatedObject != null))
            {
                if ((behavior.AssociatedObject.Password != null) && (behavior.AssociatedObject.Password != e.NewValue.ToString()))
                {
                    behavior.AssociatedObject.Password = e.NewValue.ToString();
                }
            }
        }

        /// <summary>
        ///     Occurs when the <see cref="PasswordBox.Password"/> is updated.
        /// </summary>
        /// <param name="d">Sender object.</param>
        /// <param name="e">Parameters.</param>
        private void _AssociatedObjectOnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if ((passwordBox != null) && (Password != passwordBox.Password))
            {
                Password = passwordBox.Password;
            }
        }



        /// <summary>
        ///     Called after the behavior is attached to an <see cref="Behavior.AssociatedObject"/>.
        /// </summary>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PasswordChanged += _AssociatedObjectOnPasswordChanged;
        }

        /// <summary>
        ///     Called when the behavior is being detached from its <see cref="Behavior.AssociatedObject"/>,
        ///     but before it has actually occurred.
        /// </summary>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PasswordChanged -= _AssociatedObjectOnPasswordChanged;
        }


    }
}
