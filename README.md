# How to print DisplayValue of GridTemplateColumn in WPF DataGrid (SfDataGrid) ?

How to print DisplayValue of GridTemplateColumn in WPF DataGrid (SfDataGrid) ?

# About the sample

In SfDataGrid, you can print a displayed data except GridTemplateColumn CellTemplate and changed it format by overriding the GetColumnElement in GridPrintManager class.

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
