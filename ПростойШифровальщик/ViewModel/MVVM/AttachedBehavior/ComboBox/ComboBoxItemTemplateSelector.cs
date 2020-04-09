using System.Windows.Controls;
using System.Windows;

namespace MVVM.БибдиотекаMVVM.AttachedBehavior
{
    public class ComboBoxItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SelectedTemplate { get; set; }
        public DataTemplate DropDownTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ComboBoxItem comboBoxItem = container.GetVisualParent<ComboBoxItem>();
            if (comboBoxItem == null)
            {
                return SelectedTemplate;
            }
            return DropDownTemplate;
        }
    }
}
