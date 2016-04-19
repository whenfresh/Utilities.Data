namespace Cavity.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Cavity.Collections;
    using Cavity.IO;
    using Xunit;

    public sealed class TsvFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(typeof(Tsv).IsStatic());
        }

        [Fact]
        public void op_Header_KeyStringDictionary()
        {
            var obj = new KeyStringDictionary
                          {
                              new KeyStringPair("A", string.Empty),
                              new KeyStringPair("B", string.Empty)
                          };

            const string expected = "A\tB";
            var actual = Tsv.Header(obj);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Header_KeyStringDictionaryEmpty()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Tsv.Header(new KeyStringDictionary()));
        }

        [Fact]
        public void op_Header_KeyStringDictionaryNull()
        {
            Assert.Throws<ArgumentNullException>(() => Tsv.Header(null));
        }

        [Fact]
        public void op_Header_KeyStringDictionary_whenEmptyValue()
        {
            var obj = new KeyStringDictionary
                          {
                              new KeyStringPair(string.Empty, "x"),
                              new KeyStringPair("ABC", "x")
                          };

            const string expected = "\tABC";
            var actual = Tsv.Header(obj);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Line_IEnumerableString()
        {
            var obj = new List<string>
                          {
                              "123",
                              "ABC"
                          };

            const string expected = "123\tABC";
            var actual = Tsv.Line(obj);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Line_IEnumerableStringEmpty()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Tsv.Line(new List<string>()));
        }

        [Fact]
        public void op_Line_IEnumerableStringNull()
        {
            Assert.Throws<ArgumentNullException>(() => Tsv.Line(null as IEnumerable<string>));
        }

        [Fact]
        public void op_Line_IEnumerableString_whenEmptyValue()
        {
            var obj = new List<string>
                          {
                              string.Empty,
                              "ABC"
                          };

            const string expected = "\tABC";
            var actual = Tsv.Line(obj);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Line_KeyStringDictionary()
        {
            var obj = new KeyStringDictionary
                          {
                              new KeyStringPair("A", "123"),
                              new KeyStringPair("B", "XYZ")
                          };

            const string expected = "123\tXYZ";
            var actual = Tsv.Line(obj);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Line_KeyStringDictionaryEmpty()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Tsv.Line(new KeyStringDictionary()));
        }

        [Fact]
        public void op_Line_KeyStringDictionaryNull()
        {
            Assert.Throws<ArgumentNullException>(() => Tsv.Line(null as KeyStringDictionary));
        }

        [Fact]
        public void op_Line_KeyStringDictionaryNull_IListOfString()
        {
            var columns = new List<string>
                              {
                                  "A"
                              };
            Assert.Throws<ArgumentNullException>(() => Tsv.Line(null, columns));
        }

        [Fact]
        public void op_Line_KeyStringDictionary_IListOfString()
        {
            var obj = new KeyStringDictionary
                          {
                              new KeyStringPair("A", "123"),
                              new KeyStringPair("B", "ignore"),
                              new KeyStringPair("C", "XYZ")
                          };

            const string expected = "123\tXYZ";
            var actual = Tsv.Line(obj, "A|C".Split('|').ToList());

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Line_KeyStringDictionary_IListOfStringEmpty()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Tsv.Line(new KeyStringDictionary(), new List<string>()));
        }

        [Fact]
        public void op_Line_KeyStringDictionary_IListOfStringNull()
        {
            Assert.Throws<ArgumentNullException>(() => Tsv.Line(new KeyStringDictionary(), null as IList<string>));
        }

        [Fact]
        public void op_Line_KeyStringDictionary_IListOfString_whenColumnsNotFound()
        {
            Assert.Throws<KeyNotFoundException>(() => Tsv.Line(new KeyStringDictionary(), "A,B".Split(',').ToList()));
        }

        [Fact]
        public void op_Line_KeyStringDictionary_whenEmptyValue()
        {
            var obj = new KeyStringDictionary
                          {
                              new KeyStringPair("A", string.Empty),
                              new KeyStringPair("B", "XYZ")
                          };

            const string expected = "\tXYZ";
            var actual = Tsv.Line(obj);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Save_DataTableEmpty_FileInfo_FileMode()
        {
            var table = new DataTable
                            {
                                Locale = CultureInfo.InvariantCulture
                            };

            using (var temp = new TempDirectory())
            {
                var file = temp.Info.ToFile("test.tsv");

                Tsv.Save(table, file, FileMode.Create);

                file.Refresh();
                Assert.False(file.Exists);
            }
        }

        [Fact]
        public void op_Save_DataTableEmpty_FileInfo_whenCreateNew()
        {
            var table = new DataTable
                            {
                                Locale = CultureInfo.InvariantCulture
                            };

            using (var temp = new TempDirectory())
            {
                var file = temp.Info.ToFile("test.tsv");

                Csv.Save(table, file);

                file.Refresh();
                Assert.False(file.Exists);
            }
        }

        [Fact]
        public void op_Save_DataTableNull_FileInfo()
        {
            using (var file = new TempFile())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<ArgumentNullException>(() => Csv.Save(null as DataTable, file.Info));
                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void op_Save_DataTableNull_FileInfo_FileMode()
        {
            using (var file = new TempFile())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<ArgumentNullException>(() => Csv.Save(null as DataTable, file.Info, FileMode.Append));
                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void op_Save_DataTable_FileInfoNull()
        {
            var table = new DataTable
                            {
                                Locale = CultureInfo.InvariantCulture
                            };

            Assert.Throws<ArgumentNullException>(() => Csv.Save(table, null));
        }

        [Fact]
        public void op_Save_DataTable_FileInfoNull_FileMode()
        {
            var table = new DataTable
                            {
                                Locale = CultureInfo.InvariantCulture
                            };

            Assert.Throws<ArgumentNullException>(() => Csv.Save(table, null, FileMode.Append));
        }

        [Fact]
        public void op_Save_DataTable_FileInfo_FileMode()
        {
            var table = new DataTable
                            {
                                Locale = CultureInfo.InvariantCulture
                            };
            table.Columns.Add("A");
            table.Columns.Add("B");
            var row = table.NewRow();
            row["A"] = "1";
            row["B"] = "2";
            table.Rows.Add(row);

            using (var temp = new TempDirectory())
            {
                var file = temp.Info.ToFile("example.tsv");

                Tsv.Save(table, file, FileMode.Create);

                var expected = "A\tB{0}1\t2{0}".FormatWith(Environment.NewLine);
                var actual = file.ReadToEnd();

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void op_Save_DataTable_FileInfo_whenAppend()
        {
            var table = new DataTable
                            {
                                Locale = CultureInfo.InvariantCulture
                            };
            table.Columns.Add("A");
            table.Columns.Add("B");
            var row = table.NewRow();
            row["A"] = "1";
            row["B"] = "2";
            table.Rows.Add(row);

            using (var temp = new TempDirectory())
            {
                var file = temp.Info.ToFile("example.tsv").AppendLine("A\tB");

                Tsv.Save(table, file);

                var expected = "A\tB{0}1\t2{0}".FormatWith(Environment.NewLine);
                var actual = file.ReadToEnd();

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionaryEmpty_FileInfo()
        {
            using (var temp = new TempDirectory())
            {
                var file = temp.Info.ToFile("test.tsv");
                file.CreateNew();

                Tsv.Save(new List<KeyStringDictionary>(), file);

                file.Refresh();
                Assert.False(file.Exists);
            }
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionaryEmpty_FileInfo_FileMode()
        {
            using (var temp = new TempDirectory())
            {
                var file = temp.Info.ToFile("test.tsv");
                file.CreateNew();

                Tsv.Save(new List<KeyStringDictionary>(), file, FileMode.Create);

                file.Refresh();
                Assert.False(file.Exists);
            }
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionaryNull_FileInfo()
        {
            using (var file = new TempFile())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<ArgumentNullException>(() => Tsv.Save(null as IEnumerable<KeyStringDictionary>, file.Info));
                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionaryNull_FileInfo_FileMode()
        {
            using (var file = new TempFile())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<ArgumentNullException>(() => Tsv.Save(null as IEnumerable<KeyStringDictionary>, file.Info, FileMode.Append));
                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionaryNull_Func()
        {
            using (var file = new TempFile())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<ArgumentNullException>(() => Tsv.Save(null, new TestEntryFile(file.Info).GetFile));
                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionary_FileInfoNull()
        {
            var data = new List<KeyStringDictionary>();

            Assert.Throws<ArgumentNullException>(() => Tsv.Save(data, null as FileInfo));
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionary_FileInfoNull_FileMode()
        {
            var data = new List<KeyStringDictionary>();

            Assert.Throws<ArgumentNullException>(() => Tsv.Save(data, null, FileMode.Append));
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionary_FileInfo_FileMode()
        {
            var data = new[]
                           {
                               new KeyStringDictionary
                                   {
                                       new KeyStringPair("A", "1"),
                                       new KeyStringPair("B", "2")
                                   }
                           };

            using (var temp = new TempDirectory())
            {
                var file = temp.Info.ToDirectory("example").ToFile("test.tsv");
                Tsv.Save(data, file, FileMode.Create);

                var expected = "A\tB{0}1\t2{0}".FormatWith(Environment.NewLine);
                var actual = file.ReadToEnd();

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionary_FileInfo_whenAppend()
        {
            var data = new[]
                           {
                               new KeyStringDictionary
                                   {
                                       new KeyStringPair("A", "1"),
                                       new KeyStringPair("B", "2")
                                   }
                           };

            using (var temp = new TempDirectory())
            {
                var file = temp.Info.ToFile("test.tsv").AppendLine("A\tB");
                Tsv.Save(data, file);

                var expected = "A\tB{0}1\t2{0}".FormatWith(Environment.NewLine);
                var actual = file.ReadToEnd();

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionary_FileInfo_whenCreateNew()
        {
            var data = new[]
                           {
                               new KeyStringDictionary
                                   {
                                       new KeyStringPair("A", "1"),
                                       new KeyStringPair("B", "2")
                                   }
                           };

            using (var temp = new TempDirectory())
            {
                var file = temp.Info.ToDirectory("example").ToFile("test.tsv");
                Tsv.Save(data, file);

                var expected = "A\tB{0}1\t2{0}".FormatWith(Environment.NewLine);
                var actual = file.ReadToEnd();

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionary_Func()
        {
            var data = new[]
                           {
                               new KeyStringDictionary
                                   {
                                       new KeyStringPair("A", "1"),
                                       new KeyStringPair("B", "2")
                                   }
                           };

            using (var temp = new TempDirectory())
            {
                var file = temp.Info.ToDirectory("example").ToFile("test.tsv");
                Tsv.Save(data, new TestEntryFile(file).GetFile);

                var expected = "A\tB{0}1\t2{0}".FormatWith(Environment.NewLine);
                var actual = file.ReadToEnd();

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionary_FuncNull()
        {
            var data = new List<KeyStringDictionary>();

            Assert.Throws<ArgumentNullException>(() => Tsv.Save(data, null as Func<KeyStringDictionary, FileInfo>));
        }
    }
}