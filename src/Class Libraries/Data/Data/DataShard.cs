namespace Cavity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using Cavity.Collections;

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is not a collection.")]
    public class DataShard : DataSheet
    {
        public DataShard(string title)
            : this()
        {
            Title = title;
        }

        public DataShard()
        {
            Entries = new Collection<KeyStringDictionary>();
        }

        public ICollection<KeyStringDictionary> Entries { get; private set; }

        protected override IEnumerator<T> GetEnumerator<T>()
        {
            var convert = typeof(T).FullName != typeof(KeyStringDictionary).FullName;
            foreach (var entry in Entries)
            {
                if (convert)
                {
                    var instance = Activator.CreateInstance<T>();
                    foreach (var pair in entry)
                    {
                        instance[pair.Key] = pair.Value;
                    }

                    yield return instance;
                    continue;
                }

                yield return (T)entry;
            }
        }
    }
}