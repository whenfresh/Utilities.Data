namespace Cavity.Data
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    using Cavity.Collections;

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

        public string Name
        {
            get
            {
                return this["name"];
            }
        }
    }
}