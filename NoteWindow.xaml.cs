using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Note
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NoteWindow : Window
    {
        private Action<string> closeAction;
        private Action<string, string> titleChange;

        public NoteWindow(Action<string> ca, Action<string, string> tc)
        {
            closeAction = ca;
            titleChange = tc;
            InitializeComponent();
            DataContext = ThemeModel.Instance;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SystemParameters.StaticPropertyChanged += SystemParameters_StaticPropertyChanged;
            ProcessLastRect();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMousePressInToolBar && !maximized && e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
                e.Handled = true;
            }
        }
        private bool isMousePressInToolBar = false;
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ResizeMode = ResizeMode.NoResize;
            isMousePressInToolBar = true;
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(!maximized && lastHeight == 15) ResizeMode = ResizeMode.CanResizeWithGrip;
            isMousePressInToolBar = false;
        }

        private void DeleteNote(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("确定要删除当前便签？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (dr == MessageBoxResult.OK)
            {
                closeAction(Title);
            }
        }
        private double lastHeight = 15;
        private void FoldNote(object sender, MouseButtonEventArgs e)
        {
            if (maximized) return;
            DoubleAnimation animation;
            if (lastHeight == 15)
            {
                MinHeight = 15;
                lastHeight = Height;
                animation = new DoubleAnimation(Height, 15, TimeSpan.FromSeconds(0.5));
                ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                animation = new DoubleAnimation(15, lastHeight, TimeSpan.FromSeconds(0.5));
                animation.Completed += Animation_Completed;
                lastHeight = 15;
                ResizeMode = ResizeMode.CanResizeWithGrip;
            }
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(0)", new DependencyProperty[] { HeightProperty }));
            var HeightStoryboard = new Storyboard();
            HeightStoryboard.Children.Add(animation);
            HeightStoryboard.Begin(this, true);
        }

        private void Animation_Completed(object sender, EventArgs e)
        {
            MinHeight = 100;
        }

        private Size lastSize;
        private Point lastPoint;
        private void ProcessLastRect()
        {
            lastSize.Height = Height;
            lastSize.Width = Width;
            lastPoint.X = Left;
            lastPoint.Y = Top;
        }
        private void MaxNote(object sender, MouseButtonEventArgs e)
        {
            if (maximized)
            {
                ProcessNormalArea();
                maximized = false;
                ResizeMode = ResizeMode.CanResizeWithGrip;
            }
            else
            {
                ResizeMode = ResizeMode.NoResize;
                maximized = true;
                ProcessWorkArea();
            }
        }

        private bool maximized = false;
        private void SystemParameters_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "WorkArea")
            {
                if (maximized)
                {
                    ProcessWorkArea();
                }
            }
        }

        private void ProcessNormalArea()
        {
            //Left = lastPoint.X;
            //Top = lastPoint.Y;
            //Width = lastSize.Width;
            //Height = lastSize.Height;
            ProcessSizeStoryboard(lastPoint.X, lastPoint.Y, lastSize.Width, lastSize.Height);
        }

        private void ProcessSizeStoryboard(double newLeft, double newTop, double newWidth, double newHeight)
        {
            var animationLeft = new DoubleAnimation(Left, newLeft, TimeSpan.FromSeconds(0.25));
            var animationWidth = new DoubleAnimation(Width, newWidth, TimeSpan.FromSeconds(0.25));
            afterTop = newTop;
            afterHeight = newHeight;
            animationWidth.Completed += AnimationWidth_Completed;

            BeginAnimation(LeftProperty, animationLeft, HandoffBehavior.Compose);
            BeginAnimation(WidthProperty, animationWidth, HandoffBehavior.Compose);
        }
        private double afterTop;
        private double afterHeight;

        private void AnimationWidth_Completed(object sender, EventArgs e)
        {
            var animationTop = new DoubleAnimation(Top, afterTop, TimeSpan.FromSeconds(0.25));
            var animationHeight = new DoubleAnimation(Height, afterHeight, TimeSpan.FromSeconds(0.25));
            BeginAnimation(TopProperty, animationTop, HandoffBehavior.Compose);
            BeginAnimation(HeightProperty, animationHeight, HandoffBehavior.Compose);
        }

        private void ProcessWorkArea()
        {
            //Left = SystemParameters.WorkArea.Left;
            //Top = SystemParameters.WorkArea.Top;
            //Width = SystemParameters.WorkArea.Width;
            //Height = SystemParameters.WorkArea.Height;
            ProcessSizeStoryboard(SystemParameters.WorkArea.Left, SystemParameters.WorkArea.Top, SystemParameters.WorkArea.Width, SystemParameters.WorkArea.Height);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!maximized)
            {
                ProcessLastRect();
            }
        }

        private void NoteText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            var newTitle = tb.Text.Substring(0, Math.Min(15, tb.Text.Length)).Trim();
            if(newTitle.Length > 0)
            {
                if (Title == newTitle) return;
                titleChange(Title, newTitle);
            }
        }
    }
}
