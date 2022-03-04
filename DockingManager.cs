using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Note
{
    public class DockingManager
    {
        private static readonly int LIMITSIZE = 15;
        public static Dictionary<Window, double> LeftBorder { get; }
        public static Dictionary<Window, double> TopBorder { get; }
        public static Dictionary<Window, double> RightBorder { get; }
        public static Dictionary<Window, double> BottomBorder { get; }

        static DockingManager()
        {
            LeftBorder = new Dictionary<Window, double>();
            TopBorder = new Dictionary<Window, double>();
            RightBorder = new Dictionary<Window, double>();
            BottomBorder = new Dictionary<Window, double>();
        }

        public static void Update(Window w)
        {
            LeftBorder[w] = w.Left;
            TopBorder[w] = w.Top;
            RightBorder[w] = w.Left + w.Width;
            BottomBorder[w] = w.Top + w.Height;
        }

        public static void Delete(Window w)
        {
            LeftBorder.Remove(w);
            TopBorder.Remove(w);
            RightBorder.Remove(w);
            BottomBorder.Remove(w);
        }

        public static void AutoDock(Window w)
        {
            double t = w.Top, b = w.Top + w.Height, l = w.Left, r = w.Left + w.Width;
            var temp = HorizontalSet(w);
            foreach (var d in temp)
            {
                if (Math.Abs(t - d) <= LIMITSIZE)
                {
                    w.Top = d;
                    break;
                }
                if (Math.Abs(b - d) <= LIMITSIZE)
                {
                    w.Top = d - w.Height;
                    break;
                }
            }
            temp = VerticalSet(w);
            foreach (var d in temp)
            {
                if (Math.Abs(l - d) <= LIMITSIZE)
                {
                    w.Left = d;
                    break;
                }
                if (Math.Abs(r - d) <= LIMITSIZE)
                {
                    w.Left = d - w.Width;
                    break;
                }
            }
            Update(w);
        }

        private static HashSet<double> HorizontalSet(Window w)
        {
            HashSet<double> s = new HashSet<double>();
            foreach (var v in TopBorder.Keys)
            {
                if (v != w)
                {
                    s.Add(TopBorder[v]);
                }
            }
            foreach (var v in BottomBorder.Keys)
            {
                if (v != w)
                {
                    s.Add(BottomBorder[v]);
                }
            }
            return s;
        }

        private static HashSet<double> VerticalSet(Window w)
        {
            HashSet<double> s = new HashSet<double>();
            foreach (var v in LeftBorder.Keys)
            {
                if (v != w)
                {
                    s.Add(LeftBorder[v]);
                }
            }
            foreach (var v in RightBorder.Keys)
            {
                if (v != w)
                {
                    s.Add(RightBorder[v]);
                }
            }
            return s;
        }
    }
}
