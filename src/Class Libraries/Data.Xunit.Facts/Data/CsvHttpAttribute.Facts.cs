namespace Cavity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Net;
    using Cavity.Collections;
    using Xunit;
    using Xunit.Extensions;

    public sealed class CsvHttpAttributeFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<CsvHttpAttribute>()
                            .DerivesFrom<DataAttribute>()
                            .IsConcreteClass()
                            .IsSealed()
                            .NoDefaultConstructor()
                            .AttributeUsage(AttributeTargets.Method, true, true)
                            .Result);
        }

        [Fact]
        public void ctor_strings()
        {
            Assert.NotNull(new CsvHttpAttribute("example.csv"));
        }

        [Fact]
        public void ctor_stringsEmpty()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CsvHttpAttribute(new List<string>().ToArray()));
        }

        [Fact]
        public void ctor_stringsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CsvHttpAttribute(null));
        }

        [Fact]
        public void op_Download_AbsoluteUri()
        {
            const string expected = "A1";
            var actual = CsvHttpAttribute.Download("http://www.alan-dean.com/example.csv").First()["A"];

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Download_AbsoluteUriNotFound()
        {
            Assert.Throws<WebException>(() => CsvHttpAttribute.Download("http://www.alan-dean.com/missing.csv"));
        }

        [Fact]
        public void op_Download_AbsoluteUriNull()
        {
            Assert.Throws<ArgumentNullException>(() => CsvHttpAttribute.Download(null));
        }

        [Fact]
        public void op_GetData_MethodInfoNull_Types()
        {
            var obj = new CsvHttpAttribute("example.csv");

            Assert.Throws<ArgumentNullException>(() => obj.GetData(null, new[] { typeof(CsvDataSheet) }).ToList());
        }

        [Fact]
        public void op_GetData_MethodInfo_TypesNull()
        {
            var obj = new CsvHttpAttribute("http://www.alan-dean.com/example.csv");

            Assert.Throws<ArgumentNullException>(() => obj.GetData(GetType().GetMethod("usage"), null).ToList());
        }

        [Fact]
        public void op_GetData_MethodInfo_Types_whenInvalidParameterType()
        {
            var obj = new CsvHttpAttribute("http://www.alan-dean.com/example.csv");

            Assert.Throws<InvalidOperationException>(() => obj.GetData(GetType().GetMethod("usage"), new[] { typeof(string) }).ToList());
        }

        [Fact]
        public void op_GetData_MethodInfo_Types_whenParameterCountMismatch()
        {
            var obj = new CsvHttpAttribute("http://www.alan-dean.com/one.csv", "http://www.alan-dean.com/two.csv");

            Assert.Throws<InvalidOperationException>(() => obj.GetData(GetType().GetMethod("usage"), new[] { typeof(CsvDataSheet) }).ToList());
        }

        [Fact]
        public void prop_FileName()
        {
            Assert.True(new PropertyExpectations<CsvHttpAttribute>(x => x.Locations)
                            .TypeIs<IEnumerable<string>>()
                            .IsNotDecorated()
                            .Result);
        }

        [Theory]
        [CsvHttp("http://www.alan-dean.com/example.csv")]
        public void usage(CsvDataSheet sheet)
        {
            Assert.Equal("A1", sheet.First()["A"]);
            Assert.Equal("B2", sheet.Last()["B"]);
        }

        [Theory]
        [CsvHttp("http://www.alan-dean.com/one.csv", "http://www.alan-dean.com/two.csv")]
        public void usage_whenDataSet(DataSet data)
        {
            if (null == data)
            {
                throw new ArgumentNullException("data");
            }

            Assert.Equal("one", data.Tables[0].Rows[0].Field<string>("COLUMN"));
            Assert.Equal("two", data.Tables[1].Rows[0].Field<string>("COLUMN"));
        }

        [Theory]
        [CsvHttp("http://www.alan-dean.com/example.csv")]
        [CsvHttp("http://www.alan-dean.com/example.csv")]
        public void usage_whenDataTable(DataTable table)
        {
            if (null == table)
            {
                throw new ArgumentNullException("table");
            }

            Assert.Equal("A1", table.Rows[0].Field<string>("A"));
            Assert.Equal("B2", table.Rows[1].Field<string>("B"));
        }

        [Theory]
        [CsvHttp("http://www.alan-dean.com/example.csv")]
        public void usage_whenIEnumerableParameter(IEnumerable<KeyStringDictionary> data)
        {
            // ReSharper disable PossibleMultipleEnumeration
            Assert.Equal("A1", data.First()["A"]);
            Assert.Equal("B2", data.Last()["B"]);

            // ReSharper restore PossibleMultipleEnumeration
        }

        [Theory]
        [CsvHttp("http://www.alan-dean.com/one.csv", "http://www.alan-dean.com/two.csv")]
        public void usage_whenMultipleParameters(CsvDataSheet one,
                                                 CsvDataSheet two)
        {
            Assert.Equal("one", one.First()["COLUMN"]);
            Assert.Equal("two", two.First()["COLUMN"]);
        }
    }
}