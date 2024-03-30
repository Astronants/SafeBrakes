using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;

namespace SafeBrakes
{
    public class PresetCollection : ICollection<Preset>
    {
        public Preset Selected;
        private readonly Collection<Preset> data = new Collection<Preset>();

        public int Count => data.Count;

        public bool IsReadOnly => ((ICollection<Preset>)data).IsReadOnly;

        private Preset MakeDefault()
        {
            Preset cfg = new Preset("Default");
            data.Add(cfg);
            cfg.Save();
            return cfg;
        }

        public Preset FirstOrDefault()
        {
            if (Count > 0) return data[0];
            return MakeDefault();
        }
        public Preset FirstOrDefault(string fileName)
        {
            if (data.Any(e => e.FileName == fileName)) return data.First(e => e.FileName == fileName);
            if (data.Any(e => e.FileName == "Default.cfg")) return data.First(e => e.FileName == "Default.cfg");
            return MakeDefault();
        }

        public void Add(Preset item)
        {
            data.Add(item);
        }

        public void Insert(int v, Preset cfg)
        {
            data.Insert(v, cfg);
        }

        public void Clear()
        {
            data.Clear();
            Selected = null;
        }

        public bool Contains(Preset item)
        {
            return data.Contains(item);
        }

        internal bool Exists(Predicate<Preset> match)
        {
            return data.ToList().Exists(match);
        }

        public void CopyTo(Preset[] array, int arrayIndex)
        {
            data.CopyTo(array, arrayIndex);
        }

        public bool Remove(Preset item)
        {
            File.Delete(Path.Combine(DirUtils.PresetsDir, item.FileName));
            return data.Remove(item);
        }

        public IEnumerator<Preset> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }
    }
}
