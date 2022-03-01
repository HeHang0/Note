using Prism.Commands;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly double TITLE_HEIGHT = 0;
        private Action<string> closeAction;
        private Action<string, string> titleChange;
        private readonly string titleOrigin;

        public NoteWindow(Action<string> ca, Action<string, string> tc, string title)
        {
            closeAction = ca;
            titleChange = tc;
            titleOrigin = title;
            InitializeComponent();
            Title = title;
            ViewModel viewModel = new ViewModel()
            {
                Settings = ThemeModel.Instance,
                Commands = new ToolsCommand()
                {
                    DeleteNote = DeleteNote,
                    MaxNote = MaxNote,
                    FoldNote = FoldNote
                }
            };
            DataContext = viewModel;
            DockingManager.Update(this);

            //第一个对象为触发变化的对象
            DependencyPropertyDescriptor.FromProperty(LeftProperty, typeof(Window)).AddValueChanged(this, (o, handle) =>
            {
                DockingManager.AutoDock(this);
                DockingManager.Update(this);
            });
            //第一个对象为触发变化的对象
            DependencyPropertyDescriptor.FromProperty(TopProperty, typeof(Window)).AddValueChanged(this, (o, handle) =>
            {
                DockingManager.AutoDock(this);
                DockingManager.Update(this);
            });
            //第一个对象为触发变化的对象
            DependencyPropertyDescriptor.FromProperty(HeightProperty, typeof(Window)).AddValueChanged(this, (o, handle) =>
            {
                DockingManager.AutoDock(this);
                DockingManager.Update(this);
            });
            //第一个对象为触发变化的对象
            DependencyPropertyDescriptor.FromProperty(WidthProperty, typeof(Window)).AddValueChanged(this, (o, handle) =>
            {
                DockingManager.AutoDock(this);
                DockingManager.Update(this);
            });
        }

        private ICommand DeleteNote => new DelegateCommand(() =>
        {
            MessageBoxResult dr = MessageBox.Show("确定要删除当前便签？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (dr == MessageBoxResult.OK)
            {
                closeAction(Title);
            }
        });

        private ICommand FoldNote => new DelegateCommand(() =>
        {
            if (WindowState == WindowState.Maximized) return;
            DoubleAnimation animation;
            if (lastHeight == TITLE_HEIGHT)
            {
                MinHeight = TITLE_HEIGHT;
                lastHeight = Height;
                animation = new DoubleAnimation(Height, TITLE_HEIGHT, TimeSpan.FromSeconds(0.25));
                ResizeMode = ResizeMode.NoResize;
            }
            else
            {
                animation = new DoubleAnimation(TITLE_HEIGHT, lastHeight, TimeSpan.FromSeconds(0.25));
                animation.Completed += Animation_Completed;
                lastHeight = TITLE_HEIGHT;
                ResizeMode = ResizeMode.CanResizeWithGrip;
            }
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(0)", new DependencyProperty[] { HeightProperty }));
            var HeightStoryboard = new Storyboard();
            HeightStoryboard.Children.Add(animation);
            HeightStoryboard.Begin(this);
        });
        private double lastHeight = 0;

        private void Animation_Completed(object sender, EventArgs e)
        {
            MinHeight = 100;
        }

        private ICommand MaxNote => new DelegateCommand(() =>
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        });

        private void NoteText_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            var newTitle = tb.Text.Substring(0, Math.Min(15, tb.Text.Length)).Trim();
            if (newTitle.Length == 0)
            {
                newTitle = titleOrigin;
            }
            if (Title == newTitle) return;
            titleChange(Title, newTitle);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            DockingManager.Delete(this);
        }
    }


    public class ViewModel
    {
        public ThemeModel Settings { get; set; }
        public ToolsCommand Commands { get; set; }
    }

    public class ToolsCommand
    {
        public ICommand DeleteNote { get; set; }
        public ICommand MaxNote { get; set; }
        public ICommand FoldNote { get; set; }
    }
}
