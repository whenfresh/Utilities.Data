namespace Cavity.Data
{
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using Cavity.Collections;

    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "DataSheet", Justification = "This is the correct casing.")]
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is not a collection.")]
    public interface IDataSheet : IEnumerable<KeyStringDictionary>
    {
        string Title { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "As", Justification = "This naming is intentional.")]
        IEnumerable<T> As<T>()
            where T : KeyStringDictionary, new();

        IEnumerable<DataShard> Shard(IIdentifyShard identifier);

        DataTable ToDataTable();

        DataTable ToDataTable(string name);

        IEnumerable<KeyStringDictionary> Transform(ITransformData transformer);

        IEnumerable<T> Transform<T>(ITransformData<T> transformer);
    }
}