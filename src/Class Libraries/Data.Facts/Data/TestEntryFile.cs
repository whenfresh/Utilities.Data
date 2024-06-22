namespace WhenFresh.Utilities.Data.Facts.Data
{
    public sealed class TestEntryFile
    {
        public TestEntryFile(FileInfo info)
        {
            Info = info;
        }

        public FileInfo Info { get; set; }

        public FileInfo GetFile(KeyStringDictionary entry)
        {
            return null == entry ? null : Info;
        }
    }
}