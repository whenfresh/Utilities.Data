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

    public sealed class TsvHttpAttributeFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<TsvHttpAttribute>()
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
            Assert.NotNull(new TsvHttpAttribute("example.tsv"));
        }

        [Fact]
        public void ctor_stringsEmpty()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TsvHttpAttribute(new List<string>().ToArray()));
        }

        [Fact]
        public void ctor_stringsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TsvHttpAttribute(null));
        }

        [Fact]
        public void op_Download_AbsoluteUri()
        {
            const string expected = "A1";
            var actual = TsvHttpAttribute.Download("http://www.alan-dean.com/example.tsv").First()["A"];

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Download_AbsoluteUriNotFound()
        {
            Assert.Throws<WebException>(() => TsvHttpAttribute.Download("http://www.alan-dean.com/missing.tsv"));
        }

        [Fact]
        public void op_Download_AbsoluteUriNull()
        {
            Assert.Throws<ArgumentNullException>(() => TsvHttpAttribute.Download(null));
        }

        [Fact]
        public void op_GetData_MethodInfoNull_Types()
        {
            var obj = new TsvHttpAttribute("example.tsv");

            Assert.Throws<ArgumentNullException>(() => obj.GetData(null, new[] { typeof(TsvDataSheet) }).ToList());
        }

        [Fact]
        public void op_GetData_MethodInfo_TypesNull()
        {
            var obj = new TsvHttpAttribute("http://www.alan-dean.com/example.tsv");

            Assert.Throws<ArgumentNullException>(() => obj.GetData(GetType().GetMethod("usage"), null).ToList());
        }

        [Fact]
        public void op_GetData_MethodInfo_Types_whenInvalidParameterType()
        {
            var obj = new TsvHttpAttribute("http://www.alan-dean.com/example.tsv");

            Assert.Throws<InvalidOperationException>(() => obj.GetData(GetType().GetMethod("usage"), new[] { typeof(string) }).ToList());
        }

        [Fact]
        public void op_GetData_MethodInfo_Types_whenParameterCountMismatch()
        {
            var obj = new TsvHttpAttribute("http://www.alan-dean.com/one.tsv", "http://www.alan-dean.com/two.tsv");

            Assert.Throws<InvalidOperationException>(() => obj.GetData(GetType().GetMethod("usage"), new[] { typeof(TsvDataSheet) }).ToList());
        }

        [Fact]
        public void prop_FileName()
        {
            Assert.True(new PropertyExpectations<TsvHttpAttribute>(x => x.Locations)
                            .TypeIs<IEnumerable<string>>()
                            .IsNotDecorated()
                            .Result);
        }

        [Theory]
        [TsvHttp("http://www.alan-dean.com/example.tsv")]
        public void usage(TsvDataSheet sheet)
        {
            Assert.Equal("A1", sheet.First()["A"]);
            Assert.Equal("B2", sheet.Last()["B"]);
        }

        [Theory]
        [TsvHttp("http://www.alan-dean.com/one.tsv", "http://www.alan-dean.com/two.tsv")]
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
        [TsvHttp("http://www.alan-dean.com/example.tsv")]
        [TsvHttp("http://www.alan-dean.com/example.tsv")]
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
        [TsvHttp("http://www.alan-dean.com/example.tsv")]
        public void usage_whenIEnumerableParameter(IEnumerable<KeyStringDictionary> data)
        {
            // ReSharper disable PossibleMultipleEnumeration
            Assert.Equal("A1", data.First()["A"]);
            Assert.Equal("B2", data.Last()["B"]);

            // ReSharper restore PossibleMultipleEnumeration
        }

        [Theory]
        [TsvHttp("http://www.alan-dean.com/one.tsv", "http://www.alan-dean.com/two.tsv")]
        public void usage_whenMultipleParameters(TsvDataSheet one,
                                                 TsvDataSheet two)
        {
            Assert.Equal("one", one.First()["COLUMN"]);
            Assert.Equal("two", two.First()["COLUMN"]);
        }
    }
}