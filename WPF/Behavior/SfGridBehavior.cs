using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interactivity;

namespace CaptionSummaryCustomization
{
    public class SfGridBehavior : Behavior<SfDataGrid>
    {
        int count = 1;
        public Dictionary<string, bool> GroupStates = new Dictionary<string, bool>();
        protected override void OnAttached()
        {
            this.AssociatedObject.GroupExpanded += AssociatedObject_GroupExpanded;
            this.AssociatedObject.GroupCollapsed += AssociatedObject_GroupCollapsed;

            //default CaptionSummaryCellRenderer is removed.            
            this.AssociatedObject.CellRenderers.Remove("CaptionSummary");

            //Customized CaptionSummaryCellRenderer is added.
            this.AssociatedObject.CellRenderers.Add("CaptionSummary", new CustomCaptionSummaryCellRenderer());
            this.AssociatedObject.Loaded += AssociatedObject_Loaded;
        }

        private void AssociatedObject_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            this.AssociatedObject.View.CollectionChanged += View_CollectionChanged;

        }

        private void View_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var collectionViewWrapper = sender as GridQueryableCollectionViewWrapper;
            if (collectionViewWrapper != null)
            {
                if(collectionViewWrapper.Groups != null && collectionViewWrapper.Groups.Count > 0)
                {
                    foreach (var group in collectionViewWrapper.Groups)
                    {
                        var grp = group as Group;
                        var key = grp.Key.ToString();
                        if (GroupStates.ContainsKey(key))
                        {
                            grp.IsExpanded = GroupStates[key];
                            while (!grp.IsBottomLevel)
                            {
                                var innergroups = grp.Groups;
                                if (innergroups != null && innergroups.Count > 0)
                                {
                                    foreach (var innerGroup in innergroups)
                                    {
                                        if (GroupStates.ContainsKey(innerGroup.Key.ToString()))
                                            innerGroup.IsExpanded = GroupStates[innerGroup.Key.ToString()];
                                        else
                                            GroupStates.Add(innerGroup.Key.ToString(), innerGroup.IsExpanded);
                                        grp = innerGroup;
                                    }
                                }
                            }
                        }
                        else
                        {
                            GroupStates.Add(key, grp.IsExpanded);
                        }
                    }
                }
            }
            count++;
        }

        private void AssociatedObject_GroupExpanded(object sender, GroupChangedEventArgs e)
        {
            if(GroupStates.ContainsKey(e.Group.Key.ToString()))
                GroupStates[e.Group.Key.ToString()] = e.Group.IsExpanded;
            else
                GroupStates.Add(e.Group.Key.ToString(), e.Group.IsExpanded);
        }

        private void AssociatedObject_GroupCollapsed(object sender, GroupChangedEventArgs e)
        {
            if (GroupStates.ContainsKey(e.Group.Key.ToString()))
                GroupStates[e.Group.Key.ToString()] = e.Group.IsExpanded;
            else
                GroupStates.Add(e.Group.Key.ToString(), e.Group.IsExpanded);
        }

    }
}
