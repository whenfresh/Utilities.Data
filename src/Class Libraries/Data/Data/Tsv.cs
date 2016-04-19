namespace Cavity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
#if !NET20
    using System.Linq;
#endif
    using System.Text;
    using Cavity.Collections;
    using Cavity.IO;

    public static class Tsv
    {
        public static string Header(KeyStringDictionary data)
        {
            if (null == data)
            {
                throw new ArgumentNullException("data");
            }

            return Line(data.Keys);
        }

        public static string Line(KeyStringDictionary data)
        {
            if (null == data)
            {
                throw new ArgumentNullException("data");
            }

            return Line(data.Values);
        }

        public static string Line(KeyStringDictionary data,
                                  IList<string> columns)
        {
            if (null == data)
            {
                throw new ArgumentNullException("data");
            }

            if (null == columns)
            {
                throw new ArgumentNullException("columns");
            }

            if (0 == columns.Count)
            {
                throw new ArgumentOutOfRangeException("columns");
            }

#if NET20
            var values = new List<string>();
            foreach (var column in columns)
            {
                values.Add(data[column.Normalize().Trim()]);
            }
#else
            var values = columns
                .Select(column => data[column.Normalize().Trim()])
                .ToList();
#endif

            return Line(values);
        }

        public static string Line(IEnumerable<string> data)
        {
            if (null == data)
            {
                throw new ArgumentNullException("data");
            }

            var buffer = new StringBuilder();
            var i = 0;
            foreach (var item in data)
            {
                if (0 != i)
                {
                    buffer.Append('\t');
                }

                if (-1 != item.IndexOf('\t'))
                {
                    throw new FormatException("Tab separated values cannot contain tabs.");
                }

                buffer.Append(item);
                i++;
            }

            if (0 == i)
            {
                throw new ArgumentOutOfRangeException("data");
            }

            return buffer.ToString();
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "I want type safety here.")]
        public static void Save(DataTable table,
                                FileInfo file)
        {
            if (null == file)
            {
                throw new ArgumentNullException("file");
            }

            file.Refresh();
            Save(table, file, file.Exists ? FileMode.Append : FileMode.CreateNew);
        }

        public static void Save(DataTable table,
                                FileInfo file,
                                FileMode mode)
        {
            if (null == table)
            {
                throw new ArgumentNullException("table");
            }

            if (null == file)
            {
                throw new ArgumentNullException("file");
            }

            file.Refresh();
            if (null != file.Directory && !file.Directory.Exists)
            {
                file.Directory.Create();
            }

            if (0 == table.Rows.Count)
            {
                return;
            }

            var exists = file.Exists;
            using (var stream = file.Open(mode, FileAccess.Write, FileShare.Read))
            {
                using (var writer = new StreamWriter(stream))
                {
                    var buffer = new StringBuilder();
                    var index = 0;
                    if (!exists)
                    {
                        foreach (DataColumn column in table.Columns)
                        {
                            if (0 != index)
                            {
                                buffer.Append('\t');
                            }

                            if (-1 != column.ColumnName.IndexOf('\t'))
                            {
                                throw new FormatException("Tab separated values cannot contain tabs.");
                            }

                            buffer.Append(column.ColumnName);
                            index++;
                        }
                    }

                    if (0 != buffer.Length)
                    {
                        writer.WriteLine(buffer.ToString());
                    }

#if NET20 || NET35
                    buffer = new StringBuilder();
#else
                    buffer.Clear();
#endif
                    index = 0;
                    foreach (DataRow row in table.Rows)
                    {
                        foreach (var item in row.ItemArray)
                        {
                            if (0 != index)
                            {
                                buffer.Append('\t');
                            }

                            var value = item.ToString();
                            if (-1 != value.IndexOf('\t'))
                            {
                                throw new FormatException("Tab separated values cannot contain tabs.");
                            }

                            buffer.Append(value);
                            index++;
                        }

                        writer.WriteLine(buffer.ToString());
                    }
                }
            }
        }

#if !NET20
        public static void Save(IEnumerable<KeyStringDictionary> data,
                                Func<KeyStringDictionary, FileInfo> file)
        {
            if (null == data)
            {
                throw new ArgumentNullException("data");
            }

            if (null == file)
            {
                throw new ArgumentNullException("file");
            }

            var first = true;
            using (var writers = new StreamWriterDictionary
                                     {
                                         Access = FileAccess.Write,
                                         Mode = FileMode.Append,
                                         Share = FileShare.Read
                                     })
            {
                foreach (var entry in data)
                {
                    if (first)
                    {
                        writers.FirstLine = Header(entry);
                        first = false;
                    }

                    var info = file(entry);
                    if (null != info.Directory && !info.Directory.Exists)
                    {
                        info.Directory.Create();
                    }

                    writers.Item(info).WriteLine(Line(entry));
                }
            }
        }
#endif

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "I want type safety here.")]
        public static void Save(IEnumerable<KeyStringDictionary> data,
                                FileInfo file)
        {
            if (null == file)
            {
                throw new ArgumentNullException("file");
            }

            file.Refresh();
            Save(data, file, file.Exists ? FileMode.Append : FileMode.CreateNew);
        }

        public static void Save(IEnumerable<KeyStringDictionary> data,
                                FileInfo file,
                                FileMode mode)
        {
            if (null == data)
            {
                throw new ArgumentNullException("data");
            }

            if (null == file)
            {
                throw new ArgumentNullException("file");
            }

            file.Refresh();
            if (null != file.Directory && !file.Directory.Exists)
            {
                file.Directory.Create();
            }

            var count = 0;
            using (var stream = file.Open(mode, FileAccess.Write, FileShare.Read))
            {
                using (var writer = new StreamWriter(stream))
                {
                    foreach (var line in data)
                    {
                        if (0 == count && !file.Exists)
                        {
                            writer.WriteLine(Header(line));
                        }

                        writer.WriteLine(Line(line));
                        count++;
                    }
                }
            }

            file.Refresh();
            if (0 == count && file.Exists)
            {
                file.Delete();
            }
        }
    }
}