using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace Note
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            InitNotifyIcon();
            CheckRoamingFolder();
            CheckSetting();
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        NotifyIcon notifyIcon;

        private void InitNotifyIcon()
        {
            notifyIcon = new NotifyIcon();
            notifyIcon.Text = "便签";//最小化到托盘时，鼠标点击时显示的文本
            notifyIcon.Icon = Note.Properties.Resources.logo;//程序图标
            notifyIcon.Visible = true;
            var newItem = new MenuItem("新建便签");
            newItem.Click += NewNoteClient;
            var closeItem = new MenuItem("退出");
            closeItem.Click += ExitApp;
            notesItem = new MenuItem("显示");
            var settingItem = new MenuItem("设置");
            settingItem.Click += ShowSetting;
            var aboutItem = new MenuItem("关于");
            aboutItem.Click += ShowAbout;
            var menu = new MenuItem[] { newItem, notesItem, settingItem, aboutItem, closeItem };
            notifyIcon.ContextMenu = new ContextMenu(menu);
        }

        private ThemeWindow tw;
        private void ShowSetting(object sender, EventArgs e)
        {
            if (tw == null)
            {
                tw = new ThemeWindow();
                tw.Closed += Tw_Closed;
            }
             tw.Show();
            tw.Activate();
            if(tw.WindowState == WindowState.Minimized)
            {
                tw.WindowState = WindowState.Normal;
            }
        }

        private void Tw_Closed(object sender, EventArgs e)
        {
            tw.Closed -= Tw_Closed;
            tw = null;
        }


        private AboutWindow aw;
        private void ShowAbout(object sender, EventArgs e)
        {
            if (aw == null)
            {
                aw = new AboutWindow();
                aw.Closed += Aw_Closed;
            }
            aw.Show();
            aw.Activate();
            if(aw.WindowState == WindowState.Minimized)
            {
                aw.WindowState = WindowState.Normal;
            }
        }

        private void Aw_Closed(object sender, EventArgs e)
        {
            aw.Closed -= Aw_Closed;
            aw = null;
        }

        private Dictionary<string, NoteWindow> Notes = new Dictionary<string, NoteWindow>();

        public MenuItem notesItem { get; private set; }

        private void NewNoteClient(object sender, EventArgs e)
        {
            NewNote();
        }

        private NoteWindow NewNote()
        {
            string title = getTitle("新建便笺");
            var note = new NoteWindow(WindowClose, TitleChange, title);
            Notes.Add(title, note);
            var mi = new MenuItem(title);
            mi.Name = title;
            mi.Click += ShowNote;
            notesItem.MenuItems.Add(mi);
            note.Show();
            return note;
        }

        private Action<string> WindowClose => (winTitle) =>
        {
            Notes[winTitle]?.Close();
            Notes.Remove(winTitle);
            notesItem.MenuItems.RemoveByKey(winTitle);
        };
        private static readonly object titleChangeLock = new object();
        private Action<string, string> TitleChange => (winTitle, newTitle) =>
        {
            lock (titleChangeLock)
            {
                string title = getTitle(newTitle);
                Notes[title] = Notes[winTitle];
                Notes.Remove(winTitle);
                Notes[title].Title = title;
                var items = notesItem.MenuItems.Find(winTitle, false);
                if (items.Length > 0)
                {
                    items[0].Name = title;
                    items[0].Text = title;
                }
            }
        };

        private void ShowNote(object sender, EventArgs e)
        {
            var mi = sender as MenuItem;
            var note = Notes[mi.Text];
            note.Show();
            note.Activate();
            if(note.WindowState == WindowState.Minimized)
            {
                note.WindowState = WindowState.Normal;
            }
        }

        private string getTitle(string title)
        {
            int index = 1;
            string newTitle = title;
            while (Notes.ContainsKey(newTitle))
            {
                newTitle = $"{title} {index++}";
            }
            return newTitle;
        }

        private void ExitApp(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
            SaveToRoaming();
            Current.Shutdown();
        }

        private void CheckSetting()
        {
            try
            {
                var settingStr = File.ReadAllText(Path.Combine(noteData, "setting.set"));
                var settings = settingStr.Split('+');
                if(settings.Length == 6)
                {
                    var theme = ThemeModel.Instance;

                    theme.BackGroundColorItem = theme.BackGroundColorList.SingleOrDefault(m => m.Name == settings[0]);

                    theme.FontFamilyItem = theme.FontFamilyList.SingleOrDefault(m => m.Name == settings[1]);

                    theme.FontSizeItem = theme.FontSizeList.SingleOrDefault(m => m.Name == settings[2] + " px");

                    theme.LineHeightItem = theme.FontSizeList.SingleOrDefault(m => m.Name == settings[3] + " px");

                    theme.Blod = settings[4].ToUpper() == "TRUE";
                    theme.Italic = settings[5].ToUpper() == "TRUE";
                }
            }
            catch (Exception)
            {
            }
        }

        private void SaveToRoaming()
        {
            var noteData = CheckRoamingFolder(true);
            foreach (var file in Directory.GetFiles(Path.Combine(noteData, "Note")))
            {
                FileInfo fi = new FileInfo(file);
                try
                {
                    fi.Delete();
                }
                catch (Exception)
                {
                }
            }
            int index = 1;
            foreach (var item in Notes)
            {
                File.WriteAllText(Path.Combine(noteData, "Note", $"{index++}.note"), item.Value.NoteText.Text);
            }
            var theme = ThemeModel.Instance;
            string settingStr = $"{theme.BackGroundColorItem.Name}+{theme.FontFamilyItem.Name}+{theme.FontSizeItem.Value}+{theme.LineHeightItem.Value}+{theme.Blod}+{theme.Italic}";
            File.WriteAllText(Path.Combine(noteData, "setting.set"), settingStr);
        }
        private string noteData;
        private string CheckRoamingFolder(bool onlyCheck = false)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            noteData = Path.Combine(appData, "Note_oo__H__oo");
            var noteHistory = Path.Combine(noteData, "Note");
            if (!Directory.Exists(noteData))
            {
                Directory.CreateDirectory(noteData);
            }
            if (!Directory.Exists(noteHistory))
            {
                Directory.CreateDirectory(noteHistory);
            }
            else if(!onlyCheck)
            {
                FileInfo[] files = new DirectoryInfo(noteHistory).GetFiles();
                foreach (var file in files)
                {
                    if (file.FullName.EndsWith(".note"))
                    {
                        var text = File.ReadAllText(file.FullName);
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            NewNote().NoteText.Text = text;
                        }
                    }
                }
            }
            return noteData;
        }
    }
}
