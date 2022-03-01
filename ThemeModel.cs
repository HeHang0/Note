using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace Note
{
    public class ThemeModel : INotifyPropertyChanged
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

        public SolidColorBrush BackGroundColor => (SolidColorBrush)BackGroundColorItem.Value;

        private DisplayItem backGroundColorItem = null;
        public DisplayItem BackGroundColorItem
        {
            get
            {
                if(backGroundColorItem == null)
                {
                    backGroundColorItem = BackGroundColorList.FirstOrDefault();
                }
                return backGroundColorItem;
            }
            set
            {
                backGroundColorItem = value;
                OnPropertyChanged("BackGroundColorItem");
                OnPropertyChanged("BackGroundColor");
            }
        }

        private List<DisplayItem> backGroundColorList = new List<DisplayItem>()
        {
            DisplayItem.GetDisplay("黄色", new SolidColorBrush(Color.FromRgb(0xFC, 0xE7, 0x3C)), 15),
            DisplayItem.GetDisplay("蓝色", new SolidColorBrush(Color.FromRgb(0x87, 0xEC, 0xFF)), 15),
            DisplayItem.GetDisplay("绿色", new SolidColorBrush(Color.FromRgb(0x82, 0xF9, 0x83)), 15),
            DisplayItem.GetDisplay("粉红色", new SolidColorBrush(Color.FromRgb(0xF7, 0xAC, 0xA9)), 15),
            DisplayItem.GetDisplay("紫色", new SolidColorBrush(Color.FromRgb(0x95, 0xAE, 0xFE)), 15),
            DisplayItem.GetDisplay("灰色", new SolidColorBrush(Color.FromRgb(0xD7, 0xD7, 0xD7)), 15)
        };
        public List<DisplayItem> BackGroundColorList => backGroundColorList;

        public int LineHeight => (int)LineHeightItem.Value;
        private DisplayItem lineHeightItem;
        public DisplayItem LineHeightItem
        {
            get
            {
                if (lineHeightItem == null)
                {
                    lineHeightItem = FontSizeList[11];
                }
                return lineHeightItem;
            }
            set
            {
                lineHeightItem = value;
                OnPropertyChanged("LineHeightItem");
                OnPropertyChanged("LineHeight");
            }
        }
        
        public int FontSize => (int)FontSizeItem.Value;

        private DisplayItem fontSizeItem = null;
        public DisplayItem FontSizeItem
        {
            get
            {
                if (fontSizeItem == null)
                {
                    fontSizeItem = FontSizeList[6];
                }
                return fontSizeItem;
            }
            set
            {
                fontSizeItem = value;
                OnPropertyChanged("FontSizeItem");
                OnPropertyChanged("FontSize");
            }
        }

        public List<DisplayItem> FontSizeList => fontSizeList;
        private List<DisplayItem> fontSizeList = new List<DisplayItem>()
        {
            DisplayItem.GetDisplay("6 px", 6),
            DisplayItem.GetDisplay("7 px", 7),
            DisplayItem.GetDisplay("8 px", 8),
            DisplayItem.GetDisplay("9 px", 9),
            DisplayItem.GetDisplay("10 px", 10),
            DisplayItem.GetDisplay("11 px", 11),
            DisplayItem.GetDisplay("12 px", 12),
            DisplayItem.GetDisplay("14 px", 14),
            DisplayItem.GetDisplay("16 px", 16),
            DisplayItem.GetDisplay("18 px", 18),
            DisplayItem.GetDisplay("20 px", 20),
            DisplayItem.GetDisplay("22 px", 22),
            DisplayItem.GetDisplay("24 px", 24),
            DisplayItem.GetDisplay("36 px", 36),
            DisplayItem.GetDisplay("48 px", 48),
            DisplayItem.GetDisplay("72 px", 72),
        };

        public FontFamily FontFamily => (FontFamily)FontFamilyItem.Value;

        private DisplayItem fontFamilyItem = null;
        public DisplayItem FontFamilyItem
        {
            get
            {
                if (fontFamilyItem == null)
                {
                    fontFamilyItem = FontFamilyList.Where(m => m.Name == "Microsoft YaHei UI").FirstOrDefault();
                }
                return fontFamilyItem;
            }
            set
            {
                fontFamilyItem = value;
                OnPropertyChanged("FontFamilyItem");
                OnPropertyChanged("FontFamily");
            }
        }

        private List<DisplayItem> fontFamilyList = Fonts.SystemFontFamilies.Select(m => DisplayItem.GetDisplay(m.ToString(), m)).ToList();
        public List<DisplayItem> FontFamilyList => fontFamilyList;

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

        private bool italic = false;
        public bool Italic
        {
            get
            {
                return italic;
            }
            set
            {
                italic = value;
                OnPropertyChanged("Italic");
                if (italic)
                {
                    FontStyle = FontStyles.Italic;
                }
                else
                {
                    FontStyle = FontStyles.Normal;
                }
            }
        }

        public FontWeight fontWeight = FontWeights.Normal;
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

        private bool blod = false;
        public bool Blod
        {
            get
            {
                return blod;
            }
            set
            {
                blod = value;
                OnPropertyChanged("Blod");
                if (blod)
                {
                    FontWeight = FontWeights.Bold;
                }
                else
                {
                    FontWeight = FontWeights.Normal;
                }
            }
        }

        public class DisplayItem
        {
            public string Name { get; private set; }
            public int Width { get; private set; }
            public object Value { get; private set; }

            public static DisplayItem GetDisplay(string name, object value, int width = 0)
            {
                return new DisplayItem() { Name = name, Value = value, Width = width };
            }
        }
    }
}
