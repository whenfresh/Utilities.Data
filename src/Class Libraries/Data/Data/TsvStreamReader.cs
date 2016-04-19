namespace Cavity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
#if !NET20
    using System.Linq;
#endif
    using Cavity.Collections;
    using Cavity.Properties;

    public class TsvStreamReader : StreamReader
    {
        public TsvStreamReader(Stream stream)
            : base(stream)
        {
        }

        public TsvStreamReader(Stream stream,
                               string header)
            : this(stream, ParseHeader(header))
        {
            Header = header;
        }

        public TsvStreamReader(Stream stream,
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
                Header += (string.IsNullOrEmpty(Header) ? string.Empty : "\t") + header;
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
                if (Columns.Count != entry.Count)
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

            return string.IsNullOrEmpty(Line)
                       ? null
#if NET20
                       : IEnumerableExtensionMethods.ToList(Line.Split('\t'));
#else
                       : Line.Split('\t').ToList();
#endif
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
                    using (var reader = new TsvStreamReader(stream))
                    {
                        reader.ReadEntry();
                        return reader.Columns;
                    }
                }
            }
        }
    }
}