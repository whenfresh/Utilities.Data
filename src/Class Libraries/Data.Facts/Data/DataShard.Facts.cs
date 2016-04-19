namespace Cavity.Data
{
    using System.Collections.Generic;
    using Cavity.Collections;
    using Xunit;

    public sealed class DataShardFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<DataShard>()
                            .DerivesFrom<DataSheet>()
                            .IsConcreteClass()
                            .IsUnsealed()
                            .HasDefaultConstructor()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void ctor()
        {
            Assert.NotNull(new DataShard());
        }

        [Fact]
        public void ctor_string()
        {
            Assert.NotNull(new DataShard("title"));
        }

        [Fact]
        public void op_AsOfT()
        {
            var shard = new DataShard();
            shard.Entries.Add(new KeyStringDictionary
                                  {
                                      { "name", "value" }
                                  });

            foreach (var entry in shard.As<TestDataEntry>())
            {
                Assert.Equal("value", entry.Name);
            }
        }

        [Fact]
        public void op_GetEnumerator()
        {
            var shard = new DataShard();
            shard.Entries.Add(new KeyStringDictionary
                                  {
                                      { "name", "value" }
                                  });

            foreach (var entry in shard)
            {
                Assert.Equal("value", entry["name"]);
            }
        }

        [Fact]
        public void prop_Entries()
        {
            Assert.True(new PropertyExpectations<DataShard>(x => x.Entries)
                            .IsNotDecorated()
                            .TypeIs<ICollection<KeyStringDictionary>>()
                            .DefaultValueIsNotNull()
                            .Result);
        }
    }
}