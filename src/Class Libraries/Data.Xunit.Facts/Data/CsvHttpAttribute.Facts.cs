namespace WhenFresh.Utilities.Data
{
    using System.Net;
    using WhenFresh.Utilities.Testing.Unit;
    using Xunit.Sdk;

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

        [Fact(Skip = "www.alan-dean.com")]
        public void op_Download_AbsoluteUri()
        {
            const string expected = "A1";
            var actual = CsvHttpAttribute.Download("http://www.alan-dean.com/example.csv").First()["A"];

            Assert.Equal(expected, actual);
        }

        [Fact(Skip = "www.alan-dean.com")]
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
        public void prop_FileName()
        {
            Assert.True(new PropertyExpectations<CsvHttpAttribute>(x => x.Locations)
                            .TypeIs<IEnumerable<string>>()
                            .IsNotDecorated()
                            .Result);
        }


    }
}