using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Serialization;
using System.IO;
using System.ComponentModel;
using System.Collections.Specialized;

namespace SoundKeyboard2012
{
    [Serializable]
    public class SoundPacks : IList<SoundPack>, INotifyPropertyChanged, INotifyCollectionChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private List<SoundPack> mItems = new List<SoundPack>();

        public static SoundPacks Load(string dir)
        {
            var serializer = new XmlSerializer(typeof(SoundPacks));

            using (var reader = new StreamReader(Path.Combine(dir, "SoundPacks.config")))
            {
                return serializer.Deserialize(reader) as SoundPacks;
            }
        }

        public void Save(string dir)
        {
            using (var writer = new StreamWriter(Path.Combine(dir, "SoundPacks.config")))
            {
                new XmlSerializer(typeof(SoundPacks)).Serialize(writer, this);
            }
        }

        NotifyCollectionChangedEventArgs EventArgs()
        {
            return new NotifyCollectionChangedEventArgs(
                NotifyCollectionChangedAction.Reset);
        }

        public int IndexOf(SoundPack item)
        {
            return mItems.IndexOf(item);
        }

        public void Insert(int index, SoundPack item)
        {
            mItems.Insert(index, item);
            if (CollectionChanged != null)
                CollectionChanged(this, EventArgs());
        }

        public void RemoveAt(int index)
        {
            mItems.RemoveAt(index);
            if (CollectionChanged != null)
                CollectionChanged(this, EventArgs());
        }

        public SoundPack this[int index]
        {
            get
            {
                return mItems[index];
            }
            set
            {
                if (mItems[index] != value) mItems[index] = value;

                if (CollectionChanged != null)
                    CollectionChanged(this, EventArgs());
            }
        }

        public void Add(SoundPack item)
        {
            mItems.Add(item);
            if (CollectionChanged != null)
                CollectionChanged(this, EventArgs());
        }

        public void Clear()
        {
            mItems.Clear();
            if (CollectionChanged != null)
                CollectionChanged(this, EventArgs());
        }

        public bool Contains(SoundPack item)
        {
            return mItems.Contains(item);
        }

        public void CopyTo(SoundPack[] array, int arrayIndex)
        {
            mItems.CopyTo(mItems.ToArray(), arrayIndex);
        }

        public int Count
        {
            get { return mItems.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(SoundPack item)
        {
            var value = mItems.Remove(item);
            if (CollectionChanged != null)
                CollectionChanged(this, EventArgs());
            return value;
        }

        public IEnumerator<SoundPack> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return mItems.GetEnumerator();
        }
    }

    public class SoundPack : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public SoundPack() { }

        public SoundPack(string path) { Location = path; }

        public string Location { get; set; }

        public string Name
        {
            get { return Path.GetFileNameWithoutExtension(Location); }
        }

        public int Count
        {
            get { return Items.Count(); }
        }

        public bool IsValid
        {
            get { return Directory.Exists(Location); }
        }

        public string[] Items
        {
            get
            {
                return new DirectoryInfo(Location)
                        .EnumerateFiles()
                        .Where(_ => _.Extension == ".wav")
                        .Select(_ => _.FullName)
                        .ToArray();
            }
        }
    }
}
