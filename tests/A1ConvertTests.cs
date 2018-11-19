#region Copyright (c) 2016 Atif Aziz. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

namespace A1.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Xunit;

    public class A1ConvertTests
    {
        [Fact]
        public void AlphaColumnForbidsNull()
        {
            var e = Assert.Throws<ArgumentNullException>(() => A1Convert.AlphaColumnNumber(null));
            Assert.Equal("alpha", e.ParamName);
        }

        [Theory]
        [MemberData(nameof(AlphaColumnData))]
        public void AlphaColumnNumber(string alpha, int num)
        {
            Assert.Equal(num, A1Convert.AlphaColumnNumber(alpha));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("?")]
        [InlineData("A?C")]
        [InlineData("AB?")]
        public void TryAlphaColumnNumber(string alpha)
        {
            Assert.Null(A1Convert.TryAlphaColumnNumber(alpha));
        }

        [Theory]
        [MemberData(nameof(NonAlphaChars))]
        public void AlphaColumnNumberThrowsForNonAlphaChar(char ch)
        {
            var e = Assert.Throws<ArgumentException>(() => A1Convert.AlphaColumnNumber("A" + ch + "C"));
            Assert.Equal("alpha", e.ParamName);
        }

        public static IEnumerable<object[]> NonAlphaChars =>
            from ch in Enumerable.Range(char.MinValue, char.MaxValue)
            select (char) ch into ch
            where (ch < 'A' || ch > 'Z') && (ch < 'a' || ch > 'z')
            select new object[] { ch };


        [Theory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData( 0)]
        [InlineData(A1Convert.MaxColumn + 1)]
        public void NumberColumnAlphaForbidsNumbersOutOfRange(int num)
        {
            var e = Assert.Throws<ArgumentOutOfRangeException>(() => A1Convert.NumberColumnAlpha(num));
            Assert.Equal("number", e.ParamName);
        }

        [Theory]
        [MemberData(nameof(AlphaColumnData))]
        public void NumberColumnAlpha(string alpha, int num)
        {
            Assert.Equal(alpha, A1Convert.NumberColumnAlpha(num));
        }

        public static IEnumerable<object[]> AlphaColumnData
        {
            get
            {
                var typeInfo = typeof(A1ConvertTests).GetTypeInfo();
                using (var reader = new StreamReader(typeInfo.Assembly.GetManifestResourceStream(typeInfo.Namespace + ".AlphaColumns.txt")))
                {
                    var i = 0;
                    string line;
                    while ((line = reader.ReadLine()) != null)
                        yield return new object[] { line.Trim(), ++i };
                }
            }
        }
    }
}
