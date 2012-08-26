using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Media;
using System.Windows.Input;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing;

namespace SoundKeyboard2012
{
    [Serializable]
    public class SoundEngine : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<string, SoundPlayer> mSounds = null;
        private List<Key> mPressedKeys = new List<Key>();

        private void RegisterGlobalKeyboardEvent()
        {
            GlobalKeybordMonitor.KeyDown
                += new EventHandler<GlobalKeyEventArgs>(GlobalKeybordMonitor_KeyDown);
            GlobalKeybordMonitor.KeyUp
                += new EventHandler<GlobalKeyEventArgs>(GlobalKeybordMonitor_KeyUp);
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        internal string GetFileNameToSave(string location, string prefix, string extension)
        {
            var filename = Path.Combine(location, prefix + extension);
            var i = 0;

            while (File.Exists(filename))
            {
                filename = Path.Combine(location,
                    string.Format("{0} ({1}){2}", prefix, ++i, extension));
            }

            return filename;
        }

        readonly string location = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                "スクリーンショット");
        const string prefix = "スクリーンショット";
        const string extension = ".png";

        private void GlobalKeybordMonitor_KeyDown(object sender, GlobalKeyEventArgs e)
        {
            if (mPressedKeys.Contains(e.KeyData)) return;

            var k = e.KeyData.ToString().ToLower();

            #region Windows 8 用のおまけ機能
            if (e.KeyData == Key.O && mPressedKeys.Contains(Key.LWin))
            {
                var rect = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                System.Drawing.Image image = new System.Drawing.Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb);

                using (var g = System.Drawing.Graphics.FromImage(image))
                {
                    g.CopyFromScreen(rect.X, rect.Y, 0, 0, rect.Size, CopyPixelOperation.SourceCopy);
                }
                image.Save(GetFileNameToSave(location, prefix, extension), ImageFormat.Png);

                k = "SnapShot".ToLower();
                e.Handled = true;
            }
            #endregion

            if (!MuteEnabled)
            {
                if (mSounds.Keys.Contains(k))
                    mSounds[k].Play();
                else if (DefaultSoundEnabled && mSounds.Keys.Contains("default"))
                    mSounds["default"].Play();
            }

            mPressedKeys.Add(e.KeyData);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("PressedKeys"));
        }

        private void GlobalKeybordMonitor_KeyUp(object sender, GlobalKeyEventArgs e)
        {
            mPressedKeys.Remove(e.KeyData);
            /*
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("PressedKeys"));
             */ 
        }

        public SoundEngine()
        {
            RegisterGlobalKeyboardEvent();
        }

        public SoundEngine(string path)
        {
            RegisterGlobalKeyboardEvent();
            Location = path;
        }

        public static SoundEngine FromSoundPack(SoundPack pack)
        {
            return new SoundEngine(pack.Location);
        }

        public static SoundEngine Load(string dir)
        {
            var serializer = new XmlSerializer(typeof(SoundEngine));

            using (var file = new StreamReader(Path.Combine(dir, "SoundEngine.config")))
            {
                return serializer.Deserialize(file) as SoundEngine;
            }
        }

        public void Save(string dir)
        {
            var serializer = new XmlSerializer(typeof(SoundEngine));

            using (var file = new StreamWriter(Path.Combine(dir, "SoundEngine.config")))
            {
                serializer.Serialize(file, this);
            }
        }

        public void Dispose()
        {
            foreach (var sound in mSounds.Values) sound.Dispose();
        }

        #region property Name

        public string Name
        {
            get
            {
                return Path.GetFileNameWithoutExtension(Location);
            }
        }

        #endregion

        #region property Location

        public string Location
        {
            get { return mLocation; }
            set
            {
                if (mLocation == value) return;

                mLocation = value;

                if (mSounds != null)
                    foreach (var sound in mSounds.Values)
                        sound.Dispose();

                mSounds = new System.IO.DirectoryInfo(mLocation)
                    .EnumerateFiles()
                    .Where(_ => _.Extension == ".wav")
                    .Select(_ => new SoundPlayer(_.FullName))
                    .ToDictionary(_ =>
                    {
                        return Path.GetFileNameWithoutExtension(
                            _.SoundLocation).ToLower();
                    });
            }
        }
        private string mLocation;

        #endregion

        #region property DefaultSoundEnabled

        public bool DefaultSoundEnabled { get; set; }

        #endregion

        #region property MuteEnabled

        public bool MuteEnabled { get; set; }

        #endregion

        #region property PressedKeys

        public Key[] PressedKeys { get { return mPressedKeys.ToArray(); } }

        #endregion
    }
}
