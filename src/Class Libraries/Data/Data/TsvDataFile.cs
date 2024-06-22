namespace WhenFresh.Utilities.Data.Data
{
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is not a collection.")]
    public class TsvDataFile : DataFile
    {
        public TsvDataFile(FileInfo info)
            : base(info)
        {
        }

        public override IEnumerator<IDataSheet> GetEnumerator()
        {
            yield return new TsvDataSheet(Info)
                             {
                                 Title = Title
                             };
        }
    }
}