﻿namespace WhenFresh.Utilities.Data
{
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;
    using WhenFresh.Utilities.Core.IO;

    [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "DataSheet", Justification = "This is the correct casing.")]
    public sealed class TsvDataSheetFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<TsvDataSheet>()
                            .DerivesFrom<DataSheet>()
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
                Assert.NotNull(new TsvDataSheet(temp.Info));
            }
        }

        [Fact]
        public void ctor_FileInfoMissing()
        {
            using (var temp = new TempDirectory())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<FileNotFoundException>(() => new TsvDataSheet(temp.Info.ToFile("missing.txt")));
                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void ctor_FileInfoNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TsvDataSheet(null as FileInfo));
        }

        [Fact]
        public void ctor_string()
        {
            using (var temp = new TempFile())
            {
                Assert.NotNull(new TsvDataSheet(temp.Info.FullName));
            }
        }

        [Fact]
        public void op_IEnumerable_GetEnumerator()
        {
            using (var file = new TempFile())
            {
                file.Info.AppendLine("name");
                file.Info.AppendLine("Example");
                IEnumerable enumerable = new TsvDataSheet(file.Info);
                foreach (var entry in enumerable.Cast<KeyStringDictionary>())
                {
                    Assert.Equal("Example", entry["name"]);
                }
            }
        }

        [Fact]
        public void prop_Info()
        {
            Assert.True(new PropertyExpectations<TsvDataSheet>(x => x.Info)
                            .IsNotDecorated()
                            .TypeIs<FileInfo>()
                            .Result);
        }
    }
}