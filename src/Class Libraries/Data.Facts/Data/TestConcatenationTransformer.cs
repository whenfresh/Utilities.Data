namespace Cavity.Data
{
    using System.Collections.Generic;
    using Cavity.Collections;

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