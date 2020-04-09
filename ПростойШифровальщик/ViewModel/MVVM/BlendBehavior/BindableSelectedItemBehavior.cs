using Microsoft.Xaml.Behaviors;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MVVM 
{
    public class BindableSelectedItemBehavior : Behavior<TreeView>/*Определения что выделенно в TreeView*/
    {
        private static Dictionary<BindableSelectedItemBehavior, DependencyObject> behaviors = new Dictionary<BindableSelectedItemBehavior, DependencyObject>();

        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(BindableSelectedItemBehavior), new UIPropertyMetadata(null, OnSelectedItemChanged));

        private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TreeView view = (TreeView)behaviors[(BindableSelectedItemBehavior)sender];

            TreeViewItem item = (TreeViewItem)view.ItemContainerGenerator.ContainerFromItem(e.NewValue);
            if (item != null)
            {
                item.IsSelected = true;
            }
        }


        protected override void OnAttached()
        {

            base.OnAttached();

            if (!behaviors.ContainsKey(this))
            {
                behaviors.Add(this, AssociatedObject);
            }

            this.AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.SelectedItem = e.NewValue;
        }
    }
}
