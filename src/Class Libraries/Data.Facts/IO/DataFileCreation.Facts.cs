namespace Cavity.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using Cavity.Collections;
    using Cavity.Data;
    using Xunit;

    public sealed class DataFileCreationFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<DataFileCreation>()
                            .DerivesFrom<object>()
                            .IsConcreteClass()
                            .IsUnsealed()
                            .NoDefaultConstructor()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void ctor_FileInfoNull_DateTime()
        {
            Assert.Throws<ArgumentNullException>(() => new DataFileCreation(null, DateTime.UtcNow));
        }

        [Fact]
        public void ctor_FileInfo_DateTime()
        {
            using (var temp = new TempDirectory())
            {
                var destination = temp.Info.ToFile("destination");
                var modified = DateTime.UtcNow;

                var obj = new DataFileCreation(destination, modified);

                Assert.Equal(destination, obj.Destination);
                Assert.Equal(modified, obj.Modified);
            }
        }

        [Fact]
        public void op_Create_IEnumerableOfKeyStringDictionary()
        {
            using (var temp = new TempDirectory())
            {
                var destination = temp.Info.ToFile("destination");
                var modified = DateTime.UtcNow;
                var data = new List<KeyStringDictionary>
                               {
                                   new KeyStringDictionary
                                       {
                                           { "VALUE", "123" }
                                       }
                               };

                var obj = new DataFileCreation(destination, modified);
                var count = obj.Create(data);

                Assert.True(temp.Info.ToFile("destination").Exists);
                Assert.Equal(1, count);
                Assert.Equal(count, obj.Count);
                Assert.Equal("123", new CsvDataSheet(destination).First()["VALUE"]);
            }
        }

        [Fact]
        public void op_Create_IEnumerableOfKeyStringDictionaryNull()
        {
            using (var temp = new TempDirectory())
            {
                var destination = temp.Info.ToFile("destination");
                var modified = DateTime.UtcNow;

                Assert.Throws<ArgumentNullException>(() => new DataFileCreation(destination, modified).Create(null));
            }
        }

        [Fact]
        public void op_Create_IEnumerableOfKeyStringDictionary_whenDestinationExists()
        {
            using (var temp = new TempDirectory())
            {
                var destination = temp.Info.ToFile("destination").Create("test");
                var modified = DateTime.UtcNow;
                var data = new List<KeyStringDictionary>();

                Assert.Throws<IOException>(() => new DataFileCreation(destination, modified).Create(data));
            }
        }

        [Fact]
        public void prop_Count()
        {
            Assert.True(new PropertyExpectations<DataFileCreation>(x => x.Count)
                            .IsNotDecorated()
                            .TypeIs<int>()
                            .Result);
        }

        [Fact]
        public void prop_Destination()
        {
            Assert.True(new PropertyExpectations<DataFileCreation>(x => x.Destination)
                            .IsNotDecorated()
                            .TypeIs<FileInfo>()
                            .Result);
        }

        [Fact]
        public void prop_Modified()
        {
            Assert.True(new PropertyExpectations<DataFileCreation>(x => x.Modified)
                            .IsNotDecorated()
                            .TypeIs<DateTime>()
                            .Result);
        }

        [Fact]
        public void prop_Stopwatch()
        {
            Assert.True(new PropertyExpectations<DataFileCreation>(x => x.Stopwatch)
                            .IsNotDecorated()
                            .TypeIs<Stopwatch>()
                            .Result);
        }
    }
}