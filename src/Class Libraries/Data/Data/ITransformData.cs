namespace WhenFresh.Utilities.Data.Data
{
    public interface ITransformData
    {
        IEnumerable<KeyStringDictionary> Transform(IEnumerable<KeyStringDictionary> data);
    }
}