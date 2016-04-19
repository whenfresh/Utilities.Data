namespace Cavity.Data
{
    using System;
    using Cavity.Collections;

    public sealed class ShardByValue : IIdentifyShard
    {
        public ShardByValue(string column)
            : this()
        {
            if (null == column)
            {
                throw new ArgumentNullException("column");
            }

            column = column.Trim();
            if (0 == column.Length)
            {
                throw new ArgumentOutOfRangeException("column");
            }

            Column = column;
        }

        private ShardByValue()
        {
        }

        public string Column { get; private set; }

        public string IdentifyShard(KeyStringDictionary entry)
        {
            if (null == entry)
            {
                throw new ArgumentNullException("entry");
            }

            return (entry[Column] ?? string.Empty).Trim();
        }
    }
}