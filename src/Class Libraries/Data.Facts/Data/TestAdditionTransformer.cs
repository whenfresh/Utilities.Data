namespace Cavity.Data
{
    using System.Collections.Generic;
    using System.Xml;
    using Cavity.Collections;

    public sealed class TestAdditionTransformer : ITransformData<int>
    {
        public IEnumerable<int> Transform(IEnumerable<KeyStringDictionary> data)
        {
            foreach (var entry in data)
            {
                var value = 0;
                foreach (var item in entry)
                {
                    value += XmlConvert.ToInt32(item.Value);
                }

                yield return value;
            }
        }
    }
}