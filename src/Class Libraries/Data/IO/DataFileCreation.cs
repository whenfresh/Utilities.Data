namespace Cavity.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using Cavity.Collections;
    using Cavity.Data;

    public class DataFileCreation
    {
        public DataFileCreation(FileInfo destination,
                                DateTime modified)
        {
            if (null == destination)
            {
                throw new ArgumentNullException("destination");
            }

            Destination = destination;
            Modified = modified;
        }

        public int Count { get; private set; }

        public FileInfo Destination { get; private set; }

        public DateTime Modified { get; private set; }

        public Stopwatch Stopwatch { get; private set; }

        public int Create(IEnumerable<KeyStringDictionary> data)
        {
            if (null == data)
            {
                throw new ArgumentNullException("data");
            }

            if (Destination.Exists)
            {
                throw new IOException("\"{0}\" already exists.".FormatWith(Destination.FullName));
            }

            Count = 0;
            Stopwatch = new Stopwatch();
            Stopwatch.Start();

            using (var temp = new TempFile(CurrentTempDirectory.Location))
            {
                using (var writers = new StreamWriterDictionary())
                {
                    foreach (var entry in data)
                    {
                        Count++;
                        if (null == writers.FirstLine)
                        {
                            writers.FirstLine = Csv.Header(entry);
                        }

                        writers.Item(temp.Info).WriteLine(Csv.Line(entry));
                    }
                }

                if (Count.IsNot(0))
                {
                    temp.Info.CopyTo(Destination);

                    Destination.SetDate(Modified);
                }
            }

            Stopwatch.Stop();
            return Count;
        }
    }
}