using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

using System.IO;
using System.Threading;
using Forms = System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Navigation;
using System.Windows.Input;

namespace SoundKeyboard2012
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private static readonly string APP_SIGNATURE = Forms.Application.ProductName;
        private static readonly int BALOON_INTERVAL = 3 * 1000;

        public static SoundPacks SoundPacks;
        public static SoundEngine SoundEngine;
        public static DisplayInputWindow DisplayInput = new DisplayInputWindow();

        private static Mutex mMutex = null;
        private static bool mMutexCreatedNew = false;

        private static readonly Forms.NotifyIcon mNotifyIcon = new Forms.NotifyIcon();
        private static readonly Forms.ContextMenuStrip mContextMenuStrip = new Forms.ContextMenuStrip();
        private HotKey mHotKeyMute;
        private HotKey mHotKeyDisplayInput;

        public static string ConfigDir
        {
            get
            {
                var dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    APP_SIGNATURE);

                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                return dir;
            }
        }

        static string GetAssemblyPath()
        {
            return Environment.GetCommandLineArgs()[0];
        }

        static string GetStartupPath()
        {
            return Path.GetDirectoryName(GetAssemblyPath());
        }

        public static void ShowBaloonTip(string format, params string[] values)
        {
            mNotifyIcon.ShowBalloonTip(BALOON_INTERVAL, APP_SIGNATURE,
                string.Format(format, values), Forms.ToolTipIcon.Info);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        { 
            // To prevent App.MainWindow being set to DisplayInputWindow,
            // Set "new MainWindow()" manually at first.
            MainWindow = new MainWindow();

            // 01. Single Instance only allowed.

            mMutex = new Mutex(true, APP_SIGNATURE, out mMutexCreatedNew);
            if (!mMutexCreatedNew)
            {
                MessageBox.Show("二重起動しました。アプリケーションを終了します。",
                    APP_SIGNATURE, MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
            }

            // 02. Load SoundPacks

            try
            {
                SoundPacks = SoundPacks.Load(ConfigDir);
            }
            catch
            {
                SoundPacks = new SoundPacks();
                var sounds_dir = Path.Combine(GetStartupPath(), "Sounds");
                foreach (var d in new DirectoryInfo(sounds_dir).GetDirectories())
                    SoundPacks.Add(new SoundPack(d.FullName));
            }

            // 03. Load SoundEngine

            try
            {
                SoundEngine = SoundEngine.Load(ConfigDir);
            }
            catch
            {
                SoundEngine = SoundEngine.FromSoundPack(SoundPacks[0]);
            }

            SoundEngine.PropertyChanged += (_sender, _e) =>
            {
                switch (_e.PropertyName)
                {
                    case "Location":
                        ShowBaloonTip("サウンドパック {0} をロードしました",
                            SoundEngine.Name);
                        break;
                    case "MuteEnabled":
                        ShowBaloonTip("ミュートを {0} にしました",
                            SoundEngine.MuteEnabled ? "有効" : "無効");
                        break;
                    case "DefaultSoundEnabled":
                        ShowBaloonTip("デフォルトサウンドを {0} にしました",
                            SoundEngine.MuteEnabled ? "有効" : "無効");
                        break;
                    case "PressedKeys":
                        DisplayInput.Keys = SoundEngine.PressedKeys;
                        break;
                }
            };

            // 04. Load DisplayInputWindow

            try
            {
                DisplayInput.Load(ConfigDir);
            }
            catch /* Load Default Value */
            {
                DisplayInput.DisplayMargin = 25;
                DisplayInput.DisplayPosition = DisplayInputWindow.Position.TopRight;
                DisplayInput.Visible = true;
            }

            // 05. Prepare NotifyIcon

            mNotifyIcon.Text = APP_SIGNATURE;
            mNotifyIcon.ContextMenuStrip = mContextMenuStrip;
            mNotifyIcon.Visible = true;
            mNotifyIcon.Icon = new Icon(
                Path.Combine(GetStartupPath(), "SoundKeyboard.ico"),
                new System.Drawing.Size(16, 16)
            );
            mNotifyIcon.DoubleClick += new EventHandler(ToggleMainWindowVisible);
            mContextMenuStrip.Opening += new CancelEventHandler(ContextMenuStrip_Opening);

            ContextMenuStrip_Opening(); // Call once to initialize mContextMenuStrip.

            // 06. Register Hotkeys

            mHotKeyMute = new HotKey(
                Key.X, KeyModifier.Win,
                (_) => { SoundEngine.MuteEnabled = !SoundEngine.MuteEnabled; }
            );

            mHotKeyDisplayInput = new HotKey(
                Key.K, KeyModifier.Win,
                (_) => { DisplayInput.Visible = !DisplayInput.Visible; }
            );

            App.ShowBaloonTip("起動しました");
        }

        void ContextMenuStrip_Opening(object sender = null, CancelEventArgs e = null)
        {
            Forms.ToolStripItem item;
            mContextMenuStrip.Items.Clear();

            item = new Forms.ToolStripMenuItem("メイン画面の表示");
            item.Click += new EventHandler(ToggleMainWindowVisible);
            mContextMenuStrip.Items.Add(item);

            item = new Forms.ToolStripSeparator();
            mContextMenuStrip.Items.Add(item);

            item = new Forms.ToolStripMenuItem("サウンドパックの選択");
            foreach (var pack in SoundPacks)
            {
                var dropdown = new Forms.ToolStripMenuItem()
                {
                    Text = pack.Name,
                    ToolTipText = pack.Location,
                    Checked = SoundEngine.Location == pack.Location,
                };

                dropdown.Click += (_sender, _e) =>
                {
                    SoundEngine.Location = pack.Location;
                };

                (item as Forms.ToolStripMenuItem).DropDown.Items.Add(dropdown);
            }
            mContextMenuStrip.Items.Add(item);

            item = new Forms.ToolStripSeparator();
            mContextMenuStrip.Items.Add(item);

            item = new Forms.ToolStripMenuItem("ミュート") { Checked = SoundEngine.MuteEnabled, ShortcutKeyDisplayString = "Windows + X", };
            item.Click += new EventHandler(ToggleMuteEnabled);
            mContextMenuStrip.Items.Add(item);

            item = new Forms.ToolStripMenuItem("デフォルトサウンドの再生") { Checked = SoundEngine.DefaultSoundEnabled };
            item.Click += new EventHandler(ToggleDefaultSoundEnabled);
            mContextMenuStrip.Items.Add(item);

            item = new Forms.ToolStripMenuItem("キー入力の表示") { Checked = DisplayInput.Visible, ShortcutKeyDisplayString = "Windows + K" };
            item.Click += new EventHandler(ToggleDisplayInputEnabled);
            mContextMenuStrip.Items.Add(item);

            item = new Forms.ToolStripSeparator();
            mContextMenuStrip.Items.Add(item);

            item = new Forms.ToolStripMenuItem(
                string.Format("キーボードフックの修復: {0}", GlobalKeybordMonitor.HookHandle));
            item.Click += new EventHandler(RepairKeyboardHook);
            mContextMenuStrip.Items.Add(item);

            item = new Forms.ToolStripSeparator();
            mContextMenuStrip.Items.Add(item);

            item = new Forms.ToolStripMenuItem("アプリケーションの終了");
            item.Click += new EventHandler(ExitApplication);
            mContextMenuStrip.Items.Add(item);
        }

        void RepairKeyboardHook(object sender, EventArgs e)
        {
            GlobalKeybordMonitor.InitializeKeyboardHook();
        }

        void ToggleDisplayInputEnabled(object sender, EventArgs e)
        {
            DisplayInput.Visible = !DisplayInput.Visible;
        }

        void ToggleDefaultSoundEnabled(object sender, EventArgs e)
        {
            SoundEngine.DefaultSoundEnabled = !SoundEngine.DefaultSoundEnabled;
        }

        void ToggleMuteEnabled(object sender, EventArgs e)
        {
            SoundEngine.MuteEnabled = !SoundEngine.MuteEnabled;
        }

        void ToggleMainWindowVisible(object sender, EventArgs e)
        {
            if (MainWindow.WindowState == WindowState.Minimized)
            {
                MainWindow.WindowState = WindowState.Normal;
                MainWindow.Show();
            }
            else
            {
                MainWindow.Hide();
                MainWindow.WindowState = WindowState.Minimized;
            }
        }

        void ExitApplication(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            SoundPacks.Save(ConfigDir);
            SoundEngine.Save(ConfigDir);
            DisplayInput.Save(ConfigDir);

            SoundEngine.Dispose();

            mNotifyIcon.Visible = false;

            if (mMutexCreatedNew && mMutex != null) mMutex.ReleaseMutex();
        }
    }
}
