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

    public sealed class CsvFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(typeof(Csv).IsStatic());
        }

        [Fact]
        public void op_Header_KeyStringDictionary()
        {
            var obj = new KeyStringDictionary
                          {
                              new KeyStringPair("A,B", string.Empty),
                              new KeyStringPair("C", string.Empty)
                          };

            const string expected = "\"A,B\",C";
            var actual = Csv.Header(obj);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Header_KeyStringDictionaryEmpty()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Csv.Header(new KeyStringDictionary()));
        }

        [Fact]
        public void op_Header_KeyStringDictionaryNull()
        {
            Assert.Throws<ArgumentNullException>(() => Csv.Header(null));
        }

        [Fact]
        public void op_Header_KeyStringDictionary_whenEmptyValue()
        {
            var obj = new KeyStringDictionary
                          {
                              new KeyStringPair(string.Empty, "x"),
                              new KeyStringPair("A,B", "x")
                          };

            const string expected = ",\"A,B\"";
            var actual = Csv.Header(obj);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Line_IEnumerableString()
        {
            var obj = new List<string>
                          {
                              "123",
                              "left,right"
                          };

            const string expected = "123,\"left,right\"";
            var actual = Csv.Line(obj);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Line_IEnumerableStringEmpty()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Csv.Line(new List<string>()));
        }

        [Fact]
        public void op_Line_IEnumerableStringNull()
        {
            Assert.Throws<ArgumentNullException>(() => Csv.Line(null as IEnumerable<string>));
        }

        [Fact]
        public void op_Line_IEnumerableString_whenEmptyValue()
        {
            var obj = new List<string>
                          {
                              string.Empty,
                              "left,right"
                          };

            const string expected = ",\"left,right\"";
            var actual = Csv.Line(obj);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Line_KeyStringDictionary()
        {
            var obj = new KeyStringDictionary
                          {
                              new KeyStringPair("A", "123"),
                              new KeyStringPair("B", "left,right"),
                              new KeyStringPair("C", "a \"b\" c"),
                          };

            const string expected = "123,\"left,right\",\"a \"\"b\"\" c\"";
            var actual = Csv.Line(obj);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Line_KeyStringDictionaryEmpty()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Csv.Line(new KeyStringDictionary()));
        }

        [Fact]
        public void op_Line_KeyStringDictionaryNull()
        {
            Assert.Throws<ArgumentNullException>(() => Csv.Line(null as KeyStringDictionary));
        }

        [Fact]
        public void op_Line_KeyStringDictionaryNull_IListOfString()
        {
            var columns = new List<string>
                              {
                                  "A"
                              };
            Assert.Throws<ArgumentNullException>(() => Csv.Line(null, columns));
        }

        [Fact]
        public void op_Line_KeyStringDictionary_IListOfString()
        {
            var obj = new KeyStringDictionary
                          {
                              new KeyStringPair("A", "123"),
                              new KeyStringPair("B", "ignore"),
                              new KeyStringPair("C", "left,right")
                          };

            const string expected = "123,\"left,right\"";
            var actual = Csv.Line(obj, "A,C".Split(',').ToList());

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_Line_KeyStringDictionary_IListOfStringEmpty()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Csv.Line(new KeyStringDictionary(), new List<string>()));
        }

        [Fact]
        public void op_Line_KeyStringDictionary_IListOfStringNull()
        {
            Assert.Throws<ArgumentNullException>(() => Csv.Line(new KeyStringDictionary(), null));
        }

        [Fact]
        public void op_Line_KeyStringDictionary_IListOfString_whenColumnsNotFound()
        {
            Assert.Throws<KeyNotFoundException>(() => Csv.Line(new KeyStringDictionary(), "A,B".Split(',').ToList()));
        }

        [Fact]
        public void op_Line_KeyStringDictionary_whenEmptyValue()
        {
            var obj = new KeyStringDictionary
                          {
                              new KeyStringPair("A", string.Empty),
                              new KeyStringPair("B", "left,right")
                          };

            const string expected = ",\"left,right\"";
            var actual = Csv.Line(obj);

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
                var file = temp.Info.ToFile("test.csv");

                Csv.Save(table, file, FileMode.Create);

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
                var file = temp.Info.ToFile("test.csv");

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
                var file = temp.Info.ToFile("example.csv");

                Csv.Save(table, file, FileMode.Create);

                var expected = "A,B{0}1,2{0}".FormatWith(Environment.NewLine);
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
                var file = temp.Info.ToFile("example.csv").AppendLine("A,B");

                Csv.Save(table, file);

                var expected = "A,B{0}1,2{0}".FormatWith(Environment.NewLine);
                var actual = file.ReadToEnd();

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionaryEmpty_FileInfo()
        {
            using (var temp = new TempDirectory())
            {
                var file = temp.Info.ToFile("test.csv");
                file.CreateNew();

                Csv.Save(new List<KeyStringDictionary>(), file);

                file.Refresh();
                Assert.False(file.Exists);
            }
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionaryEmpty_FileInfo_FileMode()
        {
            using (var temp = new TempDirectory())
            {
                var file = temp.Info.ToFile("test.csv");
                file.CreateNew();

                Csv.Save(new List<KeyStringDictionary>(), file, FileMode.Create);

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
                Assert.Throws<ArgumentNullException>(() => Csv.Save(null as IEnumerable<KeyStringDictionary>, file.Info));
                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionaryNull_FileInfo_FileMode()
        {
            using (var file = new TempFile())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<ArgumentNullException>(() => Csv.Save(null as IEnumerable<KeyStringDictionary>, file.Info, FileMode.Append));
                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionaryNull_Func()
        {
            using (var file = new TempFile())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<ArgumentNullException>(() => Csv.Save(null, new TestEntryFile(file.Info).GetFile));
                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionary_FileInfoNull()
        {
            var data = new List<KeyStringDictionary>();

            Assert.Throws<ArgumentNullException>(() => Csv.Save(data, null as FileInfo));
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionary_FileInfoNull_FileMode()
        {
            var data = new List<KeyStringDictionary>();

            Assert.Throws<ArgumentNullException>(() => Csv.Save(data, null, FileMode.Append));
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
                var file = temp.Info.ToDirectory("example").ToFile("test.csv");
                Csv.Save(data, file, FileMode.Create);

                var expected = "A,B{0}1,2{0}".FormatWith(Environment.NewLine);
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
                var file = temp.Info.ToFile("test.csv").AppendLine("A,B");
                Csv.Save(data, file);

                var expected = "A,B{0}1,2{0}".FormatWith(Environment.NewLine);
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
                var file = temp.Info.ToDirectory("example").ToFile("test.csv");
                Csv.Save(data, file);

                var expected = "A,B{0}1,2{0}".FormatWith(Environment.NewLine);
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
                var file = temp.Info.ToDirectory("example").ToFile("test.csv");
                Csv.Save(data, new TestEntryFile(file).GetFile);

                var expected = "A,B{0}1,2{0}".FormatWith(Environment.NewLine);
                var actual = file.ReadToEnd();

                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void op_Save_IEnumerableOfKeyStringDictionary_FuncNull()
        {
            var data = new List<KeyStringDictionary>();

            Assert.Throws<ArgumentNullException>(() => Csv.Save(data, null as Func<KeyStringDictionary, FileInfo>));
        }
    }
}