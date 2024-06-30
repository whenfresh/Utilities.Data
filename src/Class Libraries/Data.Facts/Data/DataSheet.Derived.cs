namespace WhenFresh.Utilities.Data;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "DataSheet", Justification = "This casing is correct.")]
[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is not a collection.")]
public sealed class DerivedDataSheet : DataSheet
{
    public override IEnumerable<T> As<T>()
    {
        yield break;
    }

    public override IEnumerator<KeyStringDictionary> GetEnumerator()
    {
        yield return new KeyStringDictionary
                         {
                             { "one", "1" },
                             { "two", "2" }
                         };
    }

    protected override IEnumerator<T> GetEnumerator<T>()
    {
        yield break;
    }
}