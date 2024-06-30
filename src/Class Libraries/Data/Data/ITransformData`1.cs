namespace WhenFresh.Utilities.Data
{
    public interface ITransformData<T>
    {
        IEnumerable<T> Transform(IEnumerable<KeyStringDictionary> data);
    }
}