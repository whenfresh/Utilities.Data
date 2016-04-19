namespace Cavity.Data
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
#if !NET20
    using System.Linq;
#endif
    using System.Xml;

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is not a collection.")]
    public abstract class DataFile : IEnumerable<IDataSheet>
    {
        protected DataFile(FileInfo info)
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

        public string Title
        {
            get
            {
                return 0 == Info.Extension.Length
                           ? Info.Name
#if NET20
                           : StringExtensionMethods.RemoveFromEnd(Info.Name, Info.Extension, StringComparison.OrdinalIgnoreCase);
#else
                           : Info.Name.RemoveFromEnd(Info.Extension, StringComparison.OrdinalIgnoreCase);
#endif
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "This is an indexer, not a property.")]
        [SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "This is the expected exception type for invalid indexes.")]
        public IDataSheet this[int index]
        {
            get
            {
                var i = -1;
                foreach (var sheet in this)
                {
                    i++;
                    if (i != index)
                    {
                        continue;
                    }

                    return sheet;
                }

                throw new IndexOutOfRangeException(XmlConvert.ToString(index));
            }
        }

        public IDataSheet this[string title]
        {
            get
            {
#if NET20
                foreach (var sheet in this)
                {
                    if (sheet.Title != title)
                    {
                        continue;
                    }

                    return sheet;
                }
#else
                foreach (var sheet in this.Where(sheet => sheet.Title == title))
                {
                    return sheet;
                }
#endif

                throw new KeyNotFoundException(title);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public abstract IEnumerator<IDataSheet> GetEnumerator();
    }
}