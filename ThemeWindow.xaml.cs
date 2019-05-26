using System.Windows;

namespace Note
{
    /// <summary>
    /// ThemeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ThemeWindow : Window
    {
        public ThemeWindow()
        {
            InitializeComponent();
            DataContext = ThemeModel.Instance;
        }
    }
}
