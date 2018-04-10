using System;
using System.Linq.Expressions;
using Xunit;

namespace Sandwych.Aston.Tests
{
    public class AstonParserTest : ExpressionBasedTest
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
        public void NestedFuncCallingShouldBeOk()
        {
            Assert.True(this.TryEvaluate<bool>("eq(add(1, 2), 3)", out var result));
            Assert.True(result);
        }

    }
}
