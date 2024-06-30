namespace WhenFresh.Utilities.Data
{
    public sealed class TestConcatenationTransformer : ITransformData
    {
        public IEnumerable<KeyStringDictionary> Transform(IEnumerable<KeyStringDictionary> data)
        {
            foreach (var entry in data)
            {
                var value = string.Empty;
                foreach (var item in entry)
                {
                    value += item.Value;
                }

                yield return new KeyStringDictionary
                                 {
                                     { "CONCAT", value }
                                 };
            }
        }
    }
}