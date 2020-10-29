using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace CaptionSummaryCustomization
{
    public class CustomPrintManagerBase : GridPrintManager
    {
        public CustomPrintManagerBase(SfDataGrid dataGrid) : base(dataGrid)
        {

        }

        protected override IList GetSourceListForPrinting()
        {
            if (View.GroupDescriptions.Any())
            {
                var records = ToIEnumerable(View.TopLevelGroup.GetEnumerator()).ToList();
                List<NodeEntry> expandedGroups = new List<NodeEntry>();
                foreach(var record in records)
                {
                    if(record is RecordEntry)
                    {
                        var group = record.Parent as Group;
                        if (group != null && group.IsExpanded)
                            expandedGroups.Add(record);
                    }
                    else if(record is Group)
                    {
                        var group = record as Group;
                        if(group.IsTopLevelGroup || group.Level == 1)
                        {
                            expandedGroups.Add(group);
                        }
                        //For Multi-ColumnGrouping
                        else 
                        {
                            var parentGroup = group.Parent as Group;
                            if (parentGroup != null && parentGroup.IsExpanded)
                                expandedGroups.Add(group);
                        }
                    }
                }
                return expandedGroups;
            }
            return base.GetSourceListForPrinting();
        }

        protected override void AddCaptionSummaryRowToPanel(PrintPagePanel panel, RowInfo rowInfo)
        {
            base.AddCaptionSummaryRowToPanel(panel, rowInfo);
            var cellsInfo = rowInfo.CellsInfo;
            var topThickNess = rowInfo.NeedTopBorder ? 1 : 0;
            var bottomThickness = rowInfo.NeedBottomBorder ? 1 : 0;
            var group = rowInfo.Record as Group;
            for (var start = 0; start < cellsInfo.Count; start++)
            {
                var cellInfo = cellsInfo[start];
                if (IsCaptionSummaryInRow)
                {
                    var cell = GetPrintCaptionSummaryCell(group, cellInfo.ColumnName);
                    cell.FlowDirection = PrintFlowDirection;
                    cell.Width = cellInfo.CellRect.Width;
                    cell.Padding = new Thickness(2, 0, 0, 0);
                    cell.BorderThickness = new Thickness(1, topThickNess, 1, bottomThickness);
                    cell.Content = new TextBlock
                    {
                        Text =
                            View.TopLevelGroup.GetGroupCaptionText(group,
                                GetGroupCaptionStringFormat
                                    (), cellInfo.ColumnName),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    cellInfo.Element = cell;
                    panel.Children.Add(cell);
                }
                else
                {
                    var cell = GetPrintCaptionSummaryCell(group, cellInfo.ColumnName);
                    cell.FlowDirection = PrintFlowDirection;
                    cell.Width = cellInfo.CellRect.Width;
                    cell.Padding = new Thickness(2, 0, 0, 0);
                    
                    //Setting grid lines for PrintCaptionSummaryCell.
                    if (start == 0)
                        cell.BorderThickness = new Thickness(1, topThickNess, 1, bottomThickness);
                    else
                        cell.BorderThickness = new Thickness(0, topThickNess, 1, bottomThickness);

                    var summaryColumns = group.SummaryDetails.SummaryRow.SummaryColumns;
                    if (summaryColumns != null && summaryColumns.Any())
                    {
                        var textBlock = new TextBlock
                        {
                            Padding = new Thickness(2, 0, 0, 0),
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center,
                        };

                        //Setting value for PrintCaptionSummaryCell.
                        if (summaryColumns.Any(x => x.MappingName == cellInfo.ColumnName))
                            textBlock.Text = SummaryCreator.GetSummaryDisplayText(group.SummaryDetails, cellInfo.ColumnName, View);
                        else if (cellInfo.ColumnName == "EmployeeName")
                            textBlock.Text = group.Key.ToString();

                        cell.Content = textBlock;
                        cellInfo.Element = cell;
                        panel.Children.Add(cell);
                    }
                }
            }
        }

        public override ContentControl GetPrintCaptionSummaryCell(object group, string mappingName)
        {
            var cell = base.GetPrintCaptionSummaryCell(group, mappingName);
            cell.HorizontalAlignment = HorizontalAlignment.Right;
            cell.HorizontalContentAlignment = HorizontalAlignment.Right;
            return cell;
        }

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

        /// <summary>
        /// Method to get the Column that is grouped.
        /// </summary>
        public GridColumn GetGroupedColumn(Group group)
        {
            var groupDesc = this.dataGrid.View.GroupDescriptions[group.Level - 1] as PropertyGroupDescription;
            foreach (var column in this.dataGrid.Columns)
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
