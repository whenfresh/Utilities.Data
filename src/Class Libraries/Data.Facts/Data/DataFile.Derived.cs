namespace Cavity.Data
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is not a collection.")]
    public sealed class DerivedDataFile : DataFile
    {
        public DerivedDataFile(FileInfo info)
            : base(info)
        {
            Sheets = new Collection<IDataSheet>();
        }

        public ICollection<IDataSheet> Sheets { get; private set; }

        public override IEnumerator<IDataSheet> GetEnumerator()
        {
            return Sheets.GetEnumerator();
        }
    }
}