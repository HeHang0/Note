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
            var aboutItem = new MenuItem("关于");
            aboutItem.Click += ShowAbout;
            var menu = new MenuItem[] { newItem, notesItem, aboutItem, closeItem };
            notifyIcon.ContextMenu = new ContextMenu(menu);
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
            var note = new NoteWindow(WindowClose, TitleChange);
            string title = getTitle("新建便笺");
            note.Title = title;
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
            while (Notes.ContainsKey(title))
            {
                title = $"{title} {index++}";
            }
            return title;
        }

        private void ExitApp(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
            SaveToRoaming();
            Current.Shutdown();
        }

        private void SaveToRoaming()
        {
            var noteData = CheckRoamingFolder(true);
            foreach (var file in Directory.GetFiles(noteData))
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
                File.WriteAllText(Path.Combine(noteData, $"{index++}.note"), item.Value.NoteText.Text);
            }
        }

        private string CheckRoamingFolder(bool onlyCheck = false)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var noteData = Path.Combine(appData, "Note_oo__H__oo");
            if (!Directory.Exists(noteData))
            {
                Directory.CreateDirectory(noteData);
            }
            else if(!onlyCheck)
            {
                FileInfo[] files = new DirectoryInfo(noteData).GetFiles();
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
