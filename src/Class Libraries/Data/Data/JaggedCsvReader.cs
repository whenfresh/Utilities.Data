namespace Cavity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;

    public class JaggedCsvReader : StreamReader
    {
        public JaggedCsvReader(Stream stream)
            : base(stream)
        {
        }

        public Collection<string> Columns { get; private set; }

        public int EntryNumber { get; private set; }

        public string Line { get; private set; }

        public int LineNumber { get; private set; }

        public IList<string> ReadEntry()
        {
            if (null == Columns)
            {
                Columns = new Collection<string>();
                foreach (var heading in NextLine())
                {
                    Columns.Add(heading);
                }
            }

            var entry = NextLine();
            if (null == entry)
            {
                return null;
            }

            EntryNumber++;
            return entry;
        }

        private IList<string> NextLine()
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

        private IList<string> Parse(string line)
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
    }
}