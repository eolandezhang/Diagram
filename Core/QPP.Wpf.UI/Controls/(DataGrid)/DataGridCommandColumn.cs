using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace QPP.Wpf.UI.Controls
{
    public class DataGridCommandColumn : DataGridColumn
    {
        public static readonly DependencyProperty ShowAddCommandProperty;
        public static readonly DependencyProperty ShowEditCommandProperty;
        public static readonly DependencyProperty ShowDeleteCommandProperty;
        public static readonly DependencyProperty AddCommandProperty;
        public static readonly DependencyProperty EditCommandProperty;
        public static readonly DependencyProperty DeleteCommandProperty;
        public static readonly DependencyProperty AddButtonContentProperty;
        public static readonly DependencyProperty EditButtonContentProperty;
        public static readonly DependencyProperty DeleteButtonContentProperty;

        static DataGridCommandColumn()
        {
            var thisType = typeof(DataGridCommandColumn);
            
            ShowAddCommandProperty = DependencyProperty.Register("ShowAddCommand", typeof(bool), thisType, new PropertyMetadata(false));
            ShowEditCommandProperty = DependencyProperty.Register("ShowEditCommand", typeof(bool), thisType, new PropertyMetadata(true));
            ShowDeleteCommandProperty = DependencyProperty.Register("ShowDeleteCommand", typeof(bool), thisType, new PropertyMetadata(true));
            AddCommandProperty = DependencyProperty.Register("AddCommand", typeof(ICommand), thisType, new PropertyMetadata(null));
            EditCommandProperty = DependencyProperty.Register("EditCommand", typeof(ICommand), thisType, new PropertyMetadata(null));
            DeleteCommandProperty = DependencyProperty.Register("DeleteCommand", typeof(ICommand), thisType, new PropertyMetadata(null));
            AddButtonContentProperty = DependencyProperty.Register("AddButtonContent", typeof(object), thisType, new PropertyMetadata(null));
            EditButtonContentProperty = DependencyProperty.Register("EditButtonContent", typeof(object), thisType, new PropertyMetadata(null));
            DeleteButtonContentProperty = DependencyProperty.Register("DeleteButtonContent", typeof(object), thisType, new PropertyMetadata(null));
        }

        public DataGridCommandColumn()
        {
            CanUserResize = false;
            CanUserReorder = false;
            DataGridHelper.SetCanHide(this, false);
        }

        #region Property

        public ICommand AddCommand
        {
            get { return (ICommand)GetValue(AddCommandProperty); }
            set { SetValue(AddCommandProperty, value); }
        }

        public ICommand EditCommand
        {
            get { return (ICommand)GetValue(EditCommandProperty); }
            set { SetValue(EditCommandProperty, value); }
        }

        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        public bool ShowAddCommand
        {
            get { return (bool)GetValue(ShowAddCommandProperty); }
            set { SetValue(ShowAddCommandProperty, value); }
        }

        public bool ShowEditCommand
        {
            get { return (bool)GetValue(ShowEditCommandProperty); }
            set { SetValue(ShowEditCommandProperty, value); }
        }

        public bool ShowDeleteCommand
        {
            get { return (bool)GetValue(ShowDeleteCommandProperty); }
            set { SetValue(ShowDeleteCommandProperty, value); }
        }

        public object AddButtonContent
        {
            get { return GetValue(AddButtonContentProperty); }
            set { SetValue(AddButtonContentProperty, value); }
        }

        public object EditButtonContent
        {
            get { return GetValue(EditButtonContentProperty); }
            set { SetValue(EditButtonContentProperty, value); }
        }

        public object DeleteButtonContent
        {
            get { return GetValue(DeleteButtonContentProperty); }
            set { SetValue(DeleteButtonContentProperty, value); }
        }

        #endregion

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return CreateElement(cell, dataItem);
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return CreateElement(cell, dataItem);
        }

        Button CreateButton()
        {
            Button btn = new Button();
            btn.BorderThickness = new Thickness(0);
            btn.Background = new SolidColorBrush(Colors.Transparent);
            btn.Padding = new Thickness(3, 0, 3, 0);
            btn.Margin = new Thickness(0);
            btn.MinHeight = 20;
            btn.VerticalContentAlignment = VerticalAlignment.Center;
            return btn;
        }

        FrameworkElement CreateElement(DataGridCell cell, object dataItem)
        {
            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            if (ShowAddCommand)
            {
                Button btnAdd = CreateButton();
                btnAdd.CommandParameter = dataItem;
                btnAdd.Command = AddCommand;
                if (AddButtonContent == null)
                    btnAdd.Content = new Image() { Source = new BitmapImage(new Uri("/QPP.Wpf.UI;component/Themes/Generic/Images/add.png", UriKind.RelativeOrAbsolute)), MaxHeight = 16 };
                else
                    btnAdd.Content = GetButtonContent(AddButtonContent);
                btnAdd.SetResourceReference(Button.StyleProperty, ToolBar.ButtonStyleKey);
                btnAdd.Name = "btn_add";
                panel.Children.Add(btnAdd);
            }
            if (ShowEditCommand)
            {
                Button btnEdit = CreateButton();
                btnEdit.CommandParameter = dataItem;
                btnEdit.Command = EditCommand;
                if (EditButtonContent == null)
                    btnEdit.Content = new Image() { Source = new BitmapImage(new Uri("/QPP.Wpf.UI;component/Themes/Generic/Images/pencil.png", UriKind.RelativeOrAbsolute)), MaxHeight = 16 };
                else
                    btnEdit.Content = GetButtonContent(EditButtonContent);
                btnEdit.SetResourceReference(Button.StyleProperty, ToolBar.ButtonStyleKey);
                btnEdit.Name = "btn_edit";
                panel.Children.Add(btnEdit);
            }
            if (ShowDeleteCommand)
            {
                Button btnDelete = CreateButton();
                btnDelete.CommandParameter = dataItem;
                btnDelete.Command = DeleteCommand;
                if (DeleteButtonContent == null)
                    btnDelete.Content = new Image() { Source = new BitmapImage(new Uri("/QPP.Wpf.UI;component/Themes/Generic/Images/remove.png", UriKind.RelativeOrAbsolute)), MaxHeight = 16 };
                else
                    btnDelete.Content = GetButtonContent(DeleteButtonContent);
                btnDelete.SetResourceReference(Button.StyleProperty, ToolBar.ButtonStyleKey);
                btnDelete.Name = "btn_delete";
                panel.Children.Add(btnDelete);
            }
            cell.Tag = panel;
            cell.DataContextChanged += new DependencyPropertyChangedEventHandler(cell_DataContextChanged);
            return panel;
        }

        object GetButtonContent(object content)
        {
            if (content is FrameworkElement)
            {
                string xaml = System.Windows.Markup.XamlWriter.Save(content);
                return System.Windows.Markup.XamlReader.Parse(xaml);
            }
            return content;
        }

        void cell_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var cell = sender as DataGridCell;
            var panel = cell.Content as StackPanel;
            foreach (var b in panel.Children.OfType<Button>())
                b.CommandParameter = cell.DataContext;
        }
    }
}
