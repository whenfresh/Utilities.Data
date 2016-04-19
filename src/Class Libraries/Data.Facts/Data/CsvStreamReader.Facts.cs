namespace Cavity.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Cavity.Collections;
    using Xunit;

    public sealed class CsvStreamReaderFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<CsvStreamReader>()
                            .DerivesFrom<StreamReader>()
                            .IsConcreteClass()
                            .IsUnsealed()
                            .NoDefaultConstructor()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void ctor_Stream()
        {
            using (var stream = new MemoryStream())
            {
                Assert.NotNull(new CsvStreamReader(stream));
            }
        }

        [Fact]
        public void ctor_StreamNull()
        {
            Assert.Throws<ArgumentNullException>(() => new CsvStreamReader(null));
        }

        [Fact]
        public void ctor_StreamNull_IEnumerableString()
        {
            var headers = new[]
                              {
                                  "A", "B"
                              };

            Assert.Throws<ArgumentNullException>(() => new CsvStreamReader(null, headers));
        }

        [Fact]
        public void ctor_StreamNull_IEnumerableStringEmpty()
        {
            using (var stream = new MemoryStream())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<ArgumentOutOfRangeException>(() => new CsvStreamReader(stream, new List<string>()));

                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void ctor_StreamNull_IEnumerableStringNull()
        {
            using (var stream = new MemoryStream())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<ArgumentNullException>(() => new CsvStreamReader(stream, null as IEnumerable<string>));

                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void ctor_Stream_IEnumerableString()
        {
            var headers = new[]
                              {
                                  "A", "B"
                              };

            using (var stream = new MemoryStream())
            {
                Assert.NotNull(new CsvStreamReader(stream, headers));
            }
        }

        [Fact]
        public void ctor_Stream_string()
        {
            using (var stream = new MemoryStream())
            {
                Assert.NotNull(new CsvStreamReader(stream, "A,B"));
            }
        }

        [Fact]
        public void ctor_Stream_stringEmpty()
        {
            using (var stream = new MemoryStream())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<ArgumentOutOfRangeException>(() => new CsvStreamReader(stream, string.Empty));

                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void ctor_Stream_stringNull()
        {
            using (var stream = new MemoryStream())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<ArgumentNullException>(() => new CsvStreamReader(stream, null as string));

                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void op_ReadEntry()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("A,B");
                    writer.WriteLine("1A,1B");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new CsvStreamReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal("A,B", reader.Header);
                        Assert.Equal(2, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1A", actual["A"]);
                        Assert.Equal("1B", actual["B"]);
                    }
                }
            }
        }

        [Fact]
        public void op_ReadEntryOfT()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("A,B");
                    writer.WriteLine("1A,1B");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new CsvStreamReader(stream))
                    {
                        var actual = reader.ReadEntry<KeyStringDictionary>();
                        Assert.Equal("A,B", reader.Header);
                        Assert.Equal(2, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1A", actual["A"]);
                        Assert.Equal("1B", actual["B"]);
                    }
                }
            }
        }

        [Fact]
        public void op_ReadEntry_whenColumnsCtor()
        {
            var headers = new[]
                              {
                                  "A", "B"
                              };

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("1A,1B");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new CsvStreamReader(stream, headers))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal("A,B", reader.Header);
                        Assert.Equal(1, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1A", actual["A"]);
                        Assert.Equal("1B", actual["B"]);
                    }
                }
            }
        }

        [Fact]
        public void op_ReadEntry_whenEmbeddedComma()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("A,B");
                    writer.WriteLine("\"1,A\",\"1,B\"");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new CsvStreamReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(2, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1,A", actual["A"]);
                        Assert.Equal("1,B", actual["B"]);
                    }
                }
            }
        }

        [Fact]
        public void op_ReadEntry_whenEmbeddedLeadingAndTrailingSpaces()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine(" A , B ");
                    writer.WriteLine("\" 1A \", 1B ");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new CsvStreamReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(2, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal(" 1A ", actual["A"]);
                        Assert.Equal("1B", actual["B"]);
                    }
                }
            }
        }

        [Fact]
        public void op_ReadEntry_whenEmbeddedQuotation()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("A,B,C");
                    writer.WriteLine("\"\"\"1A\",\"1\"\"B\",\"1C\"\"\"");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new CsvStreamReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(2, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("\"1A", actual["A"]);
                        Assert.Equal("1\"B", actual["B"]);
                        Assert.Equal("1C\"", actual["C"]);
                    }
                }
            }
        }

        [Fact]
        public void op_ReadEntry_whenEmptyLine()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("A,B");
                    writer.WriteLine(string.Empty);
                    writer.WriteLine("1A,1B");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new CsvStreamReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(3, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1A", actual["A"]);
                        Assert.Equal("1B", actual["B"]);
                    }
                }
            }
        }

        [Fact]
        public void op_ReadEntry_whenHeaderCtor()
        {
            const string header = "A,B";

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("1A,1B");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new CsvStreamReader(stream, header))
                    {
                        Assert.Equal(header, reader.Header);
                        var actual = reader.ReadEntry();
                        Assert.Equal(1, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1A", actual["A"]);
                        Assert.Equal("1B", actual["B"]);
                    }
                }
            }
        }

        [Fact]
        public void op_ReadEntry_whenLeadingAndTrailingSpaces()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine(" A , B ");
                    writer.WriteLine(" 1A , 1B ");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new CsvStreamReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(2, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1A", actual["A"]);
                        Assert.Equal("1B", actual["B"]);
                    }
                }
            }
        }

        [Fact]
        public void op_ReadEntry_whenLineBreak()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("A,B,C");
                    writer.WriteLine("1A,\"Line1");
                    writer.WriteLine("Line2\nLine3\",1C");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new CsvStreamReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(4, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1A", actual["A"]);
                        Assert.Equal("Line1" + Environment.NewLine + "Line2" + Environment.NewLine + "Line3", actual["B"]);
                        Assert.Equal("1C", actual["C"]);
                    }
                }
            }
        }

        [Fact]
        public void op_ReadEntry_whenMissingColumnItem()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("A,B,C");
                    writer.WriteLine("1,2");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new CsvStreamReader(stream))
                    {
                        // ReSharper disable AccessToDisposedClosure
                        Assert.Throws<FormatException>(() => reader.ReadEntry());

                        // ReSharper restore AccessToDisposedClosure
                    }
                }
            }
        }

        [Fact]
        public void op_ReadEntry_whenQuotation()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("A,B");
                    writer.WriteLine("\"1A\",\"1B\"");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new CsvStreamReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(2, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1A", actual["A"]);
                        Assert.Equal("1B", actual["B"]);
                    }
                }
            }
        }

        [Fact]
        public void op_ReadEntry_whenTrailingNewLine()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("A,B");
                    writer.WriteLine("1A,1B");
                    writer.WriteLine(string.Empty);
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new CsvStreamReader(stream))
                    {
                        reader.ReadEntry();
                        var actual = reader.ReadEntry();
                        Assert.Equal(3, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Null(actual);
                    }
                }
            }
        }

        [Fact]
        public void op_ReadEntry_whenUnixLF()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("A,B\n1A,1B");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new CsvStreamReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(2, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1A", actual["A"]);
                        Assert.Equal("1B", actual["B"]);
                    }
                }
            }
        }

        [Fact]
        public void prop_EntryNumber()
        {
            Assert.True(new PropertyExpectations<CsvStreamReader>(p => p.EntryNumber)
                            .TypeIs<int>()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void prop_Header()
        {
            Assert.True(new PropertyExpectations<CsvStreamReader>(p => p.Header)
                            .TypeIs<string>()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void prop_Line()
        {
            Assert.True(new PropertyExpectations<CsvStreamReader>(p => p.Line)
                            .TypeIs<string>()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void prop_LineNumber()
        {
            Assert.True(new PropertyExpectations<CsvStreamReader>(p => p.LineNumber)
                            .TypeIs<int>()
                            .IsNotDecorated()
                            .Result);
        }
    }
}