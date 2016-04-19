namespace Cavity.Data
{
    using System;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Xunit;

    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "DataSheet", Justification = "This is the correct casing.")]
    public sealed class DataSheetFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<DataSheet>()
                            .DerivesFrom<object>()
                            .IsAbstractBaseClass()
                            .IsNotDecorated()
                            .Implements<IDataSheet>()
                            .Result);
        }

        [Fact]
        public void ctor()
        {
            Assert.NotNull(new DerivedDataSheet());
        }

        [Fact]
        public void op_AsOfT()
        {
            Assert.Empty(new DerivedDataSheet().As<TestDataEntry>());
        }

        [Fact]
        public void op_GetEnumerator()
        {
            Assert.NotEmpty(new DerivedDataSheet());
        }

        [Fact]
        public void op_Shard_IIdentifyShard()
        {
            var identifier = new ShardByValue("one");
            var sheet = new DerivedDataSheet();
            foreach (var shard in sheet.Shard(identifier))
            {
                Assert.Equal("1", shard.Title);
            }
        }

        [Fact]
        public void op_Shard_IIdentifyShardNull()
        {
            Assert.Throws<ArgumentNullException>(() => new DerivedDataSheet().Shard(null).ToList());
        }

        [Fact]
        public void op_ToDataTable()
        {
            var sheet = new DerivedDataSheet();
            var table = sheet.ToDataTable();
            Assert.Equal(string.Empty, table.TableName);

            foreach (DataRow row in table.Rows)
            {
                Assert.Equal("1", row.Field<string>("one"));
            }
        }

        [Fact]
        public void op_ToDataTable_string()
        {
            const string name = "Example";

            var sheet = new DerivedDataSheet();
            var table = sheet.ToDataTable(name);
            Assert.Equal(name, table.TableName);

            foreach (DataRow row in table.Rows)
            {
                Assert.Equal("1", row.Field<string>("one"));
            }
        }

        [Fact]
        public void op_TransformOfT_ITransformDataOfT()
        {
            foreach (var value in new DerivedDataSheet().Transform(new TestAdditionTransformer()))
            {
                Assert.Equal(3, value);
            }
        }

        [Fact]
        public void op_TransformOfT_ITransformDataOfTNull()
        {
            Assert.Throws<ArgumentNullException>(() => new DerivedDataSheet().Transform<bool>(null));
        }

        [Fact]
        public void op_Transform_ITransformData()
        {
            foreach (var entry in new DerivedDataSheet().Transform(new TestConcatenationTransformer()))
            {
                Assert.Equal("12", entry["CONCAT"]);
            }
        }

        [Fact]
        public void op_Transform_ITransformDataNull()
        {
            Assert.Throws<ArgumentNullException>(() => new DerivedDataSheet().Transform(null));
        }

        [Fact]
        public void prop_Title()
        {
            Assert.True(new PropertyExpectations<DataSheet>(x => x.Title)
                            .IsNotDecorated()
                            .TypeIs<string>()
                            .Result);
        }
    }
}