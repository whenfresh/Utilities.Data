namespace WhenFresh.Utilities.Data
{
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is not a collection.")]
    public class CsvDataFile : DataFile
    {
        public CsvDataFile(FileInfo info)
            : base(info)
        {
        }

        public override IEnumerator<IDataSheet> GetEnumerator()
        {
            yield return new CsvDataSheet(Info)
                             {
                                 Title = Title
                             };
        }
    }
}