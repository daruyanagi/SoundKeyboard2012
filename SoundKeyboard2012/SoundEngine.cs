using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Media;
using System.Windows.Input;
using System.IO;
using System.Xml.Serialization;
using System.ComponentModel;

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

        private void GlobalKeybordMonitor_KeyDown(object sender, GlobalKeyEventArgs e)
        {
            if (mPressedKeys.Contains(e.KeyData)) return;

            var k = e.KeyData.ToString().ToLower();

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
