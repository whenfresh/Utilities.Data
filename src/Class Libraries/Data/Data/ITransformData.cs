namespace WhenFresh.Utilities.Data
{
    public interface ITransformData
    {
        IEnumerable<KeyStringDictionary> Transform(IEnumerable<KeyStringDictionary> data);
    }
}