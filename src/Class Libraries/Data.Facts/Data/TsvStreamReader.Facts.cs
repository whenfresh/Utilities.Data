namespace Cavity.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Cavity.Collections;
    using Xunit;

    public sealed class TsvStreamReaderFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<TsvStreamReader>()
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
                Assert.NotNull(new TsvStreamReader(stream));
            }
        }

        [Fact]
        public void ctor_StreamNull()
        {
            Assert.Throws<ArgumentNullException>(() => new TsvStreamReader(null));
        }

        [Fact]
        public void ctor_StreamNull_IEnumerableString()
        {
            var headers = new[]
                              {
                                  "A", "B"
                              };

            Assert.Throws<ArgumentNullException>(() => new TsvStreamReader(null, headers));
        }

        [Fact]
        public void ctor_StreamNull_IEnumerableStringEmpty()
        {
            using (var stream = new MemoryStream())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<ArgumentOutOfRangeException>(() => new TsvStreamReader(stream, new List<string>()));

                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void ctor_StreamNull_IEnumerableStringNull()
        {
            using (var stream = new MemoryStream())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<ArgumentNullException>(() => new TsvStreamReader(stream, null as IEnumerable<string>));

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
                Assert.NotNull(new TsvStreamReader(stream, headers));
            }
        }

        [Fact]
        public void ctor_Stream_string()
        {
            using (var stream = new MemoryStream())
            {
                Assert.NotNull(new TsvStreamReader(stream, "A\tB"));
            }
        }

        [Fact]
        public void ctor_Stream_stringEmpty()
        {
            using (var stream = new MemoryStream())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<ArgumentOutOfRangeException>(() => new TsvStreamReader(stream, string.Empty));

                // ReSharper restore AccessToDisposedClosure
            }
        }

        [Fact]
        public void ctor_Stream_stringNull()
        {
            using (var stream = new MemoryStream())
            {
                // ReSharper disable AccessToDisposedClosure
                Assert.Throws<ArgumentNullException>(() => new TsvStreamReader(stream, null as string));

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
                    writer.WriteLine("A\tB");
                    writer.WriteLine("1A\t1B");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new TsvStreamReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal("A\tB", reader.Header);
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
                    writer.WriteLine("A\tB");
                    writer.WriteLine("1A\t1B");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new TsvStreamReader(stream))
                    {
                        var actual = reader.ReadEntry<KeyStringDictionary>();
                        Assert.Equal("A\tB", reader.Header);
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
                    writer.WriteLine("1A\t1B");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new TsvStreamReader(stream, headers))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal("A\tB", reader.Header);
                        Assert.Equal(1, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1A", actual["A"]);
                        Assert.Equal("1B", actual["B"]);
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
                    writer.WriteLine("A\tB");
                    writer.WriteLine(string.Empty);
                    writer.WriteLine("1A\t1B");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new TsvStreamReader(stream))
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
            const string header = "A\tB";

            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("1A\t1B");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new TsvStreamReader(stream, header))
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
        public void op_ReadEntry_whenMissingColumnItem()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("A\tB\tC");
                    writer.WriteLine("1\t2");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new TsvStreamReader(stream))
                    {
                        // ReSharper disable AccessToDisposedClosure
                        Assert.Throws<FormatException>(() => reader.ReadEntry());

                        // ReSharper restore AccessToDisposedClosure
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
                    writer.WriteLine("A\tB");
                    writer.WriteLine("1A\t1B");
                    writer.WriteLine(string.Empty);
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new TsvStreamReader(stream))
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
                    writer.WriteLine("A\tB\n1A\t1B");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new TsvStreamReader(stream))
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
            Assert.True(new PropertyExpectations<TsvStreamReader>(p => p.EntryNumber)
                            .TypeIs<int>()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void prop_Header()
        {
            Assert.True(new PropertyExpectations<TsvStreamReader>(p => p.Header)
                            .TypeIs<string>()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void prop_Line()
        {
            Assert.True(new PropertyExpectations<TsvStreamReader>(p => p.Line)
                            .TypeIs<string>()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void prop_LineNumber()
        {
            Assert.True(new PropertyExpectations<TsvStreamReader>(p => p.LineNumber)
                            .TypeIs<int>()
                            .IsNotDecorated()
                            .Result);
        }
    }
}