namespace Cavity.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Cavity.Collections;

    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "DataSheet", Justification = "This is the correct casing.")]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is not a collection.")]
    public abstract class DataSheet : IDataSheet
    {
        public string Title { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "As", Justification = "This naming is intentional.")]
        public virtual IEnumerable<T> As<T>()
            where T : KeyStringDictionary, new()
        {
            var enumerator = GetEnumerator<T>();
            while (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
        }

        public virtual IEnumerable<DataShard> Shard(IIdentifyShard identifier)
        {
            if (null == identifier)
            {
                throw new ArgumentNullException("identifier");
            }

            DataShard shard = null;
            string current = null;
            foreach (var entry in this)
            {
                var id = identifier.IdentifyShard(entry);
                if (null == shard)
                {
                    shard = new DataShard(id);
                }

                if (id != current)
                {
                    current = id;
                    if (0 != shard.Entries.Count)
                    {
                        yield return shard;
                    }

                    shard = new DataShard(id);
                }

                shard.Entries.Add(entry);
            }

            if (null == shard)
            {
                yield break;
            }

            if (0 != shard.Entries.Count)
            {
                yield return shard;
            }
        }

        public virtual DataTable ToDataTable()
        {
            return ToDataTable(null);
        }

        public virtual DataTable ToDataTable(string name)
        {
            DataTable table = null;
            foreach (var entry in this)
            {
                if (null == table)
                {
                    table = NewDataTable(name, entry);
                }

                table.Rows.Add(NewDataRow(table, entry));
            }

            return table;
        }

        public virtual IEnumerable<KeyStringDictionary> Transform(ITransformData transformer)
        {
            if (null == transformer)
            {
                throw new ArgumentNullException("transformer");
            }

            return transformer.Transform(this);
        }

        public virtual IEnumerable<T> Transform<T>(ITransformData<T> transformer)
        {
            if (null == transformer)
            {
                throw new ArgumentNullException("transformer");
            }

            return transformer.Transform(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual IEnumerator<KeyStringDictionary> GetEnumerator()
        {
            return GetEnumerator<KeyStringDictionary>();
        }

        protected abstract IEnumerator<T> GetEnumerator<T>()
            where T : KeyStringDictionary, new();

        private static DataRow NewDataRow(DataTable table,
                                          Dictionary<string, string> entry)
        {
            var row = table.NewRow();
            foreach (var key in entry.Keys)
            {
                row[key] = entry[key];
            }

            return row;
        }

        private static DataTable NewDataTable(string name,
                                              Dictionary<string, string> entry)
        {
            var table = new DataTable(name)
                            {
                                Locale = CultureInfo.InvariantCulture
                            };
            foreach (var key in entry.Keys)
            {
                table.Columns.Add(key);
            }

            return table;
        }
    }
}