namespace Cavity.Data
{
    using System;
    using System.IO;
    using System.Linq;
    using Cavity.IO;
    using Xunit;

    public sealed class TsvDataFileFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<TsvDataFile>()
                            .DerivesFrom<DataFile>()
                            .IsConcreteClass()
                            .IsUnsealed()
                            .NoDefaultConstructor()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void ctor_FileInfo()
        {
            using (var temp = new TempFile())
            {
                Assert.NotNull(new TsvDataFile(temp.Info));
            }
        }

        [Fact]
        public void ctor_FileInfoMissing()
        {
            using (var temp = new TempDirectory())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<FileNotFoundException>(() => new TsvDataFile(temp.Info.ToFile("missing.txt")));
                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void ctor_FileInfoNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TsvDataFile(null));
        }

        [Fact]
        public void op_IEnumerable_GetEnumerator()
        {
            using (var temp = new TempDirectory())
            {
                var file = temp
                    .Info
                    .ToFile("Data.tsv")
                    .AppendLine("name")
                    .AppendLine("Example");
                var sheet = new TsvDataFile(file).First();

                Assert.Equal("Data", sheet.Title);
                Assert.Equal("Example", sheet.First()["name"]);
            }
        }
    }
}