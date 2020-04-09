using System.Windows;
using System.Windows.Controls;

namespace MVVM.БибдиотекаMVVM.AttachedBehavior
{
    //https://meleak.wordpress.com/2012/05/13/different-combobox-itemtemplate-for-dropdown/
    //Нужно чтобы в комбоксе в списке отображалось одно а при выборе другое
    /*
     <Window x:Class="ComboBoxItemTemplateDemo.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:ts="clr-namespace:ComboBoxItemTemplateDemo"
        xmlns:cbp="clr-namespace:ComboBoxItemTemplateDemo"
        Title="MainWindow" Height="350" Width="525">
    <Window.Resources>
        <ts:ComboBoxItemTemplateChooser x:Key="ComboBoxItemTemplateChooser"/>
    </Window.Resources>
    <StackPanel>
        <ComboBox ItemsSource="{Binding MyDataList}"
                  HorizontalAlignment="Center"
                  Width="200"
                  SelectedIndex="0">
            <ComboBox.ItemTemplateSelector>
                <ts:ComboBoxItemTemplateSelector>
                    <ts:ComboBoxItemTemplateSelector.SelectedTemplate>
                        <DataTemplate>
                            <TextBlock Text="{Binding ID}"/>
                        </DataTemplate>
                    </ts:ComboBoxItemTemplateSelector.SelectedTemplate>
                    <ts:ComboBoxItemTemplateSelector.DropDownTemplate>
                        <DataTemplate>
                            <TextBlock Text="{Binding Details}"/>
                        </DataTemplate>
                    </ts:ComboBoxItemTemplateSelector.DropDownTemplate>
                </ts:ComboBoxItemTemplateSelector>
            </ComboBox.ItemTemplateSelector>
        </ComboBox>

        <ComboBox ItemsSource="{Binding MyDataList}"
                  HorizontalAlignment="Center"
                  Width="200"
                  SelectedIndex="0"
                  ItemTemplateSelector="{StaticResource ComboBoxItemTemplateChooser}">
            <cbp:ComboBoxItemTemplateChooser.SelectedTemplate>
                <DataTemplate>
                    <TextBlock Text="{Binding ID}"/>
                </DataTemplate>
            </cbp:ComboBoxItemTemplateChooser.SelectedTemplate>
            <cbp:ComboBoxItemTemplateChooser.DropDownTemplate>
                <DataTemplate>
                    <TextBlock Text="{Binding Details}"/>
                </DataTemplate>
            </cbp:ComboBoxItemTemplateChooser.DropDownTemplate>
        </ComboBox>
    </StackPanel>
</Window>
         */
    public class ComboBoxItemTemplateChooser : DataTemplateSelector
    {
        #region SelectedTemplate

        public static DependencyProperty SelectedTemplateProperty =
            DependencyProperty.RegisterAttached("SelectedTemplate",
                                                typeof(DataTemplate),
                                                typeof(ComboBoxItemTemplateChooser),
                                                new UIPropertyMetadata(null));
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static DataTemplate GetSelectedTemplate(ComboBox obj)
        {
            return (DataTemplate)obj.GetValue(SelectedTemplateProperty);
        }
        public static void SetSelectedTemplate(ComboBox obj, DataTemplate value)
        {
            obj.SetValue(SelectedTemplateProperty, value);
        }

        #endregion // SelectedTemplate

        #region DropDownTemplate

        public static DependencyProperty DropDownTemplateProperty =
            DependencyProperty.RegisterAttached("DropDownTemplate",
                                                typeof(DataTemplate),
                                                typeof(ComboBoxItemTemplateChooser),
                                                new UIPropertyMetadata(null));
        [AttachedPropertyBrowsableForType(typeof(ComboBox))]
        public static DataTemplate GetDropDownTemplate(ComboBox obj)
        {
            return (DataTemplate)obj.GetValue(DropDownTemplateProperty);
        }
        public static void SetDropDownTemplate(ComboBox obj, DataTemplate value)
        {
            obj.SetValue(DropDownTemplateProperty, value);
        }

        #endregion // DropDownTemplate

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ComboBox parentComboBox = null;
            ComboBoxItem comboBoxItem = container.GetVisualParent<ComboBoxItem>();
            if (comboBoxItem == null)
            {
                parentComboBox = container.GetVisualParent<ComboBox>();
                return ComboBoxItemTemplateChooser.GetSelectedTemplate(parentComboBox);
            }
            parentComboBox = ComboBox.ItemsControlFromItemContainer(comboBoxItem) as ComboBox;
            return ComboBoxItemTemplateChooser.GetDropDownTemplate(parentComboBox);
        }
    }
}
