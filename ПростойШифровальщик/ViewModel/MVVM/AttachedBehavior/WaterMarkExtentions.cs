using System.Windows;
namespace MVVM /*для создания подсказки в текстовом поле и комбоксе*/
{
    public sealed class WaterMarkExtentions
    {
        public static string GetWaterMark(DependencyObject obj)
        {
            return (string)obj.GetValue(WaterMarkProperty);
        }

        public static void SetWaterMark(DependencyObject obj, string value)
        {
            obj.SetValue(WaterMarkProperty, value);
        }

        public static readonly DependencyProperty WaterMarkProperty =
           DependencyProperty.RegisterAttached("WaterMark"
                                       , typeof(string)
                                       , typeof(FrameworkElement)
                                       , new FrameworkPropertyMetadata(""));
    }
}
/*
     <Window.Resources>
        <Style TargetType="TextBox" x:Key="WaterMarkTextboxStyle">
            <Setter Property="Template">
                <Setter.Value>
                    <ControlTemplate TargetType="TextBox">
                        <Border Background="{TemplateBinding Background}" 
                       BorderBrush="{TemplateBinding BorderBrush}"
                       BorderThickness="{TemplateBinding BorderThickness}" >
                            <Grid>
                                <ScrollViewer x:Name="PART_ContentHost" />
                                <TextBlock x:Name="WatermarkText"
                          Text="{Binding WaterMark, 
                              RelativeSource={RelativeSource TemplatedParent}}"
                          Foreground="Gray" Margin="5,0,0,0" 
                          HorizontalAlignment="Left" 
                          VerticalAlignment="Center" 
                          Visibility="Collapsed" 
                          IsHitTestVisible="False"/>
                            </Grid>
                        </Border>
                        <ControlTemplate.Triggers>
                            <MultiTrigger>
                                <MultiTrigger.Conditions>
                                    <Condition Property="IsKeyboardFocusWithin" 
                                   Value="False"/>
                                    <Condition Property="Text" Value=""/>
                                </MultiTrigger.Conditions>
                                <Setter Property="Visibility" 
                           TargetName="WatermarkText" 
                           Value="Visible"/>
                            </MultiTrigger>
                            <MultiTrigger>
                                <MultiTrigger.Conditions>
                                    <Condition Property="IsKeyboardFocusWithin" 
                                   Value="False"/>
                                    <Condition Property="Text" Value="{x:Null}"/>
                                </MultiTrigger.Conditions>
                                <Setter Property="Visibility"
                           TargetName="WatermarkText" 
                           Value="Visible"/>
                            </MultiTrigger>
                        </ControlTemplate.Triggers>
                    </ControlTemplate>
                </Setter.Value>
            </Setter>
        </Style>
    </Window.Resources>
 
 <TextBox local:WaterMarkExtentions.WaterMark = "обозначение" Style="{StaticResource WaterMarkTextboxStyle}"/>*/