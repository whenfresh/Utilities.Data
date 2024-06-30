namespace WhenFresh.Utilities.Data.Xunit.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using global::Xunit.Sdk;
    using WhenFresh.Utilities.Core;
    using WhenFresh.Utilities.Core.Collections;
    using WhenFresh.Utilities.Data.Data;
    using WhenFresh.Utilities.Data.Xunit.Properties;
#if !NET20
#endif

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class TsvDataAttribute : DataAttribute
    {
        public TsvDataAttribute(params string[] files)
            : this()
        {
            if (null == files)
            {
                throw new ArgumentNullException("files");
            }

            if (0 == files.Length)
            {
                throw new ArgumentOutOfRangeException("files");
            }

            Files = files;
        }

        private TsvDataAttribute()
        {
        }

        public IEnumerable<string> Files { get; private set; }

        public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest)
        {
            
            var parameterTypes = methodUnderTest.GetParameters().Select(p => p.ParameterType).ToArray();

            if (null == methodUnderTest)
            {
                throw new ArgumentNullException("methodUnderTest");
            }

            if (null == parameterTypes)
            {
                throw new ArgumentNullException("parameterTypes");
            }

            var list = new List<object>();
            if (1 == parameterTypes.Length && parameterTypes[0] == typeof(DataSet))
            {
                var data = new DataSet
                               {
                                   Locale = CultureInfo.InvariantCulture
                               };
                foreach (var file in Files)
                {
                    data.Tables.Add(new TsvDataSheet(file).ToDataTable(file));
                }

                list.Add(data);
            }
            else
            {
#if NET20
                if (IEnumerableExtensionMethods.Count(Files) != parameterTypes.Length)
                {
                    throw new InvalidOperationException(StringExtensionMethods.FormatWith(Resources.CountsDiffer, IEnumerableExtensionMethods.Count(Files), parameterTypes.Length));
                }
#else
                if (Files.Count() != parameterTypes.Length)
                {
                    throw new InvalidOperationException(Resources.CountsDiffer.FormatWith(Files.Count(), parameterTypes.Length));
                }
#endif

                var index = -1;
                foreach (var file in Files)
                {
                    index++;
                    if (parameterTypes[index] == typeof(TsvDataSheet) ||
                        parameterTypes[index] == typeof(IEnumerable<KeyStringDictionary>))
                    {
                        list.Add(new TsvDataSheet(file));
                        continue;
                    }

                    if (parameterTypes[index] == typeof(DataTable))
                    {
                        list.Add(new TsvDataSheet(file).ToDataTable());
                        continue;
                    }

                    throw new InvalidOperationException(Resources.UnsupportedParameterType);
                }
            }

            yield return list.ToArray();
        }
    }
}