namespace WhenFresh.Utilities.Data.Data
{
    public interface ITransformData<T>
    {
        IEnumerable<T> Transform(IEnumerable<KeyStringDictionary> data);
    }
}