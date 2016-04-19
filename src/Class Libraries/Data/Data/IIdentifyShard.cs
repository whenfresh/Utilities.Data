namespace Cavity.Data
{
    using Cavity.Collections;

    public interface IIdentifyShard
    {
        string IdentifyShard(KeyStringDictionary entry);
    }
}