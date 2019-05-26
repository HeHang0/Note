using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Note
{
    class ThemeModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        static ThemeModel()
        {
            Instance = new ThemeModel();
        }

        private ThemeModel()
        {
        }

        public static ThemeModel Instance { get; private set; } = null;

        private SolidColorBrush backGroundColor = new SolidColorBrush(Color.FromRgb(0xFC, 0xE7, 0x3C));
        public SolidColorBrush BackGroundColor
        {
            get
            {
                return backGroundColor;
            }
            set
            {
                backGroundColor = value;
                OnPropertyChanged("BackGroundColor");
            }
        }

        public List<BackGroundColorDisplay> BackGroundColorList => new List<BackGroundColorDisplay>()
        {
            BackGroundColorDisplay.GetDisplay("黄色", new SolidColorBrush(Color.FromRgb(0xFC, 0xE7, 0x3C))),
            BackGroundColorDisplay.GetDisplay("蓝色", new SolidColorBrush(Color.FromRgb(0x87, 0xEC, 0xFF))),
            BackGroundColorDisplay.GetDisplay("绿色", new SolidColorBrush(Color.FromRgb(0x82, 0xF9, 0x83))),
            BackGroundColorDisplay.GetDisplay("粉红色", new SolidColorBrush(Color.FromRgb(0xF7, 0xAC, 0xA9))),
            BackGroundColorDisplay.GetDisplay("紫色", new SolidColorBrush(Color.FromRgb(0x95, 0xAE, 0xFE))),
            BackGroundColorDisplay.GetDisplay("灰色", new SolidColorBrush(Color.FromRgb(0xD7, 0xD7, 0xD7)))
        };

        private double lineHeight = 22;
        public double LineHeight
        {
            get
            {
                return lineHeight;
            }
            set
            {
                lineHeight = value;
                OnPropertyChanged("LineHeight");
            }
        }

        private double fontSize = 12;
        public double FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                fontSize = value;
                OnPropertyChanged("FontSize");
            }
        }

        //Fonts.SystemFontFamilies
        private FontFamily fontFamily = new FontFamily("Microsoft YaHei UI");
        public FontFamily FontFamily
        {
            get
            {
                return fontFamily;
            }
            set
            {
                fontFamily = value;
                OnPropertyChanged("FontFamily");
            }
        }

        private FontStyle fontStyle = FontStyles.Normal;
        public FontStyle FontStyle
        {
            get
            {
                return fontStyle;
            }
            set
            {
                fontStyle = value;
                OnPropertyChanged("FontStyle");
            }
        }

        private FontWeight fontWeight = FontWeights.Normal;
        public FontWeight FontWeight
        {
            get
            {
                return fontWeight;
            }
            set
            {
                fontWeight = value;
                OnPropertyChanged("FontWeight");
            }
        }

        public class BackGroundColorDisplay
        {
            public string Name { get; set; }
            public SolidColorBrush BackGroundColor { get; set; }

            public static BackGroundColorDisplay GetDisplay(string name, SolidColorBrush bgc)
            {
                return new BackGroundColorDisplay() { Name = name, BackGroundColor = bgc };
            }
        }
    }
}
