namespace Cavity.Data
{
    using System;
    using Xunit;
    using Xunit.Extensions;

    public sealed class CsvStringExtensionMethodsFacts
    {
        [Fact]
        public void a_definition()
        {
            Assert.True(typeof(CsvStringExtensionMethods).IsStatic());
        }

        [Fact]
        public void op_FormatCommaSeparatedValue_string()
        {
            const string expected = "example";
            var actual = expected.FormatCommaSeparatedValue();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_FormatCommaSeparatedValue_stringComma()
        {
            const string expected = "\"foo, bar\"";
            var actual = "foo, bar".FormatCommaSeparatedValue();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_FormatCommaSeparatedValue_stringCommaQuote()
        {
            const string expected = "\"\"\"foo\"\", bar\"";
            var actual = "\"foo\", bar".FormatCommaSeparatedValue();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_FormatCommaSeparatedValue_stringEmpty()
        {
            var expected = string.Empty;
            var actual = expected.FormatCommaSeparatedValue();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_FormatCommaSeparatedValue_stringLeadingSpace()
        {
            const string expected = "\" example\"";
            var actual = " example".FormatCommaSeparatedValue();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_FormatCommaSeparatedValue_stringNewLine()
        {
            var expected = "\"foo{0} bar\"".FormatWith(Environment.NewLine);
            var actual = "foo{0} bar".FormatWith(Environment.NewLine).FormatCommaSeparatedValue();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_FormatCommaSeparatedValue_stringNull()
        {
            Assert.Null((null as string).FormatCommaSeparatedValue());
        }

        [Theory]
        [InlineData("\"a \"\"b\"\" c\"", "a \"b\" c")]
        [InlineData("\"a, \"\"b\"\", c\"", "a, \"b\", c")]
        public void op_FormatCommaSeparatedValue_stringQuote(string expected,
                                                             string value)
        {
            var actual = value.FormatCommaSeparatedValue();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_FormatCommaSeparatedValue_stringTrailingSpace()
        {
            const string expected = "\"example \"";
            var actual = "example ".FormatCommaSeparatedValue();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void op_FormatCommaSeparatedValue_stringUnixLF()
        {
            const string expected = "\"foo\n bar\"";
            var actual = "foo\n bar".FormatCommaSeparatedValue();

            Assert.Equal(expected, actual);
        }
    }
}