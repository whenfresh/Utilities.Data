namespace Cavity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Cavity.Collections;
    using Xunit;
    using Xunit.Extensions;

    public sealed class TsvDataAttributeFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<TsvDataAttribute>()
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
            Assert.NotNull(new TsvDataAttribute("example.tsv"));
        }

        [Fact]
        public void ctor_stringsEmpty()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new TsvDataAttribute(new List<string>().ToArray()));
        }

        [Fact]
        public void ctor_stringsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TsvDataAttribute(null));
        }

        [Fact]
        public void op_GetData_MethodInfoNull_Types()
        {
            var obj = new TsvDataAttribute("example.tsv");

            Assert.Throws<ArgumentNullException>(() => obj.GetData(null, new[] { typeof(TsvDataSheet) }).ToList());
        }

        [Fact]
        public void op_GetData_MethodInfo_TypesNull()
        {
            var obj = new TsvDataAttribute("example.tsv");

            Assert.Throws<ArgumentNullException>(() => obj.GetData(GetType().GetMethod("usage"), null).ToList());
        }

        [Fact]
        public void op_GetData_MethodInfo_Types_whenInvalidParameterType()
        {
            var obj = new TsvDataAttribute("example.tsv");

            Assert.Throws<InvalidOperationException>(() => obj.GetData(GetType().GetMethod("usage"), new[] { typeof(string) }).ToList());
        }

        [Fact]
        public void op_GetData_MethodInfo_Types_whenParameterCountMismatch()
        {
            var obj = new TsvDataAttribute("one.tsv", "two.tsv");

            Assert.Throws<InvalidOperationException>(() => obj.GetData(GetType().GetMethod("usage"), new[] { typeof(TsvDataSheet) }).ToList());
        }

        [Fact]
        public void prop_FileName()
        {
            Assert.True(new PropertyExpectations<TsvDataAttribute>(x => x.Files)
                            .TypeIs<IEnumerable<string>>()
                            .IsNotDecorated()
                            .Result);
        }

        [Theory]
        [TsvData("example.tsv")]
        public void usage(TsvDataSheet tsv)
        {
            Assert.Equal("A1", tsv.First()["A"]);
            Assert.Equal("B2", tsv.Last()["B"]);
        }

        [Theory]
        [TsvData("one.tsv", "two.tsv")]
        public void usage_whenDataSet(DataSet data)
        {
            if (null == data)
            {
                throw new ArgumentNullException("data");
            }

            Assert.Equal("one", data.Tables["one.tsv"].Rows[0].Field<string>("COLUMN"));
            Assert.Equal("two", data.Tables["two.tsv"].Rows[0].Field<string>("COLUMN"));
        }

        [Theory]
        [TsvData("example.tsv")]
        [TsvData("example.tsv")]
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
        [TsvData("example.tsv")]
        public void usage_whenIEnumerableParameter(IEnumerable<KeyStringDictionary> data)
        {
            // ReSharper disable PossibleMultipleEnumeration
            Assert.Equal("A1", data.First()["A"]);
            Assert.Equal("B2", data.Last()["B"]);

            // ReSharper restore PossibleMultipleEnumeration
        }

        [Theory]
        [TsvData("one.tsv", "two.tsv")]
        public void usage_whenMultipleParameters(TsvDataSheet one,
                                                 TsvDataSheet two)
        {
            Assert.Equal("one", one.First()["COLUMN"]);
            Assert.Equal("two", two.First()["COLUMN"]);
        }
    }
}