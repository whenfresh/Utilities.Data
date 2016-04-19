namespace Cavity.IO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Cavity.Collections;
    using Cavity.Data;
    using Xunit;

    public sealed class TempCsvFileFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<TempCsvFile>()
                            .DerivesFrom<TempFile>()
                            .IsConcreteClass()
                            .IsSealed()
                            .NoDefaultConstructor()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void ctor_IEnumerableOfKeyStringDictionary()
        {
            Assert.NotNull(new TempCsvFile(new List<KeyStringDictionary>()));
        }

        [Fact]
        public void ctor_IEnumerableOfKeyStringDictionaryNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TempCsvFile(null));
        }

        [Fact]
        public void op_Create_IEnumerableOfKeyStringDictionaryEmpty_FileInfo()
        {
            using (var temp = new TempDirectory())
            {
                var data = new List<KeyStringDictionary>();
                var destination = temp.Info.ToFile("destination");

                const int expected = 0;
                var actual = TempCsvFile.Create(data, destination);

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void op_Create_IEnumerableOfKeyStringDictionaryNull_FileInfo()
        {
            using (var temp = new TempDirectory())
            {
                var destination = temp.Info.ToFile("destination");

                Assert.Throws<ArgumentNullException>(() => TempCsvFile.Create(null, destination));
            }
        }

        [Fact]
        public void op_Create_IEnumerableOfKeyStringDictionary_FileInfo()
        {
            using (var temp = new TempDirectory())
            {
                var destination = temp.Info.ToFile("destination");
                var data = new List<KeyStringDictionary>
                               {
                                   new KeyStringDictionary
                                       {
                                           { "VALUE", "123" }
                                       }
                               };

                var count = TempCsvFile.Create(data, destination);

                Assert.True(temp.Info.ToFile("destination").Exists);
                Assert.Equal(1, count);
                Assert.Equal("123", new CsvDataSheet(destination).First()["VALUE"]);
            }
        }

        [Fact]
        public void op_Create_IEnumerableOfKeyStringDictionary_FileInfoNull()
        {
            var data = new List<KeyStringDictionary>
                           {
                               new KeyStringDictionary
                                   {
                                       { "VALUE", "123" }
                                   }
                           };

            Assert.Throws<ArgumentNullException>(() => TempCsvFile.Create(data, null));
        }
    }
}