namespace WhenFresh.Utilities.Data
{
    using WhenFresh.Utilities.Testing.Unit;
    using Xunit.Sdk;

    public sealed class CsvDataAttributeFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<CsvDataAttribute>()
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
            Assert.NotNull(new CsvDataAttribute("example.csv"));
        }

        [Fact]
        public void ctor_stringsEmpty()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CsvDataAttribute(new List<string>().ToArray()));
        }

        [Fact]
        public void ctor_stringsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CsvDataAttribute(null));
        }

        [Fact]
        public void prop_FileName()
        {
            Assert.True(new PropertyExpectations<CsvDataAttribute>(x => x.Files)
                            .TypeIs<IEnumerable<string>>()
                            .IsNotDecorated()
                            .Result);
        }

        [Theory]
        [CsvData("example.csv")]
        public void usage(CsvDataSheet csv)
        {
            Assert.Equal("A1", csv.First()["A"]);
            Assert.Equal("B2", csv.Last()["B"]);
        }

        [Theory]
        [CsvData("one.csv", "two.csv")]
        public void usage_whenDataSet(DataSet data)
        {
            if (null == data)
            {
                throw new ArgumentNullException("data");
            }

            Assert.Equal("one", data.Tables["one.csv"].Rows[0].Field<string>("COLUMN"));
            Assert.Equal("two", data.Tables["two.csv"].Rows[0].Field<string>("COLUMN"));
        }

        [Theory]
        [CsvData("example.csv")]
        [CsvData("example.csv")]
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
        [CsvData("example.csv")]
        public void usage_whenIEnumerableParameter(IEnumerable<KeyStringDictionary> data)
        {
            // ReSharper disable PossibleMultipleEnumeration
            Assert.Equal("A1", data.First()["A"]);
            Assert.Equal("B2", data.Last()["B"]);

            // ReSharper restore PossibleMultipleEnumeration
        }

        [Theory]
        [CsvData("one.csv", "two.csv")]
        public void usage_whenMultipleParameters(CsvDataSheet one,
                                                 CsvDataSheet two)
        {
            Assert.Equal("one", one.First()["COLUMN"]);
            Assert.Equal("two", two.First()["COLUMN"]);
        }
    }
}