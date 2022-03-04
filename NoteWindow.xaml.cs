using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
                    CreateNote = CreateNote,
                    TitleMouseUp = TitleMouseUp
                }
            };
            DataContext = viewModel;
            InitDockingManager();
        }

        private void InitDockingManager()
        {
            DockingManager.Update(this);

            //第一个对象为触发变化的对象
            DependencyPropertyDescriptor.FromProperty(LeftProperty, typeof(Window)).AddValueChanged(this, OnWindowPosChange);
            //第一个对象为触发变化的对象
            DependencyPropertyDescriptor.FromProperty(TopProperty, typeof(Window)).AddValueChanged(this, OnWindowPosChange);
        }

        private void OnWindowPosChange(object sender, EventArgs e)
        {
            DockingManager.Update(this);
            cancelToken?.Cancel();
            cancelToken = new CancellationTokenSource();
            TaskDelay(200, cancelToken.Token, () => {
                DockingManager.AutoDock(this);
            });
        }

        private CancellationTokenSource cancelToken = null;
        async void TaskDelay(int timeout, CancellationToken token, Action method)
        {
            try
            {
                await Task.Delay(timeout, token);
                method?.Invoke();
            }
            catch(Exception)
            {
            }
        }

        private ICommand CreateNote => new DelegateCommand(() =>
        {
            createNote?.Invoke();
        });

        private DateTime lastTitleMouseUpTime = new DateTime();
        private ICommand TitleMouseUp => new DelegateCommand(() =>
        {
            var now = DateTime.Now;
            if ((now - lastTitleMouseUpTime).TotalMilliseconds < 500)
            {
                FoldNote.Execute(null);
                lastTitleMouseUpTime = new DateTime();
            }else
            {
                lastTitleMouseUpTime = now;
            }
        });

        private ICommand DeleteNote => new DelegateCommand(() =>
        {
            MessageBoxResult dr = MessageBox.Show("确定要删除当前便条？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (dr == MessageBoxResult.OK)
            {
                closeAction?.Invoke(Title);
            }
        });

        Point lastFoldPoint = new Point();
        private bool isFold = false;
        private ICommand FoldNote => new DelegateCommand(() =>
        {
            lastFoldPoint.X = Left;
            lastFoldPoint.Y = Top;
            if (WindowState == WindowState.Maximized) return;
            DoubleAnimation animation;
            ViewModel viewModel = DataContext as ViewModel;
            isFold = lastHeight == TITLE_HEIGHT;
            if (lastHeight == TITLE_HEIGHT)
            {
                MinHeight = TITLE_HEIGHT;
                lastHeight = Height;
                animation = new DoubleAnimation(Height, TITLE_HEIGHT, TimeSpan.FromSeconds(0.25));
                ResizeMode = ResizeMode.NoResize;
                viewModel.Commands.MaxNoteVisibility = Visibility.Hidden;
                viewModel.Commands.TitleVisibility = Visibility.Visible;
            }
            else
            {
                animation = new DoubleAnimation(TITLE_HEIGHT, lastHeight, TimeSpan.FromSeconds(0.25));
                lastHeight = TITLE_HEIGHT;
                ResizeMode = ResizeMode.CanResizeWithGrip;
                viewModel.Commands.MaxNoteVisibility = Visibility.Visible;
                viewModel.Commands.TitleVisibility = Visibility.Hidden;
            }
            animation.Completed += Animation_Completed;
            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(0)", new DependencyProperty[] { HeightProperty }));
            var HeightStoryboard = new Storyboard();
            HeightStoryboard.Children.Add(animation);
            HeightStoryboard.Begin(this);
        });
        private double lastHeight = 0;

        private void Animation_Completed(object sender, EventArgs e)
        {
            if(lastHeight == TITLE_HEIGHT) MinHeight = 100;
            Left = lastFoldPoint.X;
            Top = lastFoldPoint.Y;
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

        private void NoteText_TextChanged(object sender, RoutedEventArgs e)
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

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            System.Windows.Interop.HwndSource source = System.Windows.Interop.HwndSource.FromHwnd(new System.Windows.Interop.WindowInteropHelper(this).Handle);
            source.AddHook(new System.Windows.Interop.HwndSourceHook(WndProc));
        }

        int WM_NCLBUTTONDBLCLK { get { return 0x00A3; } }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_NCLBUTTONDBLCLK)
            {
                handled = true;  //prevent double click from maximizing the window.
                FoldNote.Execute(null);
            }

            return IntPtr.Zero;
        }

        private void MenuBold_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItalic_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonTools_MouseEnter(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.Background = new SolidColorBrush(Color.FromArgb(0x30, 0, 0, 0));
        }

        private void ButtonTools_MouseLeave(object sender, MouseEventArgs e)
        {
            Border border = sender as Border;
            border.Background = Brushes.Transparent;
        }

        private void ButtonToolsPannel_MouseEnter(object sender, MouseEventArgs e)
        {
            if (isFold) return;
            ToolsPannel.Opacity = 1;
            ToolsPannelBorder.Opacity = 1;
        }

        private void ButtonToolsPannel_MouseLeave(object sender, MouseEventArgs e)
        {
            ToolsPannelBorder.Opacity = 0;
            ToolsPannel.Opacity = 0;
        }

        private void ButtonBold_Click(object sender, MouseButtonEventArgs e)
        {
            TextRange selRange = new TextRange(NoteText.Selection.Start, NoteText.Selection.End);
            selRange.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
        }

        private void ButtonItalic_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void ButtonUnderLine_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void ButtonStrikeThrough_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void ButtonOrderedList_Click(object sender, MouseButtonEventArgs e)
        {

        }

        private void ButtonUnOrderedList_Click(object sender, MouseButtonEventArgs e)
        {

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
        public ICommand TitleMouseUp { get; set; }

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

        private Visibility _maxNoteVisibility = Visibility.Visible;
        public Visibility MaxNoteVisibility
        {
            get
            {
                return _maxNoteVisibility;
            }
            set
            {
                _maxNoteVisibility = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxNoteVisibility"));
            }
        }
    }
}
