using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace MyLights.Util
{
    public class SharedVisibilityBehavior : Behavior<UIElement>
    {
        protected override void OnAttached()
        {
            AssociatedObject.IsVisibleChanged += AssociatedObject_IsVisibleChanged;
            Register(GroupName, this);
        }

        private void AssociatedObject_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsSource)
            {
                var vis = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;

                if (GroupName != null)
                {
                    foreach (var behavior in groups[GroupName].Where(e => e != this))
                    {
                        behavior.AssociatedObject.Visibility = vis;
                    }
                }
            }
        }

        protected override void OnDetaching()
        {
            AssociatedObject.IsVisibleChanged -= AssociatedObject_IsVisibleChanged;
            Deregister(GroupName, this);
        }

        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(SharedVisibilityBehavior),
                new PropertyMetadata("", (s, e) => ((SharedVisibilityBehavior)s).OnGroupNameChanged(e)));



        public bool IsSource
        {
            get { return (bool)GetValue(IsSourceProperty); }
            set { SetValue(IsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSourceProperty =
            DependencyProperty.Register("IsSource", typeof(bool), typeof(SharedVisibilityBehavior), new PropertyMetadata(false));



        private void OnGroupNameChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is string oldValue)
                Deregister(oldValue, this);

            if (e.NewValue is string newValue)
                Register(newValue, this);
        }

        static private void Register(string groupName, SharedVisibilityBehavior behavior)
        {
            if (groupName == null || groupName == "")
                return;

            if (!groups.ContainsKey(groupName))
            {
                groups[groupName] = new List<SharedVisibilityBehavior>();
            }

            if (!groups[groupName].Contains(behavior))
                groups[groupName].Add(behavior);
        }

        static private void Deregister(string groupName, SharedVisibilityBehavior behavior)
        {
            if (groupName == null || groupName == "")
                return;

            if (groups[groupName].Count == 1)
            {
                groups.Remove(groupName);
            }
            else
            {
                groups[groupName].Remove(behavior);
            }
        }

        #region Static

        static Dictionary<string, List<SharedVisibilityBehavior>> groups =
            new Dictionary<string, List<SharedVisibilityBehavior>>();

        #endregion
    }
}