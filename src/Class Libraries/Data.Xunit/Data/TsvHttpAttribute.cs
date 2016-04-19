namespace Cavity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
#if !NET20
    using System.Linq;
#endif
    using System.Net;
    using System.Reflection;
    using Cavity.Collections;
    using Cavity.IO;
    using Cavity.Properties;
    using Xunit.Extensions;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class TsvHttpAttribute : DataAttribute
    {
        public TsvHttpAttribute(params string[] locations)
            : this()
        {
            if (null == locations)
            {
                throw new ArgumentNullException("locations");
            }

            if (0 == locations.Length)
            {
                throw new ArgumentOutOfRangeException("locations");
            }

            Locations = locations;
        }

        private TsvHttpAttribute()
        {
        }

        public IEnumerable<string> Locations { get; private set; }

        public static TsvDataSheet Download(AbsoluteUri location)
        {
            if (null == location)
            {
                throw new ArgumentNullException("location");
            }

            TsvDataSheet tsv = null;

            var request = WebRequest.Create((Uri)location);
            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (null != stream)
                    {
                        using (var reader = new StreamReader(stream))
                        {
#if NET20
                            var file = new FileInfo(StringExtensionMethods.FormatWith("{0}.tsv", AlphaDecimal.Random()));
                            FileInfoExtensionMethods.Create(file, reader.ReadToEnd());
#else
                            var file = new FileInfo("{0}.tsv".FormatWith(AlphaDecimal.Random()));
                            file.Create(reader.ReadToEnd());
#endif

                            tsv = new TsvDataSheet(file);
                        }
                    }
                }
            }

            return tsv;
        }

        public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest,
                                                      Type[] parameterTypes)
        {
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
                foreach (var location in Locations)
                {
                    data.Tables.Add(Download(location).ToDataTable());
                }

                list.Add(data);
            }
            else
            {
#if NET20
                if (IEnumerableExtensionMethods.Count(Locations) != parameterTypes.Length)
                {
                    throw new InvalidOperationException(StringExtensionMethods.FormatWith(Resources.CountsDiffer, IEnumerableExtensionMethods.Count(Locations), parameterTypes.Length));
                }
#else
                if (Locations.Count() != parameterTypes.Length)
                {
                    throw new InvalidOperationException(Resources.CountsDiffer.FormatWith(Locations.Count(), parameterTypes.Length));
                }
#endif

                var index = -1;
                foreach (var location in Locations)
                {
                    index++;
                    if (parameterTypes[index] == typeof(TsvDataSheet) ||
                        parameterTypes[index] == typeof(IEnumerable<KeyStringDictionary>))
                    {
                        list.Add(Download(location));
                        continue;
                    }

                    if (parameterTypes[index] == typeof(DataTable))
                    {
                        list.Add(Download(location).ToDataTable());
                        continue;
                    }

                    throw new InvalidOperationException(Resources.UnsupportedParameterType);
                }
            }

            yield return list.ToArray();
        }
    }
}