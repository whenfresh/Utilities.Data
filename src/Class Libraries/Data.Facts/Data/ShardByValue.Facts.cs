namespace Cavity.Data
{
    using System;
    using System.Collections.Generic;
    using Cavity.Collections;
    using Xunit;
    using Xunit.Extensions;

    public sealed class ShardByValueFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<ShardByValue>()
                            .DerivesFrom<object>()
                            .IsConcreteClass()
                            .IsSealed()
                            .NoDefaultConstructor()
                            .IsNotDecorated()
                            .Implements<IIdentifyShard>()
                            .Result);
        }

        [Fact]
        public void ctor_string()
        {
            Assert.NotNull(new ShardByValue("example"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void ctor_stringEmpty(string value)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ShardByValue(value));
        }

        [Fact]
        public void ctor_stringNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ShardByValue(null));
        }

        [Theory]
        [InlineData("", null)]
        [InlineData("", "")]
        [InlineData("", "   ")]
        [InlineData("value", "value")]
        [InlineData("value", " value ")]
        public void op_IdentifyShard_KeyStringDictionary(string expected,
                                                         string value)
        {
            var obj = new ShardByValue("example");
            var entry = new KeyStringDictionary
                            {
                                { "example", value }
                            };

            var actual = obj.IdentifyShard(entry);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_IdentifyShard_KeyStringDictionaryNull()
        {
            var obj = new ShardByValue("example");

            Assert.Throws<ArgumentNullException>(() => obj.IdentifyShard(null));
        }

        [Fact]
        public void op_IdentifyShard_KeyStringDictionary_whenColumnMissing()
        {
            const string expected = "value";
            var obj = new ShardByValue("foo");
            var entry = new KeyStringDictionary
                            {
                                { "bar", expected }
                            };

            Assert.Throws<KeyNotFoundException>(() => obj.IdentifyShard(entry));
        }

        [Fact]
        public void prop_Column()
        {
            Assert.True(new PropertyExpectations<ShardByValue>(x => x.Column)
                            .TypeIs<string>()
                            .IsNotDecorated()
                            .Result);
        }
    }
}