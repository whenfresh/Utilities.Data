namespace Cavity.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Cavity.Collections;
    using Cavity.Data;

    public sealed class TempCsvFile : TempFile
    {
        public TempCsvFile(IEnumerable<KeyStringDictionary> data)
            : base(CurrentTempDirectory.Location)
        {
            if (null == data)
            {
                throw new ArgumentNullException("data");
            }

            using (var writers = new StreamWriterDictionary())
            {
                foreach (var entry in data)
                {
                    if (null == writers.FirstLine)
                    {
                        writers.FirstLine = Csv.Header(entry);
                    }

                    writers.Item(Info).WriteLine(Csv.Line(entry));
                }
            }
        }

        public static int Create(IEnumerable<KeyStringDictionary> data,
                                 FileInfo destination)
        {
            return Create(data, destination, DateTime.UtcNow);
        }

        public static int Create(IEnumerable<KeyStringDictionary> data,
                                 FileInfo destination,
                                 DateTime modified)
        {
            using (var file = new TempCsvFile(data))
            {
                return file.Info.Exists
                           ? new DataFileCreation(destination, modified).Create(new CsvDataSheet(file.Info))
                           : 0;
            }
        }
    }
}