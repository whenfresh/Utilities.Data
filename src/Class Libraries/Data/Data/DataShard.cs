namespace WhenFresh.Utilities.Data;

using System.Collections.ObjectModel;

[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is not a collection.")]
public class DataShard : DataSheet
{
    public DataShard(string title)
        : this()
    {
        Title = title;
    }

    public DataShard()
    {
        Entries = new Collection<KeyStringDictionary>();
    }

    public ICollection<KeyStringDictionary> Entries { get; }

    protected override IEnumerator<T> GetEnumerator<T>()
    {
        var convert = typeof(T).FullName != typeof(KeyStringDictionary).FullName;
        foreach (var entry in Entries)
        {
            if (convert)
            {
                var instance = Activator.CreateInstance<T>();
                foreach (var pair in entry)
                    instance[pair.Key] = pair.Value;

                yield return instance;
                continue;
            }

            yield return (T)entry;
        }
    }
}