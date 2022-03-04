using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Note
{
    public class UniversalWindowStyle
    {
        public static readonly DependencyProperty TitleBarProperty = DependencyProperty.RegisterAttached(
            "TitleBar", typeof(UniversalTitleBar), typeof(UniversalWindowStyle),
            new PropertyMetadata(new UniversalTitleBar(), OnTitleBarChanged));

        public static UniversalTitleBar GetTitleBar(DependencyObject element)
            => (UniversalTitleBar)element.GetValue(TitleBarProperty);

        public static void SetTitleBar(DependencyObject element, UniversalTitleBar value)
            => element.SetValue(TitleBarProperty, value);

        public static readonly DependencyProperty TitleVisibilityProperty = DependencyProperty.RegisterAttached(
            "TitleVisibility", typeof(Visibility), typeof(UniversalWindowStyle),
            new PropertyMetadata(Visibility.Visible));

        public static Visibility GetTitleVisibility(DependencyObject element) => (Visibility)element.GetValue(TitleVisibilityProperty);

        public static void SetTitleVisibility(DependencyObject element, Visibility value)
            => element.SetValue(TitleVisibilityProperty, value);

        public static readonly DependencyProperty MaxNoteVisibilityProperty = DependencyProperty.RegisterAttached(
            "MaxNoteVisibility", typeof(Visibility), typeof(UniversalWindowStyle),
            new PropertyMetadata(Visibility.Visible));

        public static Visibility GetMaxNoteVisibility(DependencyObject element) => (Visibility)element.GetValue(MaxNoteVisibilityProperty);

        public static void SetMaxNoteVisibility(DependencyObject element, Visibility value)
            => element.SetValue(MaxNoteVisibilityProperty, value);

        public static readonly DependencyProperty DeleteNoteCommondProperty = DependencyProperty.RegisterAttached(
            "DeleteNoteCommond", typeof(ICommand), typeof(UniversalWindowStyle),
            new PropertyMetadata(null, OnDeleteNoteCommondChanged));

        public static ICommand GetDeleteNoteCommond(DependencyObject element) => (ICommand)element.GetValue(DeleteNoteCommondProperty);

        public static void SetDeleteNoteCommond(DependencyObject element, ICommand value)
            => element.SetValue(DeleteNoteCommondProperty, value);

        public static readonly DependencyProperty MaxNoteCommondProperty = DependencyProperty.RegisterAttached(
            "MaxNoteCommond", typeof(ICommand), typeof(UniversalWindowStyle),
            new PropertyMetadata(null, OnMaxNoteCommondChanged));

        public static ICommand GetMaxNoteCommond(DependencyObject element) => (ICommand)element.GetValue(MaxNoteCommondProperty);

        public static void SetMaxNoteCommond(DependencyObject element, ICommand value)
            => element.SetValue(MaxNoteCommondProperty, value);

        public static readonly DependencyProperty TitleMouseUpCommondProperty = DependencyProperty.RegisterAttached(
            "TitleMouseUpCommond", typeof(ICommand), typeof(UniversalWindowStyle),
            new PropertyMetadata(null, OnTitleMouseUpCommondChanged));

        public static ICommand GetTitleMouseUpCommond(DependencyObject element) => (ICommand)element.GetValue(TitleMouseUpCommondProperty);

        public static void SetTitleMouseUpCommond(DependencyObject element, ICommand value)
            => element.SetValue(TitleMouseUpCommondProperty, value);

        public static readonly DependencyProperty FoldNoteCommondProperty = DependencyProperty.RegisterAttached(
            "FoldNoteCommond", typeof(ICommand), typeof(UniversalWindowStyle),
            new PropertyMetadata(null, OnFoldNoteCommondChanged));

        public static ICommand GetFoldNoteCommond(DependencyObject element) => (ICommand)element.GetValue(FoldNoteCommondProperty);

        public static void SetFoldNoteCommond(DependencyObject element, ICommand value)
            => element.SetValue(FoldNoteCommondProperty, value);

        public static readonly DependencyProperty ForegroundProperty = DependencyProperty.RegisterAttached(
            "Foreground", typeof(Brush), typeof(UniversalWindowStyle),
            new PropertyMetadata(Brushes.Black));

        public static Brush GetForeground(DependencyObject element) => (Brush)element.GetValue(ForegroundProperty);

        public static void SetForeground(DependencyObject element, Brush value)
            => element.SetValue(ForegroundProperty, value);

        public static readonly DependencyProperty TitleBarButtonStateProperty = DependencyProperty.RegisterAttached(
            "TitleBarButtonState", typeof(WindowState?), typeof(UniversalWindowStyle),
            new PropertyMetadata(null, OnButtonStateChanged));

        public static WindowState? GetTitleBarButtonState(DependencyObject element)
            => (WindowState?)element.GetValue(TitleBarButtonStateProperty);

        public static void SetTitleBarButtonState(DependencyObject element, WindowState? value)
            => element.SetValue(TitleBarButtonStateProperty, value);

        public static readonly DependencyProperty IsTitleBarCloseButtonProperty = DependencyProperty.RegisterAttached(
            "IsTitleBarCloseButton", typeof(bool), typeof(UniversalWindowStyle),
            new PropertyMetadata(false, OnIsCloseButtonChanged));

        public static bool GetIsTitleBarCloseButton(DependencyObject element)
            => (bool)element.GetValue(IsTitleBarCloseButtonProperty);

        public static void SetIsTitleBarCloseButton(DependencyObject element, bool value)
            => element.SetValue(IsTitleBarCloseButtonProperty, value);

        private static void OnTitleBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is null) throw new NotSupportedException("TitleBar property should not be null.");
        }

        private static void OnButtonStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (Button)d;

            if (e.OldValue is WindowState)
            {
                button.Click -= StateButton_Click;
            }

            if (e.NewValue is WindowState)
            {
                button.Click -= StateButton_Click;
                button.Click += StateButton_Click;
            }
        }

        private static void OnDeleteNoteCommondChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void OnMaxNoteCommondChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void OnTitleMouseUpCommondChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void OnFoldNoteCommondChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void OnIsCloseButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (Button)d;

            if (e.OldValue is true)
            {
                button.Click -= CloseButton_Click;
            }

            if (e.NewValue is true)
            {
                button.Click -= CloseButton_Click;
                button.Click += CloseButton_Click;
            }
        }

        private static void StateButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (DependencyObject)sender;
            var window = Window.GetWindow(button);
            var state = GetTitleBarButtonState(button);
            if (window != null && state != null)
            {
                window.WindowState = state.Value;
            }
        }

        private static void CloseButton_Click(object sender, RoutedEventArgs e)
            => Window.GetWindow((DependencyObject)sender)?.Close();
    }

    public class UniversalTitleBar
    {
        public Color ForegroundColor { get; set; } = Colors.Black;
        public Color InactiveForegroundColor { get; set; } = Color.FromRgb(0x99, 0x99, 0x99);
        public Color ButtonHoverForeground { get; set; } = Colors.Black;
        public Color ButtonHoverBackground { get; set; } = Colors.Transparent;// Color.FromArgb(0x80, 0x82, 0x82, 0x82);
        public Color ButtonPressedForeground { get; set; } = Colors.Black;
        public Color ButtonPressedBackground { get; set; } = Colors.Transparent;// Color.FromArgb(0xB2, 0x82, 0x82, 0x82);
        public ICommand Open => new DelegateCommand(() => { });
    }

    public class DelegateCommand : ICommand
    {
        private readonly Func<bool> canExecute = () => true;
        private readonly Action executeMethod = null;
        public DelegateCommand(Action executeMethod)
        {
            this.executeMethod = executeMethod;
        }
        public DelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
        {
            this.canExecute = canExecuteMethod;
            this.executeMethod = executeMethod;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter = null)
        {
            return canExecute?.Invoke() ?? true;
        }

        public void Execute(object parameter = null)
        {
            executeMethod?.Invoke();
        }
    }
}
