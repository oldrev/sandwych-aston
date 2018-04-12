using System;
using System.Linq.Expressions;
using Xunit;

namespace Sandwych.Aston.Tests
{
    public partial class AstonParserTest
    {
        [Fact]
        public void CanParseIntegerNumbers()
        {
            int intValue;
            Assert.True(this.TryEvaluate<int>("eval(123)", out intValue));
            Assert.Equal(123, intValue);
        }

        [Fact]
        public void CanParseLongIntegerNumbers()
        {
            long longValue;
            Assert.True(this.TryEvaluate<long>("eval(123L)", out longValue));
            Assert.Equal(123L, longValue);

            Assert.True(this.TryEvaluate<long>("eval(3212l)", out longValue));
            Assert.Equal(3212L, longValue);

            Assert.True(this.TryEvaluate<object>("eval(321)", out var badLongValue));
            Assert.IsNotType<long>(badLongValue);
            Assert.IsType<int>(badLongValue);
        }

        [Fact]
        public void CanParseDoubleNumbers()
        {
            double doubleValue;
            Assert.True(this.TryEvaluate<double>("eval(12.5)", out doubleValue));

            Assert.True(this.TryEvaluate<double>("eval(123.55d)", out doubleValue));

            Assert.True(this.TryEvaluate<double>("eval(123.55D)", out doubleValue));
        }

        [Fact]
        public void CanParseFloatNumbers()
        {
            float floatValue;

            Assert.True(this.TryEvaluate<float>("eval(123.5f)", out floatValue));

            Assert.True(this.TryEvaluate<float>("eval(123.5F)", out floatValue));
        }

        [Fact]
        public void CanParseDecimalLiterals()
        {
            decimal value;
            Assert.True(this.TryEvaluate<decimal>("eval(123.5m)", out value));
            Assert.True(this.TryEvaluate<decimal>("eval(123.5M)", out value));
        }

        [Fact]
        public void CanParseSingleQuotedStrings()
        {
            var stringLiteral = "DunderMifflin";
            string value;

            //Single quoted string
            Assert.True(this.TryEvaluate<string>($"eval('DunderMifflin')", out value));
            Assert.Equal(stringLiteral, value);
        }

        [Fact]
        public void CanParseDoubleQuotedStrings()
        {
            var stringLiteral = "DunderMifflin";
            string value;

            //Double quoted string
            Assert.True(this.TryEvaluate<string>($"eval(\"DunderMifflin\")", out value));
            Assert.Equal(stringLiteral, value);
        }

        [Fact]
        public void CanParseGuidLiterals()
        {
            var guidLiteral = "bd11ec30-3110-4ba9-889b-a2ebcb2abe3a";
            Guid value;
            Assert.True(this.TryEvaluate<Guid>($"eval(guid'{guidLiteral}')", out value));
            Assert.Equal(Guid.Parse(guidLiteral), value);
        }

        [Fact]
        public void CanParseDateTimeLiterals()
        {
            var dateTimeLiteral = "2018-04-01T12:32:11";
            DateTime value;
            Assert.True(this.TryEvaluate<DateTime>($"eval(dt'{dateTimeLiteral}')", out value));
            Assert.Equal(DateTime.Parse(dateTimeLiteral), value);
        }

        [Fact]
        public void CanParseDateTimeOffsetLiterals()
        {
            var dateTimeOffsetLiteral = "2018-04-01T12:32:11";
            DateTimeOffset value;
            Assert.True(this.TryEvaluate<DateTimeOffset>($"eval(dto'{dateTimeOffsetLiteral}')", out value));
            Assert.Equal(DateTimeOffset.Parse(dateTimeOffsetLiteral), value);
        }

    }
}
