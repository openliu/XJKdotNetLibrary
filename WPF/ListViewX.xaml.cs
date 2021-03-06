﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace XJK.WPF
{
    /// <summary>
    /// ListViewX.xaml 的交互逻辑
    /// </summary>
    public partial class ListViewX : ListView
    {
        public event ItemMouseDoubleClickEventHandler ItemMouseDoubleClick;

        public static RoutedCommand FirstItemCommand = new RoutedCommand();
        public static RoutedCommand LastItemCommand = new RoutedCommand();
        public static RoutedCommand NextItemCommand = new RoutedCommand();
        public static RoutedCommand PrevItemCommand = new RoutedCommand();

        public ListViewX()
        {
            InitializeComponent();
            this.CommandBindings.AddRange(new Collection<CommandBinding>()
            {
                new CommandBinding(FirstItemCommand, (sender,e)=>{ SelectedIndex = 0; }),
                new CommandBinding(LastItemCommand, (sender,e)=>{ SelectedIndex = Items.Count - 1; }),
                new CommandBinding(NextItemCommand, (sender,e)=>{ SelectedIndex = (SelectedIndex + 1) % Items.Count; }),
                new CommandBinding(PrevItemCommand, (sender,e)=>{ SelectedIndex = (SelectedIndex - 1 + Items.Count) % Items.Count; }),
            });
        }


        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ItemMouseDoubleClick?.Invoke(sender, new ItemMouseDoubleClickEventArgs());
        }

        private GridViewColumnHeader lastHeaderClicked = null;
        private ListSortDirection lastDirection = ListSortDirection.Ascending;

        private void GridViewColumnHeaderClicked(object sender, RoutedEventArgs e)
        {
            ListSortDirection direction;
            if (e.OriginalSource is GridViewColumnHeader clickedHeader)
            {
                if (clickedHeader.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (clickedHeader != lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }
                    string sortString = clickedHeader.Column.GetSortBy();
                    if (!string.IsNullOrEmpty(sortString))
                    {
                        Sort(sortString, direction);

                        lastHeaderClicked = clickedHeader;
                        lastDirection = direction;
                    }
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(ItemsSource ?? Items);
            dataView.SortDescriptions.Clear();
            SortDescription sD = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sD);
            dataView.Refresh();
        }

        public void RefreshView()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(ItemsSource ?? Items);
            view.Refresh();
        }

        private void ListView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Focus();
        }

        private void ListView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HitTestResult r = VisualTreeHelper.HitTest(this, e.GetPosition(this));
            var parent = r.VisualHit.FindVisualTreeParent(typeof(ListViewItem), this);
            if (parent == null)
            {
                this.UnselectAll();
            }
        }
    }
}
