﻿namespace WhenFresh.Utilities.Data
{
    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "DataSheet", Justification = "This is the correct casing.")]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is not a collection.")]
    public class TsvDataSheet : DataSheet
    {
        public TsvDataSheet(string path)
            : this(new FileInfo(path))
        {
        }

        public TsvDataSheet(FileInfo info)
        {
            if (null == info)
            {
                throw new ArgumentNullException("info");
            }

            if (!info.Exists)
            {
                throw new FileNotFoundException(info.FullName);
            }

            Info = info;
        }

        public FileInfo Info { get; private set; }

        protected override IEnumerator<T> GetEnumerator<T>()
        {
            Info.Refresh();
            if (!Info.Exists)
            {
                yield break;
            }

            using (var stream = Info.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new TsvStreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        var entry = reader.ReadEntry<T>();
                        if (null == entry)
                        {
                            break;
                        }

                        yield return (T)entry;
                    }
                }
            }
        }
    }
}