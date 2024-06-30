namespace WhenFresh.Utilities.Data;

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This naming is intentional.")]
[Serializable]
public sealed class TestDataEntry : KeyStringDictionary
{
    public TestDataEntry()
    {
    }

    private TestDataEntry(SerializationInfo info,
                          StreamingContext context)
        : base(info, context)
    {
    }

    public string Name => this["name"];
}