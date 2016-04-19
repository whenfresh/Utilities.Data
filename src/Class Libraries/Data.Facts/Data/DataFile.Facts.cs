namespace Cavity.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Cavity.IO;
    using Xunit;
    using Xunit.Extensions;

    public sealed class DataFileFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<DataFile>()
                            .DerivesFrom<object>()
                            .IsAbstractBaseClass()
                            .IsNotDecorated()
                            .Implements<IEnumerable<IDataSheet>>()
                            .Result);
        }

        [Fact]
        public void ctor_FileInfo()
        {
            using (var temp = new TempFile())
            {
                Assert.NotNull(new DerivedDataFile(temp.Info));
            }
        }

        [Fact]
        public void ctor_FileInfoMissing()
        {
            using (var temp = new TempDirectory())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<FileNotFoundException>(() => new DerivedDataFile(temp.Info.ToFile("missing.txt")));

                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void ctor_FileInfoNull()
        {
            Assert.Throws<ArgumentNullException>(() => new DerivedDataFile(null));
        }

        [Fact]
        public void opIndexer_int()
        {
            using (var temp = new TempFile())
            {
                var expected = new DerivedDataSheet();
                var data = new DerivedDataFile(temp.Info);
                data.Sheets.Add(expected);

                var actual = data[0];

                Assert.Same(expected, actual);
            }
        }

        [Fact]
        public void opIndexer_intInvalid()
        {
            using (var temp = new TempFile())
            {
                var data = new DerivedDataFile(temp.Info);
                data.Sheets.Add(new DerivedDataSheet());

                Assert.Throws<IndexOutOfRangeException>(() => data[1]);
            }
        }

        [Fact]
        public void opIndexer_intNegative()
        {
            using (var temp = new TempFile())
            {
                Assert.Throws<IndexOutOfRangeException>(() => new DerivedDataFile(temp.Info)[0]);
            }
        }

        [Fact]
        public void opIndexer_string()
        {
            using (var temp = new TempFile())
            {
                const string title = "Example";
                var expected = new DerivedDataSheet
                                   {
                                       Title = title
                                   };
                var data = new DerivedDataFile(temp.Info);
                data.Sheets.Add(expected);

                var actual = data[title];

                Assert.Same(expected, actual);
            }
        }

        [Fact]
        public void opIndexer_stringInvalid()
        {
            using (var temp = new TempFile())
            {
                var data = new DerivedDataFile(temp.Info);
                data.Sheets.Add(new DerivedDataSheet());

                Assert.Throws<KeyNotFoundException>(() => data["Example"]);
            }
        }

        [Fact]
        public void op_GetEnumerator()
        {
            using (var temp = new TempFile())
            {
                Assert.Empty(new DerivedDataFile(temp.Info));
            }
        }

        [Fact]
        public void prop_Info()
        {
            Assert.True(new PropertyExpectations<DataFile>(x => x.Info)
                            .IsNotDecorated()
                            .TypeIs<FileInfo>()
                            .Result);
        }

        [Fact]
        public void prop_Info_get()
        {
            using (var temp = new TempFile())
            {
                var expected = temp.Info;
                var actual = new DerivedDataFile(expected).Info;

                Assert.Same(expected, actual);
            }
        }

        [Fact]
        public void prop_Title()
        {
            Assert.True(new PropertyExpectations<DataFile>(x => x.Title)
                            .IsNotDecorated()
                            .TypeIs<string>()
                            .Result);
        }

        [Theory]
        [InlineData("Data", "Data")]
        [InlineData("Data", "Data.example")]
        [InlineData("", ".example")]
        public void prop_Title_get(string expected,
                                   string fileName)
        {
            using (var temp = new TempDirectory())
            {
                var file = temp.Info.ToFile(fileName).AppendLine(string.Empty);
                var actual = new DerivedDataFile(file).Title;

                Assert.Equal(expected, actual);
            }
        }
    }
}