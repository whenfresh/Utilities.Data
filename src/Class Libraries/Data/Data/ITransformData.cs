namespace Cavity.Data
{
    using System.Collections.Generic;
    using Cavity.Collections;

    public interface ITransformData
    {
        IEnumerable<KeyStringDictionary> Transform(IEnumerable<KeyStringDictionary> data);
    }
}