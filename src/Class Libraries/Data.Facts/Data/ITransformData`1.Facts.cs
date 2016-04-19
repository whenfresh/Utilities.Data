namespace Cavity.Data
{
    using System.Collections.Generic;
    using Cavity.Collections;
    using Moq;
    using Xunit;

    public sealed class ITransformDataOfTFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<ITransformData<string>>()
                            .IsInterface()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void op_Transform_IEnumerableOfKeyStringDictionary()
        {
            var data = new Mock<IEnumerable<KeyStringDictionary>>().Object;
            var expected = new Mock<IEnumerable<bool>>().Object;

            var mock = new Mock<ITransformData<bool>>();
            mock
                .Setup(x => x.Transform(data))
                .Returns(expected)
                .Verifiable();

            var actual = mock.Object.Transform(data);

            Assert.Same(expected, actual);

            mock.VerifyAll();
        }
    }
}