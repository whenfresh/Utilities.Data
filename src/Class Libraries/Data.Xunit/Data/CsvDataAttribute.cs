﻿namespace WhenFresh.Utilities.Data;

using System.Data;
using System.Globalization;
using System.Reflection;
using WhenFresh.Utilities.Collections;
using WhenFresh.Utilities.Properties;
using Xunit.Sdk;
#if !NET20
#endif

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class CsvDataAttribute : DataAttribute
{
    public CsvDataAttribute(params string[] files)
        : this()
    {
        if (null == files)
            throw new ArgumentNullException("files");

        if (0 == files.Length)
            throw new ArgumentOutOfRangeException("files");

        Files = files;
    }

    private CsvDataAttribute()
    {
    }

    public IEnumerable<string> Files { get; }

    public override IEnumerable<object[]> GetData(MethodInfo methodUnderTest)
    {
        var parameterTypes = methodUnderTest.GetParameters().Select(p => p.ParameterType).ToArray();

        if (null == methodUnderTest)
            throw new ArgumentNullException("methodUnderTest");

        if (null == parameterTypes)
            throw new ArgumentNullException("parameterTypes");

        var list = new List<object>();
        if (1 == parameterTypes.Length && parameterTypes[0] == typeof(DataSet))
        {
            var data = new DataSet
                           {
                               Locale = CultureInfo.InvariantCulture
                           };
            foreach (var file in Files)
                data.Tables.Add(new CsvDataSheet(file).ToDataTable(file));

            list.Add(data);
        }
        else
        {
            if (Files.Count() != parameterTypes.Length)
                throw new InvalidOperationException(Resources.CountsDiffer.FormatWith(Files.Count(), parameterTypes.Length));

            var index = -1;
            foreach (var file in Files)
            {
                index++;
                if (parameterTypes[index] == typeof(CsvDataSheet) ||
                    parameterTypes[index] == typeof(IEnumerable<KeyStringDictionary>))
                {
                    list.Add(new CsvDataSheet(file));
                    continue;
                }

                if (parameterTypes[index] == typeof(DataTable))
                {
                    list.Add(new CsvDataSheet(file).ToDataTable());
                    continue;
                }

                throw new InvalidOperationException(Resources.UnsupportedParameterType);
            }
        }

        yield return list.ToArray();
    }
}