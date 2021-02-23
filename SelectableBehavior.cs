using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace MyLights
{
    public class SelectableBehavior : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {

            base.OnAttached();
            AssociatedObject.MouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
        }

        private void AssociatedObject_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!IsSelected)
            {
                var oldSelection = selectionGroups[SelectionGroup];
                if (oldSelection != null)
                {
                    oldSelection.IsSelected = false;
                    oldSelection.HideHandle();
                }

                selectionGroups[SelectionGroup] = this;
                IsSelected = true;
                ShowHandle();

                OnGroupSelectionChanged(SelectionGroup, AssociatedObject);
            }
        }

        private void ShowHandle()
        {

        }

        private void HideHandle()
        {
            throw new NotImplementedException();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseLeftButtonDown -= AssociatedObject_MouseLeftButtonDown;
            base.OnDetaching();
        }

        private static Dictionary<string, SelectableBehavior> selectionGroups = new Dictionary<string, SelectableBehavior>();

        public string SelectionGroup
        {
            get { return (string)GetValue(SelectionGroupProperty); }
            set { SetValue(SelectionGroupProperty, value); }
        }

        public static readonly DependencyProperty SelectionGroupProperty =
            DependencyProperty.Register("SelectionGroup", typeof(string), typeof(SelectableBehavior),
                new PropertyMetadata("", (s, e) => ((SelectableBehavior)s).OnSelectionGroupChanged(e)));

        private void OnSelectionGroupChanged(DependencyPropertyChangedEventArgs e)
        {
            if (!selectionGroups.ContainsKey(SelectionGroup))
                selectionGroups.Add(SelectionGroup, null);
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(SelectableBehavior),
                new PropertyMetadata(false, (s, e) => ((SelectableBehavior)s).OnIsSelectedChanged(e)));

        private void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        public static FrameworkElement GetSelectedItem(string groupName)
        {
            if (selectionGroups.ContainsKey(groupName))
                return selectionGroups[groupName].AssociatedObject;
            else
                return null;
        }

        public static event GroupSelectionChangedEventHandler GroupSelectionChanged;

        private static void OnGroupSelectionChanged(string groupName, FrameworkElement selectedItem)
        {
            var handler = GroupSelectionChanged;
            handler?.Invoke(new GroupSelectionChangedEventArgs(groupName, selectedItem));
        }
    }

    public delegate void GroupSelectionChangedEventHandler(GroupSelectionChangedEventArgs e);

    public class GroupSelectionChangedEventArgs
    {
        public GroupSelectionChangedEventArgs(string groupName, FrameworkElement selectedItem)
        {
            this.GroupName = groupName;
            this.SelectedItem = selectedItem;
        }

        public string GroupName { get; }
        public FrameworkElement SelectedItem { get; }
    }

    public class SelectableBehaviorSelection : DependencyObject
    {
        public SelectableBehaviorSelection()
        {
            SelectableBehavior.GroupSelectionChanged += SelectableBehavior_GroupSelectionChanged;
        }

        private void SelectableBehavior_GroupSelectionChanged(GroupSelectionChangedEventArgs e)
        {
            if (e.GroupName == GroupName)
            {
                if (BindToData)
                    SelectedItem = e.SelectedItem.DataContext;
                else
                    SelectedItem = e.SelectedItem;
            }
        }

        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(SelectableBehaviorSelection),
                new PropertyMetadata(""));



        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedItem.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(SelectableBehaviorSelection), new PropertyMetadata(null));

        public bool BindToData { get; set; }

    }
}
