using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sandwych.Aston.Tests
{
    public class AstonParserTest : ExpressionBasedTest
    {
        [Fact]
        public void NestedFuncCallingShouldBeOk()
        {
            Assert.True(this.TryEvaluate<bool>("eq(add(1, 2), 3)", out var result));
            Assert.True(result);
        }

        [Fact]
        public void ArithmeticOperationsShouldBeOk()
        {
            {
                Assert.True(this.TryEvaluate<int>("add(2, 3)", out var value));
                Assert.Equal(5, value);
            }
            {
                Assert.True(this.TryEvaluate<int>("sub(2, 3)", out var value));
                Assert.Equal(-1, value);
            }
            {
                Assert.True(this.TryEvaluate<int>("mul(2, 3)", out var value));
                Assert.Equal(6, value);
            }
            {
                Assert.True(this.TryEvaluate<int>("div(9, 3)", out var value));
                Assert.Equal(3, value);
            }
            {
                Assert.True(this.TryEvaluate<int>("mod(7, 3)", out var value));
                Assert.Equal(1, value);
            }
            {
                Assert.True(this.TryEvaluate<double>("pow(2.0, 3.0)", out var value));
                Assert.Equal(8.0, value, 6);
            }
        }
    }
}
