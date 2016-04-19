namespace Cavity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using Cavity.Collections;
    using Cavity.Properties;

    public class CsvStreamReader : StreamReader
    {
        public CsvStreamReader(Stream stream)
            : base(stream)
        {
        }

        public CsvStreamReader(Stream stream,
                               string header)
            : this(stream, ParseHeader(header))
        {
            Header = header;
        }

        public CsvStreamReader(Stream stream,
                               IEnumerable<string> columns)
            : base(stream)
        {
            if (null == columns)
            {
                throw new ArgumentNullException("columns");
            }

            Columns = new Collection<string>();
            foreach (var header in columns)
            {
                Columns.Add(header);
                Header += (string.IsNullOrEmpty(Header) ? string.Empty : ",") + header;
            }

            if (0 == Columns.Count)
            {
                throw new ArgumentOutOfRangeException("columns");
            }
        }

        public int EntryNumber { get; protected set; }

        public string Header { get; protected set; }

        public string Line { get; protected set; }

        public int LineNumber { get; protected set; }

        protected Collection<string> Columns { get; private set; }

        public virtual KeyStringDictionary ReadEntry()
        {
            return ReadEntry<KeyStringDictionary>();
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "A non-generic version is available.")]
        public virtual KeyStringDictionary ReadEntry<T>()
            where T : KeyStringDictionary, new()
        {
            var result = Activator.CreateInstance<T>();

            if (null == Columns)
            {
                Columns = new Collection<string>();
                foreach (var heading in NextLine())
                {
                    Columns.Add(heading);
                }

                Header = Line;
            }

            var entry = NextLine();
            if (null == entry)
            {
                return null;
            }

            if (0 != entry.Count)
            {
                EntryNumber++;
                if (Columns.Count !=
                    entry.Count)
                {
#if NET20
                    throw new FormatException(StringExtensionMethods.FormatWith(Resources.ReadEntry_FormatException, LineNumber));
#else
                    throw new FormatException(Resources.ReadEntry_FormatException.FormatWith(LineNumber));
#endif
                }

                for (var i = 0; i < Columns.Count; i++)
                {
                    result.Add(Columns[i], entry[i]);
                }
            }

            return result;
        }

        protected virtual IList<string> NextLine()
        {
            Line = null;
            while (!EndOfStream)
            {
                Line = ReadLine();
                LineNumber++;
                if (!string.IsNullOrEmpty(Line))
                {
                    break;
                }
            }

            return Parse(Line);
        }

        protected virtual IList<string> Parse(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return null;
            }

            IList<string> result = new List<string>();

            var trim = true;
            var buffer = new StringBuilder();
            var quote = false;
            for (var i = 0; i < line.Length; i++)
            {
                var c = line[i];
                switch (c)
                {
                    case ',':
                        if (quote)
                        {
                            buffer.Append(c);
                            break;
                        }

                        result.Add(trim ? buffer.ToString().Trim() : buffer.ToString());
                        buffer.Remove(0, buffer.Length);
                        trim = true;
                        break;

                    case '"':
                        trim = false;
                        if (quote)
                        {
                            if (i == line.Length - 1)
                            {
                                quote = false;
                                break;
                            }

                            if ('"' == line[i + 1])
                            {
                                buffer.Append(c);
                                i++;
                                break;
                            }

                            quote = false;
                            break;
                        }

                        if (0 == buffer.Length)
                        {
                            quote = true;
                        }

                        break;

                    default:
                        buffer.Append(c);
                        break;
                }
            }

            if (quote)
            {
#if NET20
                Line = StringExtensionMethods.Append(line, Environment.NewLine, ReadLine());
#else
                Line = line.Append(Environment.NewLine, ReadLine());
#endif
                LineNumber++;
                return Parse(Line);
            }

            result.Add(trim ? buffer.ToString().Trim() : buffer.ToString());

            return result;
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "This is an odd rule that seems to be impossible to actually pass.")]
        private static IEnumerable<string> ParseHeader(string header)
        {
            if (null == header)
            {
                throw new ArgumentNullException("header");
            }

            if (0 == header.Length)
            {
                throw new ArgumentOutOfRangeException("header");
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine(header);
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new CsvStreamReader(stream))
                    {
                        reader.ReadEntry();
                        return reader.Columns;
                    }
                }
            }
        }
    }
}