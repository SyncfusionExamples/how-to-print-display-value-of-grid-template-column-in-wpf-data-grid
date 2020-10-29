using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Cells;
using Syncfusion.UI.Xaml.ScrollAxis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace CaptionSummaryCustomization
{
    public class CustomCaptionSummaryCellRenderer : GridCaptionSummaryCellRenderer
    {
        public override void OnInitializeEditElement(DataColumnBase dataColumn, GridCaptionSummaryCell uiElement, object dataContext)
        {
            if (dataContext is Group)
            {
                var groupRecord = dataContext as Group;
                if (this.DataGrid.CaptionSummaryRow.ShowSummaryInRow)
                {
                    uiElement.Content = SummaryCreator.GetSummaryDisplayTextForRow(groupRecord.SummaryDetails,
                        this.DataGrid.View);
                }
                else
                {
                    uiElement.Content = SummaryCreator.GetSummaryDisplayText(groupRecord.SummaryDetails,
                        dataColumn.GridColumn.MappingName, this.DataGrid.View);
                    if (uiElement.Content == (object)"" && dataColumn.GridColumn.MappingName == "EmployeeName")//&& this.DataGrid.GroupColumnDescriptions.Any(col => col.ColumnName == dataColumn.GridColumn.MappingName))
                    {
                        uiElement.Content = groupRecord.Key;
                    }
                }
            }
        }
        public override void OnUpdateEditBinding(DataColumnBase dataColumn, GridCaptionSummaryCell element, object dataContext)
        {
            if (element.DataContext is Group && this.DataGrid.View.GroupDescriptions.Count > 0)
            {
                var groupRecord = element.DataContext as Group;
                //get the column which is grouped.
                var groupedColumn = this.GetGroupedColumn(groupRecord);
                if (this.DataGrid.CaptionSummaryRow.ShowSummaryInRow)
                {
                    element.Content = SummaryCreator.GetSummaryDisplayTextForRow(groupRecord.SummaryDetails,
                        this.DataGrid.View, groupedColumn.HeaderText);
                }
                else
                {
                    element.Content = SummaryCreator.GetSummaryDisplayText(groupRecord.SummaryDetails,
                        dataColumn.GridColumn.MappingName, this.DataGrid.View);

                    if(element.Content == (object)"" && dataColumn.GridColumn.MappingName == this.DataGrid.Columns.FirstOrDefault(col => !col.IsHidden).MappingName)
                    {
                        element.Content = groupRecord.Key;
                    }
                }
            }
            
        }
        public override void UpdateToolTip(DataColumnBase dataColumn)
        {
            base.UpdateToolTip(dataColumn);
            var uiElement = dataColumn.ColumnElement;
            var column = dataColumn.GridColumn;
            var group = dataColumn.ColumnElement.DataContext as Group;
            var obj = ToolTipService.GetToolTip(uiElement);
            ToolTip tooltip;
            if (obj is ToolTip)
                tooltip = obj as ToolTip;
            else
                tooltip = new ToolTip();
            
            var summaryColumns = group.SummaryDetails.SummaryRow.SummaryColumns;
            var isSummaryColumn = summaryColumns.Any(col => col.MappingName == column.MappingName);
           
            if(isSummaryColumn)
            {
                tooltip.Content = SummaryCreator.GetSummaryDisplayText(group.SummaryDetails,
                        dataColumn.GridColumn.MappingName, this.DataGrid.View);
                ToolTipService.SetToolTip(uiElement, tooltip);
            }
            
        }

        // Method to get the Grouped Column.
        private GridColumn GetGroupedColumn(Group group)
        {
            var groupDesc = this.DataGrid.View.GroupDescriptions[group.Level - 1] as PropertyGroupDescription;
            foreach (var column in this.DataGrid.Columns)
            {
                if (column.MappingName == groupDesc.PropertyName)
                {
                    return column;
                }
            }
            return null;
        }
    }
}
