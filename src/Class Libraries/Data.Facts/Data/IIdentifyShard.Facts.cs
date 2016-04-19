namespace Cavity.Data
{
    using Cavity.Collections;
    using Moq;
    using Xunit;

    public sealed class IIdentifyShardFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<IIdentifyShard>()
                            .IsInterface()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void op_IdentifyShard_KeyStringDictionary()
        {
            var entry = new KeyStringDictionary();
            const string expected = "example";
            var mock = new Mock<IIdentifyShard>();
            mock
                .Setup(x => x.IdentifyShard(entry))
                .Returns(expected)
                .Verifiable();

            var actual = mock.Object.IdentifyShard(entry);

            Assert.Equal(expected, actual);

            mock.VerifyAll();
        }
    }
}