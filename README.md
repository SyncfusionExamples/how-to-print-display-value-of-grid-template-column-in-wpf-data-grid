# How to print DisplayValue of GridTemplateColumn in WPF DataGrid (SfDataGrid) ?

This sample show cases how to print DisplayValue of GridTemplateColumn in [WPF DataGrid](https://www.syncfusion.com/wpf-ui-controls/datagrid) (SfDataGrid?

# About the sample

You can print a displayed data except GridTemplateColumn [CellTemplate](https://help.syncfusion.com/cr/wpf/Syncfusion.UI.Xaml.Grid.GridColumnBase.html#Syncfusion_UI_Xaml_Grid_GridColumnBase_CellTemplate) and changed it format by overriding the GetColumnElement in GridPrintManager class in [WPF DataGrid](https://www.syncfusion.com/wpf-ui-controls/datagrid) (SfDataGrid).

```c#
protected override object GetColumnElement(object record, string mappingName)
{
    var column = dataGrid.Columns[mappingName];
    if (column.CellTemplate == null)
        return base.GetColumnElement(record, mappingName);
    else
    {
        var tb = new TextBlock
        {
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = GetColumnTextAlignment(mappingName),
            TextWrapping = GetColumnTextWrapping(mappingName),
            FlowDirection = PrintFlowDirection,
            DataContext = record
        };
        tb.SetBinding(TextBlock.TextProperty, column.DisplayBinding);
        decimal value;
        decimal.TryParse(tb.Text, out value);
        tb.Text = value.ToString("F3");
        var padding = column.ReadLocalValue(GridColumn.PaddingProperty);
        tb.Padding = padding != DependencyProperty.UnsetValue
                            ? column.Padding
                            : new Thickness(4, 3, 3, 1);
        return tb;
    }
}
```
## Requirements to run the demo
 Visual Studio 2015 and above versions
