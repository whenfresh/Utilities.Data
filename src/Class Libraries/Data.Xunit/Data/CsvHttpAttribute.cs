﻿namespace WhenFresh.Utilities.Data.Xunit.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using global::Xunit.Extensions;
    using global::Xunit.Sdk;
    using WhenFresh.Utilities.Core;
    using WhenFresh.Utilities.Core.Collections;
    using WhenFresh.Utilities.Core.IO;
    using WhenFresh.Utilities.Data.Data;
    using WhenFresh.Utilities.Data.Xunit.Properties;
#if !NET20
#endif

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class CsvHttpAttribute : DataAttribute
    {
        public CsvHttpAttribute(params string[] locations)
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

        private CsvHttpAttribute()
        {
        }

        public IEnumerable<string> Locations { get; private set; }

        public static CsvDataSheet Download(AbsoluteUri location)
        {
            if (null == location)
            {
                throw new ArgumentNullException("location");
            }

            CsvDataSheet csv = null;

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
                            var file = new FileInfo(StringExtensionMethods.FormatWith("{0}.csv", AlphaDecimal.Random()));
                            FileInfoExtensionMethods.Create(file, reader.ReadToEnd());
#else
                            var file = new FileInfo("{0}.csv".FormatWith(AlphaDecimal.Random()));
                            file.Create(reader.ReadToEnd());
#endif

                            csv = new CsvDataSheet(file);
                        }
                    }
                }
            }

            return csv;
        }

        public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest)
        {
            if (null == methodUnderTest)
            {
                throw new ArgumentNullException("methodUnderTest");
            }


            var parameterTypes = methodUnderTest.GetParameters().Select(p => p.GetType()).ToArray();
            
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
                if (Locations.Count() != parameterTypes.Length)
                {
                    throw new InvalidOperationException(Resources.CountsDiffer.FormatWith(Locations.Count(), parameterTypes.Length));
                }

                var index = -1;
                foreach (var location in Locations)
                {
                    index++;
                    if (parameterTypes[index] == typeof(CsvDataSheet) ||
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