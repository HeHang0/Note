using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Note
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NoteWindow : Window
    {
        private readonly double TITLE_HEIGHT = 0;
        private readonly Action<string> closeAction;
        private readonly Action<string, string> titleChange;
        private readonly Func<Window> createNote;
        private readonly string titleOrigin;

        public NoteWindow(Action<string> ca, Action<string, string> tc, Func<Window> cn, string title)
        {
            closeAction = ca;
            titleChange = tc;
            createNote = cn;
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
                    FoldNote = FoldNote,
                    CreateNote = CreateNote
                }
            };
            DataContext = viewModel;
            InitDockingManager();
        }

        private void InitDockingManager()
        {
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

        private ICommand CreateNote => new DelegateCommand(() =>
        {
            createNote?.Invoke();
        });

        private ICommand DeleteNote => new DelegateCommand(() =>
        {
            MessageBoxResult dr = MessageBox.Show("确定要删除当前便签？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (dr == MessageBoxResult.OK)
            {
                closeAction?.Invoke(Title);
            }
        });

        private ICommand FoldNote => new DelegateCommand(() =>
        {
            if (WindowState == WindowState.Maximized) return;
            DoubleAnimation animation;
            ViewModel viewModel = DataContext as ViewModel;
            if (lastHeight == TITLE_HEIGHT)
            {
                MinHeight = TITLE_HEIGHT;
                lastHeight = Height;
                animation = new DoubleAnimation(Height, TITLE_HEIGHT, TimeSpan.FromSeconds(0.25));
                ResizeMode = ResizeMode.NoResize;
                viewModel.Commands.TitleVisibility = Visibility.Visible;
            }
            else
            {
                animation = new DoubleAnimation(TITLE_HEIGHT, lastHeight, TimeSpan.FromSeconds(0.25));
                animation.Completed += Animation_Completed;
                lastHeight = TITLE_HEIGHT;
                ResizeMode = ResizeMode.CanResizeWithGrip;
                viewModel.Commands.TitleVisibility = Visibility.Hidden;
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
            TextRange textRange = new TextRange(NoteText.Document.ContentStart, NoteText.Document.ContentEnd);
            var firstLine = textRange.Text.Split('\n')[0];
            var newTitle = firstLine.Substring(0, Math.Min(15, firstLine.Length)).Trim();
            if (newTitle.Length == 0)
            {
                newTitle = titleOrigin;
            }
            if (Title == newTitle) return;
            titleChange?.Invoke(Title, newTitle);
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

    public class ToolsCommand: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand DeleteNote { get; set; }
        public ICommand MaxNote { get; set; }
        public ICommand FoldNote { get; set; }
        public ICommand CreateNote { get; set; }

        private Visibility _titleVisibility = Visibility.Hidden;
        public Visibility TitleVisibility
        {
            get
            {
                return _titleVisibility;
            }
            set
            {
                _titleVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("TitleVisibility"));
            }
        }
    }
}
