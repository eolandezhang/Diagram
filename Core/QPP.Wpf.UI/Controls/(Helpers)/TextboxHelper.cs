using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace QPP.Wpf.UI.Controls
{
    /// <summary>
    /// Password watermarking code from: http://prabu-guru.blogspot.com/2010/06/how-to-add-watermark-text-to-textbox.html
    /// </summary>
    public class TextboxHelper : DependencyObject
    {
        public static readonly DependencyProperty AutoScrollToEndProperty;
        public static readonly DependencyProperty IsMonitoringProperty;
        public static readonly DependencyProperty WatermarkProperty;
        public static readonly DependencyProperty ClearTextButtonProperty;
        public static readonly DependencyProperty SelectAllOnFocusProperty;
        public static readonly DependencyProperty MoveFocusOnEnterProperty;
        public static readonly DependencyProperty UpdateSourceOnEnterProperty;
        public static readonly DependencyProperty EnterCommandProperty;
        public static readonly DependencyProperty AccessoryProperty;
        private static readonly DependencyProperty hasTextProperty;
        static readonly DependencyProperty TextLengthProperty;

        static TextboxHelper()
        {
            AutoScrollToEndProperty = DependencyProperty.RegisterAttached("AutoScrollToEnd", typeof(bool), typeof(TextboxHelper), new UIPropertyMetadata(false));
            IsMonitoringProperty = DependencyProperty.RegisterAttached("IsMonitoring", typeof(bool), typeof(TextboxHelper), new UIPropertyMetadata(false, OnIsMonitoringChanged));
            WatermarkProperty = DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(TextboxHelper), new UIPropertyMetadata(string.Empty));
            TextLengthProperty = DependencyProperty.RegisterAttached("TextLength", typeof(int), typeof(TextboxHelper), new UIPropertyMetadata(0));
            ClearTextButtonProperty = DependencyProperty.RegisterAttached("ClearTextButton", typeof(bool), typeof(TextboxHelper), new FrameworkPropertyMetadata(false, ButtonCommandOrClearTextChanged));
            SelectAllOnFocusProperty = DependencyProperty.RegisterAttached("SelectAllOnFocus", typeof(bool), typeof(TextboxHelper), new FrameworkPropertyMetadata(false));
            MoveFocusOnEnterProperty = DependencyProperty.RegisterAttached("MoveFocusOnEnter", typeof(bool), typeof(TextboxHelper), new FrameworkPropertyMetadata(false));
            UpdateSourceOnEnterProperty = DependencyProperty.RegisterAttached("UpdateSourceOnEnter", typeof(bool), typeof(TextboxHelper), new FrameworkPropertyMetadata(false));
            EnterCommandProperty = DependencyProperty.RegisterAttached("EnterCommand", typeof(ICommand), typeof(TextboxHelper));
            AccessoryProperty = DependencyProperty.RegisterAttached("Accessory", typeof(object), typeof(TextboxHelper));
            hasTextProperty = DependencyProperty.RegisterAttached("HasText", typeof(bool), typeof(TextboxHelper), new FrameworkPropertyMetadata(false));
        }

        public static void SetAccessory(DependencyObject obj, object value)
        {
            obj.SetValue(AccessoryProperty, value);
        }

        public static object GetAccessory(DependencyObject obj)
        {
            return obj.GetValue(AccessoryProperty);
        }

        public static void SetAutoScrollToEnd(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollToEndProperty, value);
        }

        public static bool GetAutoScrollToEnd(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollToEndProperty);
        }

        public static void SetEnterCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(EnterCommandProperty, value);
        }

        public static ICommand GetEnterCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(EnterCommandProperty);
        }

        public static void SetUpdateSourceOnEnter(DependencyObject obj, bool value)
        {
            obj.SetValue(UpdateSourceOnEnterProperty, value);
        }

        public static bool GetUpdateSourceOnEnter(DependencyObject obj)
        {
            return (bool)obj.GetValue(UpdateSourceOnEnterProperty);
        }

        public static void SetSelectAllOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(SelectAllOnFocusProperty, value);
        }

        public static bool GetSelectAllOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(SelectAllOnFocusProperty);
        }

        public static void SetMoveFocusOnEnter(DependencyObject obj, bool value)
        {
            obj.SetValue(MoveFocusOnEnterProperty, value);
        }

        public static bool GetMoveFocusOnEnter(DependencyObject obj)
        {
            return (bool)obj.GetValue(MoveFocusOnEnterProperty);
        }

        public static void SetIsMonitoring(DependencyObject obj, bool value)
        {
            obj.SetValue(IsMonitoringProperty, value);
        }

        public static string GetWatermark(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        private static void SetTextLength(DependencyObject obj, int value)
        {
            obj.SetValue(TextLengthProperty, value);
            obj.SetValue(hasTextProperty, value >= 1);
        }

        public bool HasText
        {
            get { return (bool)GetValue(hasTextProperty); }
        }

        static void OnIsMonitoringChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox)
            {
                var txtBox = d as TextBox;

                if ((bool)e.NewValue)
                {
                    txtBox.TextChanged += TextChanged;
                    txtBox.GotFocus += TextBoxGotFocus;
                    txtBox.KeyDown += TextBoxKeyDown;
                }
                else
                {
                    txtBox.TextChanged -= TextChanged;
                    txtBox.GotFocus -= TextBoxGotFocus;
                    txtBox.KeyDown -= TextBoxKeyDown;
                }
            }
            else if (d is PasswordBox)
            {
                var passBox = d as PasswordBox;

                if ((bool)e.NewValue)
                {
                    passBox.PasswordChanged += PasswordChanged;
                    passBox.GotFocus += PasswordGotFocus;
                }
                else
                {
                    passBox.PasswordChanged -= PasswordChanged;
                    passBox.GotFocus -= PasswordGotFocus;
                }
            }
        }

        static void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (txtBox == null)
                return;
            if (e.Key == Key.Enter)
            {
                if (GetMoveFocusOnEnter(txtBox))
                {
                    txtBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
                var command = GetEnterCommand(txtBox);
                if (command != null || GetUpdateSourceOnEnter(txtBox))
                {
                    var binding = txtBox.GetBindingExpression(TextBox.TextProperty);
                    if (binding != null)
                        binding.UpdateSource();
                    if (command != null)
                    {
                        command.Execute(null);
                        e.Handled = true;
                    }
                }
            }
        }

        static void TextChanged(object sender, TextChangedEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (txtBox == null)
                return;
            SetTextLength(txtBox, txtBox.Text.Length);
            if (GetAutoScrollToEnd(txtBox))
                txtBox.ScrollToEnd();
        }

        static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            var passBox = sender as PasswordBox;
            if (passBox == null)
                return;
            SetTextLength(passBox, passBox.Password.Length);
        }

        static void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            var txtBox = sender as TextBox;
            if (txtBox == null)
                return;
            if (GetSelectAllOnFocus(txtBox))
            {
                txtBox.Dispatcher.BeginInvoke((Action)(txtBox.SelectAll));
            }
        }

        static void PasswordGotFocus(object sender, RoutedEventArgs e)
        {
            var passBox = sender as PasswordBox;
            if (passBox == null)
                return;
            if (GetSelectAllOnFocus(passBox))
            {
                passBox.Dispatcher.BeginInvoke((Action)(passBox.SelectAll));
            }
        }

        public static bool GetClearTextButton(DependencyObject d)
        {
            return (bool)d.GetValue(ClearTextButtonProperty);
        }

        public static void SetClearTextButton(DependencyObject obj, bool value)
        {
            obj.SetValue(ClearTextButtonProperty, value);
        }

        private static void ButtonCommandOrClearTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textbox = d as TextBox;
            if (textbox != null)
            {
                // only one loaded event
                textbox.Loaded -= TextBoxLoaded;
                textbox.Loaded += TextBoxLoaded;
            }
            var passbox = d as PasswordBox;
            if (passbox != null)
            {
                // only one loaded event
                passbox.Loaded -= PassBoxLoaded;
                passbox.Loaded += PassBoxLoaded;
            }
        }

        static void PassBoxLoaded(object sender, RoutedEventArgs e)
        {
            var passbox = sender as PasswordBox;
            if (passbox == null || passbox.Style == null)
                return;

            var template = passbox.Template;
            if (template == null)
                return;

            var button = template.FindName("PART_ClearText", passbox) as Button;
            if (button == null)
                return;

            if (GetClearTextButton(passbox))
            {
                // only one event, because loaded event fires more than once, if the textbox is hosted in a tab item
                button.Tag = passbox;
                button.Click -= ButtonClicked;
                button.Click += ButtonClicked;
            }
            else
            {
                button.Click -= ButtonClicked;
            }
        }

        static void TextBoxLoaded(object sender, RoutedEventArgs e)
        {
            var textbox = sender as TextBox;
            if (textbox == null || textbox.Style == null)
                return;

            var template = textbox.Template;
            if (template == null)
                return;

            var button = template.FindName("PART_ClearText", textbox) as Button;
            if (button == null)
                return;

            if (GetClearTextButton(textbox))
            {
                // only one event, because loaded event fires more than once, if the textbox is hosted in a tab item
                button.Tag = textbox;
                button.Click -= ButtonClicked;
                button.Click += ButtonClicked;
            }
            else
            {
                button.Click -= ButtonClicked;
            }
        }

        static void ButtonClicked(object sender, RoutedEventArgs e)
        {
            var button = ((Button)sender);
            var parent = button.Tag as DependencyObject;

            if (GetClearTextButton(parent))
            {
                if (parent is TextBox)
                {
                    ((TextBox)parent).Clear();
                }
                else if (parent is PasswordBox)
                {
                    ((PasswordBox)parent).Clear();
                }
            }
        }
    }

}
