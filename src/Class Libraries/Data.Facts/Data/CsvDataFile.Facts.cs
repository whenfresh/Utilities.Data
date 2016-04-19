namespace Cavity.Data
{
    using System;
    using System.IO;
    using System.Linq;
    using Cavity.IO;
    using Xunit;

    public sealed class CsvDataFileFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<CsvDataFile>()
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
                Assert.NotNull(new CsvDataFile(temp.Info));
            }
        }

        [Fact]
        public void ctor_FileInfoMissing()
        {
            using (var temp = new TempDirectory())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<FileNotFoundException>(() => new CsvDataFile(temp.Info.ToFile("missing.txt")));
                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void ctor_FileInfoNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CsvDataFile(null));
        }

        [Fact]
        public void op_IEnumerable_GetEnumerator()
        {
            using (var temp = new TempDirectory())
            {
                var file = temp
                    .Info
                    .ToFile("Data.csv")
                    .AppendLine("name")
                    .AppendLine("Example");
                var sheet = new CsvDataFile(file).First();

                Assert.Equal("Data", sheet.Title);
                Assert.Equal("Example", sheet.First()["name"]);
            }
        }
    }
}