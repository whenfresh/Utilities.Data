namespace Cavity.Data
{
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using Cavity.Collections;
    using Moq;
    using Xunit;

    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "DataSheet", Justification = "This is the correct casing.")]
    public sealed class IDataSheetFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<IDataSheet>()
                            .IsInterface()
                            .IsNotDecorated()
                            .Implements<IEnumerable<KeyStringDictionary>>()
                            .Result);
        }

        [Fact]
        public void op_AsOfT()
        {
            var expected = new Mock<IEnumerable<TestDataEntry>>().Object;

            var mock = new Mock<IDataSheet>();
            mock
                .Setup(x => x.As<TestDataEntry>())
                .Returns(expected)
                .Verifiable();

            var actual = mock.Object.As<TestDataEntry>();

            Assert.Same(expected, actual);

            mock.VerifyAll();
        }

        [Fact]
        public void op_Shard_IIdentifyShard()
        {
            var identifier = new Mock<IIdentifyShard>().Object;
            var expected = new Mock<IEnumerable<DataShard>>().Object;

            var mock = new Mock<IDataSheet>();
            mock
                .Setup(x => x.Shard(identifier))
                .Returns(expected)
                .Verifiable();

            var actual = mock.Object.Shard(identifier);

            Assert.Same(expected, actual);

            mock.VerifyAll();
        }

        [Fact]
        public void op_ToDataTable()
        {
            var expected = new DataTable
                               {
                                   Locale = CultureInfo.InvariantCulture
                               };

            var mock = new Mock<IDataSheet>();
            mock
                .Setup(x => x.ToDataTable())
                .Returns(expected)
                .Verifiable();

            var actual = mock.Object.ToDataTable();

            Assert.Same(expected, actual);

            mock.VerifyAll();
        }

        [Fact]
        public void op_ToDataTable_string()
        {
            const string name = "Example";
            var expected = new DataTable
                               {
                                   Locale = CultureInfo.InvariantCulture
                               };

            var mock = new Mock<IDataSheet>();
            mock
                .Setup(x => x.ToDataTable(name))
                .Returns(expected)
                .Verifiable();

            var actual = mock.Object.ToDataTable(name);

            Assert.Same(expected, actual);

            mock.VerifyAll();
        }

        [Fact]
        public void op_Transform_ITransformData()
        {
            var transformer = new Mock<ITransformData>().Object;
            var expected = new Mock<IEnumerable<KeyStringDictionary>>().Object;

            var mock = new Mock<IDataSheet>();
            mock
                .Setup(x => x.Transform(transformer))
                .Returns(expected)
                .Verifiable();

            var actual = mock.Object.Transform(transformer);

            Assert.Same(expected, actual);

            mock.VerifyAll();
        }

        [Fact]
        public void op_Transform_ITransformDataOfT()
        {
            var transformer = new Mock<ITransformData<bool>>().Object;
            var expected = new Mock<IEnumerable<bool>>().Object;

            var mock = new Mock<IDataSheet>();
            mock
                .Setup(x => x.Transform(transformer))
                .Returns(expected)
                .Verifiable();

            var actual = mock.Object.Transform(transformer);

            Assert.Same(expected, actual);

            mock.VerifyAll();
        }

        [Fact]
        public void prop_Title_get()
        {
            const string expected = "Example";

            var mock = new Mock<IDataSheet>();
            mock
                .Setup(x => x.Title)
                .Returns(expected)
                .Verifiable();

            var actual = mock.Object.Title;

            Assert.Equal(expected, actual);

            mock.VerifyAll();
        }

        [Fact]
        public void prop_Title_set()
        {
            const string expected = "Example";

            var mock = new Mock<IDataSheet>();
            mock
                .SetupSet(x => x.Title = expected)
                .Verifiable();

            mock.Object.Title = expected;

            mock.VerifyAll();
        }
    }
}