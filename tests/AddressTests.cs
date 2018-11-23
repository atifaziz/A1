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
    using System.Globalization;
    using Xunit;

    public class AddressTests
    {
        [Fact]
        public void InitEmpty()
        {
            var address = new Address();
            Assert.False(address.IsColAbs);
            Assert.Equal(1, address.Col);
            Assert.False(address.IsRowAbs);
            Assert.Equal(1, address.Row);
            Assert.Equal("A1", address.ToString());
        }

        [Fact]
        public void InitRelative()
        {
            var address = new Address((Row) 35, (Col) 13);
            Assert.False(address.IsRowAbs);
            Assert.Equal(35, address.Row);
            Assert.False(address.IsColAbs);
            Assert.Equal(13, address.Col);
            Assert.Equal("M35", address.ToString());
        }

        [Fact]
        public void InitAbsolute()
        {
            var address = new Address((Row) 35, (Col) 13, true);
            Assert.True(address.IsColAbs);
            Assert.Equal(13, address.Col);
            Assert.True(address.IsRowAbs);
            Assert.Equal(35, address.Row);
            Assert.Equal("$M$35", address.ToString());
        }

        [Theory]
        [InlineData(false, 13, false, 35, "M35"  )]
        [InlineData(true , 13, false, 35, "$M35" )]
        [InlineData(false, 13, true , 35, "M$35" )]
        [InlineData(true , 13, true , 35, "$M$35")]
        public void Init(bool isColAbs, int col, bool isRowAbs, int row, string s)
        {
            var address = new Address(isRowAbs, (Row) row, isColAbs, (Col) col);
            Assert.Equal(isColAbs, address.IsColAbs);
            Assert.Equal(col, address.Col);
            Assert.Equal(isRowAbs, address.IsRowAbs);
            Assert.Equal(row, address.Row);
            Assert.Equal(s, address.ToString());
        }

        [Fact]
        public void ParseA1ForbidsNull()
        {
            var e = Assert.Throws<ArgumentNullException>(() => Address.ParseA1(null));
            Assert.Equal("s", e.ParamName);
        }

        [Theory]
        [InlineData("A1"    ,  false,    1, false,  1)]
        [InlineData("B1"    ,  false,    2, false,  1)]
        [InlineData("C1"    ,  false,    3, false,  1)]
        [InlineData("A5"    ,  false,    1, false,  5)]
        [InlineData("B5"    ,  false,    2, false,  5)]
        [InlineData("C5"    ,  false,    3, false,  5)]
        [InlineData("AA1"   ,  false,   27, false,  1)]
        [InlineData("AB2"   ,  false,   28, false,  2)]
        [InlineData("AC3"   ,  false,   29, false,  3)]
        [InlineData("ABC43" ,  false,  731, false, 43)]
        [InlineData("DEF43" ,  false, 2840, false, 43)]
        [InlineData("GHI43" ,  false, 4949, false, 43)]
        [InlineData("$GHI43",  true , 4949, false, 43)]
        [InlineData("GHI$43",  false, 4949, true , 43)]
        [InlineData("$GHI$43", true , 4949, true , 43)]
        [InlineData("a1"    ,  false,    1, false,  1)]
        [InlineData("b1"    ,  false,    2, false,  1)]
        [InlineData("c1"    ,  false,    3, false,  1)]
        [InlineData("a5"    ,  false,    1, false,  5)]
        [InlineData("b5"    ,  false,    2, false,  5)]
        [InlineData("c5"    ,  false,    3, false,  5)]
        [InlineData("aa1"   ,  false,   27, false,  1)]
        [InlineData("ab2"   ,  false,   28, false,  2)]
        [InlineData("ac3"   ,  false,   29, false,  3)]
        [InlineData("abc43" ,  false,  731, false, 43)]
        [InlineData("def43" ,  false, 2840, false, 43)]
        [InlineData("ghi43" ,  false, 4949, false, 43)]
        [InlineData("$ghi43",  true , 4949, false, 43)]
        [InlineData("ghi$43",  false, 4949, true , 43)]
        [InlineData("$ghi$43", true , 4949, true , 43)]
        public void ParseA1(string s, bool isColAbs, int col, bool isRowAbs, int row)
        {
            var address = Address.ParseA1(s);
            Assert.Equal(isRowAbs, address.IsRowAbs);
            Assert.Equal(row, address.Row);
            Assert.Equal(isColAbs, address.IsColAbs);
            Assert.Equal(col, address.Col);
            Assert.Equal(s, address.ToString(), StringComparer.OrdinalIgnoreCase);
        }

        public static IEnumerable<object[]> BadA1Data => new[]
        {
            Data(""     ),
            Data("Al"   ),
            Data("AB1C" ),
            Data("F00"  ),
            Data("F-1"  ),
            Data("FOO"  ),
            Data("TEST1"),
        };

        [Theory]
        [MemberData(nameof(BadA1Data))]
        public void ParseA1ThrowsFormatException(string s)
        {
            Assert.Throws<FormatException>(() => Address.ParseA1(s));
        }

        [Theory]
        [MemberData(nameof(BadA1Data))]
        public void TryParseA1(string s)
        {
            Assert.Null(Address.TryParseA1(s));
        }

        [Fact]
        public void TryParseA1AllowsNull()
        {
            Assert.Null(Address.TryParseA1(null));
        }

        [Theory]
        [InlineData("A1")]
        [InlineData("$A1")]
        [InlineData("A$1")]
        [InlineData("$A$1")]
        public void MakeAbsolute(string s)
        {
            var address = Address.ParseA1(s);
            var absolute = address.MakeAbsolute();
            Assert.True(absolute.IsColAbs);
            Assert.True(absolute.IsRowAbs);
            Assert.Equal(absolute.RowCol, address.RowCol);
        }

        [Theory]
        [InlineData("A1")]
        [InlineData("$A1")]
        [InlineData("A$1")]
        [InlineData("$A$1")]
        public void MakeRelative(string s)
        {
            var address = Address.ParseA1(s);
            var absolute = address.MakeRelative();
            Assert.False(absolute.IsColAbs);
            Assert.False(absolute.IsRowAbs);
            Assert.Equal(absolute.RowCol, address.RowCol);
        }

        [Fact]
        public void EqualityWithRelativeAnchor()
        {
            Assert.True(new Address((Row) 12, (Col) 34).Equals(new Address((Row) 12, (Col) 34)));
        }

        [Fact]
        public void EqualityWithAbsoluteAnchor()
        {
            Assert.True(new Address((Row) 12, (Col) 34, true).Equals(new Address((Row) 12, (Col) 34, true)));
        }

        [Fact]
        public void EqualityWithMixedAnchor()
        {
            Assert.True(new Address(true, (Row) 12, false, (Col) 34).Equals(new Address(true, (Row) 12, false, (Col) 34)));
        }

        [Fact]
        public void InequalityWithRelativeAnchor()
        {
            Assert.False(new Address((Row) 12, (Col) 34).Equals(new Address((Row) 34, (Col) 12)));
        }

        [Fact]
        public void InequalityWithAbsoluteAnchor()
        {
            Assert.False(new Address((Row) 12, (Col) 34).Equals(new Address((Row) 12, (Col) 34, true)));
        }

        [Fact]
        public void InequalityWithMixedAnchor()
        {
            Assert.False(new Address(false, (Row) 12, true, (Col) 34).Equals(new Address(true, (Row) 12, false, (Col) 34)));
        }

        static object[] Data(params object[] data) => data;

        [Theory]
        [InlineData("A1"    ,    1,  1,    1,  1)]
        [InlineData("B1"    ,    2,  1,    2,  1)]
        [InlineData("C1"    ,    3,  1,    3,  1)]
        [InlineData("A5"    ,    1,  5,    1,  5)]
        [InlineData("B5"    ,    2,  5,    2,  5)]
        [InlineData("C5"    ,    3,  5,    3,  5)]
        [InlineData("AA1"   ,   27,  1,   27,  1)]
        [InlineData("AB2"   ,   28,  2,   28,  2)]
        [InlineData("AC3"   ,   29,  3,   29,  3)]
        [InlineData("ABC43" ,  731, 43,  731, 43)]
        [InlineData("DEF43" , 2840, 43, 2840, 43)]
        [InlineData("GHI43" , 4949, 43, 4949, 43)]
        [InlineData("A1:C5" ,    1,  1,    3,  5)]
        [InlineData("C5:A1" ,    3,  5,    1,  1)]
        [InlineData("B3:G5" ,    2,  3,    7,  5)]
        public void ParseA1Range(string s, int col1, int row1, int col2, int row2)
        {
            var (from, to) = Address.ParseA1Range(s);
            Assert.Equal(col1, from.Col);
            Assert.Equal(row1, from.Row);
            Assert.Equal(col2, to.Col);
            Assert.Equal(row2, to.Row);
        }

        [Theory]
        [InlineData("FOO")]
        [InlineData("X")]
        [InlineData("42")]
        [InlineData(":B42")]
        [InlineData("A:B42")]
        [InlineData("1:B42")]
        [InlineData("A1:")]
        [InlineData("A1:B")]
        [InlineData("A1:42")]
        [InlineData("TEST42")]
        [InlineData("A1:TEST42")]
        [InlineData("A1 B42")]
        [InlineData("A1;B42")]
        [InlineData("A1:B42!")]
        public void TryParseA1Range(string range)
        {
            Assert.Null(Address.TryParseA1Range(range));
            Assert.False(Address.TryParseA1Range(range, out _, out _));
        }


        const string BadAddress1MessageTemplate = "The first address in the range '?' is not a valid A1 cell reference style. Parsing failed at position #.";
        const string BadAddress2MessageTemplate = "The second address in the range '?' is not a valid A1 cell reference style. Parsing failed at position #.";
        const string BadSeparatorMessageTemplate = "The separator at position # in the range '?' must be a colon (:).";

        [Theory]
        [InlineData("FOO"      , 4, BadAddress1MessageTemplate)]
        [InlineData("X"        , 2, BadAddress1MessageTemplate)]
        [InlineData("42"       , 1, BadAddress1MessageTemplate)]
        [InlineData(":B42"     , 1, BadAddress1MessageTemplate)]
        [InlineData("A:B42"    , 2, BadAddress1MessageTemplate)]
        [InlineData("1:B42"    , 1, BadAddress1MessageTemplate)]
        [InlineData("A1:"      , 4, BadAddress2MessageTemplate)]
        [InlineData("A1:B"     , 5, BadAddress2MessageTemplate)]
        [InlineData("A1:42"    , 4, BadAddress2MessageTemplate)]
        [InlineData("TEST42"   , 5, BadAddress1MessageTemplate)]
        [InlineData("A1:TEST42", 8, BadAddress2MessageTemplate)]
        [InlineData("A1 B42"   , 3, BadSeparatorMessageTemplate)]
        [InlineData("A1;B42"   , 3, BadSeparatorMessageTemplate)]
        [InlineData("A1:B42!"  , 7, "The range '?' is incorrectly terminated at position #.")]
        public void ParseA1RangeBad(string s, int position, string template)
        {
            var e = Assert.Throws<FormatException>(() => Address.ParseA1Range(s));
            var message = template.Replace("?", s)
                                  .Replace("#", position.ToString(CultureInfo.InvariantCulture));
            Assert.Equal(message, e.Message);
        }

        [Fact]
        public void TryParseA1WithNullStringThrows()
        {
            var e = Assert.Throws<ArgumentNullException>(() =>
                Address.TryParseA1(null, 0, 0, out _, out _));
            Assert.Equal("s", e.ParamName);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void TryParseA1WithInvalidIndexThrows(int i)
        {
            var e = Assert.Throws<ArgumentOutOfRangeException>(() =>
                Address.TryParseA1(string.Empty, i, 0, out _, out _));
            Assert.Equal("index", e.ParamName);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public void TryParseA1WithInvalidEndIndexThrows(int i)
        {
            var e = Assert.Throws<ArgumentOutOfRangeException>(() =>
                Address.TryParseA1(string.Empty, 0, i, out _, out _));
            Assert.Equal("endIndex", e.ParamName);
        }

        [Fact]
        public void TryParseA1WithIndexGreaterThanEndIndexThrows()
        {
            var e = Assert.Throws<ArgumentOutOfRangeException>(() =>
                Address.TryParseA1("FOOBAR", 1, 0, out _, out _));
            Assert.Equal("index", e.ParamName);
        }

        [Fact]
        public void ParseA1WithVeryLargeRowThrows()
        {
            Assert.Throws<FormatException>(() =>
                Address.ParseA1("A" + (int.MaxValue + 1L).ToString(CultureInfo.InvariantCulture)));
        }
    }
}
