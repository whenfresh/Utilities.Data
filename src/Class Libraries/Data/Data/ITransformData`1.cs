namespace Cavity.Data
{
    using System.Collections.Generic;
    using Cavity.Collections;

    public interface ITransformData<T>
    {
        IEnumerable<T> Transform(IEnumerable<KeyStringDictionary> data);
    }
}