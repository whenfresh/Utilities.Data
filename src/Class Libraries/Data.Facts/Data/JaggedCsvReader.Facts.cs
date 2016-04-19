namespace Cavity.Data
{
    using System;
    using System.IO;
    using Xunit;

    public sealed class JaggedCsvReaderFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(new TypeExpectations<JaggedCsvReader>()
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
                Assert.NotNull(new JaggedCsvReader(stream));
            }
        }

        [Fact]
        public void ctor_StreamNull()
        {
            Assert.Throws<ArgumentNullException>(() => new JaggedCsvReader(null));
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
                    using (var reader = new JaggedCsvReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(2, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1A", actual[0]);
                        Assert.Equal("1B", actual[1]);
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
                    using (var reader = new JaggedCsvReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(2, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1,A", actual[0]);
                        Assert.Equal("1,B", actual[1]);
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
                    writer.WriteLine("A,B");
                    writer.WriteLine("\" 1A \", 1B ");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new JaggedCsvReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(2, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal(" 1A ", actual[0]);
                        Assert.Equal("1B", actual[1]);
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
                    using (var reader = new JaggedCsvReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(2, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("\"1A", actual[0]);
                        Assert.Equal("1\"B", actual[1]);
                        Assert.Equal("1C\"", actual[2]);
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
                    using (var reader = new JaggedCsvReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(3, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1A", actual[0]);
                        Assert.Equal("1B", actual[1]);
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
                    writer.WriteLine("A,B");
                    writer.WriteLine(" 1A , 1B ");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new JaggedCsvReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(2, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1A", actual[0]);
                        Assert.Equal("1B", actual[1]);
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
                    writer.WriteLine("A,B");
                    writer.WriteLine("1A,\"Line1");
                    writer.WriteLine("Line2\nLine3\",1C");
                    writer.Flush();
                    stream.Position = 0;
                    using (var reader = new JaggedCsvReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(4, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1A", actual[0]);
                        Assert.Equal("Line1" + Environment.NewLine + "Line2" + Environment.NewLine + "Line3", actual[1]);
                        Assert.Equal("1C", actual[2]);
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
                    using (var reader = new JaggedCsvReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(2, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1A", actual[0]);
                        Assert.Equal("1B", actual[1]);
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
                    using (var reader = new JaggedCsvReader(stream))
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
                    using (var reader = new JaggedCsvReader(stream))
                    {
                        var actual = reader.ReadEntry();
                        Assert.Equal(2, reader.LineNumber);
                        Assert.Equal(1, reader.EntryNumber);
                        Assert.Equal("1A", actual[0]);
                        Assert.Equal("1B", actual[1]);
                    }
                }
            }
        }

        [Fact]
        public void prop_EntryNumber()
        {
            Assert.True(new PropertyExpectations<JaggedCsvReader>(p => p.EntryNumber)
                            .TypeIs<int>()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void prop_Line()
        {
            Assert.True(new PropertyExpectations<JaggedCsvReader>(p => p.Line)
                            .TypeIs<string>()
                            .IsNotDecorated()
                            .Result);
        }

        [Fact]
        public void prop_LineNumber()
        {
            Assert.True(new PropertyExpectations<JaggedCsvReader>(p => p.LineNumber)
                            .TypeIs<int>()
                            .IsNotDecorated()
                            .Result);
        }
    }
}