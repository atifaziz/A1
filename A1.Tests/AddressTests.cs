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
    using Xunit;

    public class AddressTests
    {
        [Fact]
        public void InitEmpty()
        {
            var address = new Address();
            Assert.Equal(false, address.IsColAbs);
            Assert.Equal(0, address.Col);
            Assert.Equal(false, address.IsRowAbs);
            Assert.Equal(0, address.Row);
            Assert.Equal("A1", address.ToString());
        }

        [Fact]
        public void InitRelative()
        {
            var address = new Address(12, 34);
            Assert.Equal(false, address.IsColAbs);
            Assert.Equal(12, address.Col);
            Assert.Equal(false, address.IsRowAbs);
            Assert.Equal(34, address.Row);
            Assert.Equal("M35", address.ToString());
        }

        [Fact]
        public void InitAbsolute()
        {
            var address = new Address(12, 34, true);
            Assert.Equal(true, address.IsColAbs);
            Assert.Equal(12, address.Col);
            Assert.Equal(true, address.IsRowAbs);
            Assert.Equal(34, address.Row);
            Assert.Equal("$M$35", address.ToString());
        }

        [Theory]
        [InlineData(false, 12, false, 34, "M35"  )]
        [InlineData(true , 12, false, 34, "$M35" )]
        [InlineData(false, 12, true , 34, "M$35" )]
        [InlineData(true , 12, true , 34, "$M$35")]
        public void Init(bool isColAbs, int col, bool isRowAbs, int row, string s)
        {
            var address = new Address(isColAbs, col, isRowAbs, row);
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
        [InlineData("A1"    ,  false,    0, false,  0)]
        [InlineData("B1"    ,  false,    1, false,  0)]
        [InlineData("C1"    ,  false,    2, false,  0)]
        [InlineData("A5"    ,  false,    0, false,  4)]
        [InlineData("B5"    ,  false,    1, false,  4)]
        [InlineData("C5"    ,  false,    2, false,  4)]
        [InlineData("AA1"   ,  false,   26, false,  0)]
        [InlineData("AB2"   ,  false,   27, false,  1)]
        [InlineData("AC3"   ,  false,   28, false,  2)]
        [InlineData("ABC43" ,  false,  730, false, 42)]
        [InlineData("DEF43" ,  false, 2839, false, 42)]
        [InlineData("GHI43" ,  false, 4948, false, 42)]
        [InlineData("$GHI43",  true , 4948, false, 42)]
        [InlineData("GHI$43",  false, 4948, true , 42)]
        [InlineData("$GHI$43", true , 4948, true , 42)]
        [InlineData("a1"    ,  false,    0, false,  0)]
        [InlineData("b1"    ,  false,    1, false,  0)]
        [InlineData("c1"    ,  false,    2, false,  0)]
        [InlineData("a5"    ,  false,    0, false,  4)]
        [InlineData("b5"    ,  false,    1, false,  4)]
        [InlineData("c5"    ,  false,    2, false,  4)]
        [InlineData("aa1"   ,  false,   26, false,  0)]
        [InlineData("ab2"   ,  false,   27, false,  1)]
        [InlineData("ac3"   ,  false,   28, false,  2)]
        [InlineData("abc43" ,  false,  730, false, 42)]
        [InlineData("def43" ,  false, 2839, false, 42)]
        [InlineData("ghi43" ,  false, 4948, false, 42)]
        [InlineData("$ghi43",  true , 4948, false, 42)]
        [InlineData("ghi$43",  false, 4948, true , 42)]
        [InlineData("$ghi$43", true , 4948, true , 42)]
        public void ParseA1(string s, bool isColAbs, int col, bool isRowAbs, int row)
        {
            var address = Address.ParseA1(s);
            Assert.Equal(isColAbs, address.IsColAbs);
            Assert.Equal(col, address.Col);
            Assert.Equal(isRowAbs, address.IsRowAbs);
            Assert.Equal(row, address.Row);
            Assert.Equal(s, address.ToString(), StringComparer.OrdinalIgnoreCase);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Al")]
        [InlineData("AB1C")]
        [InlineData("F00")]
        [InlineData("F-1")]
        [InlineData("FOO")]
        public void ParseA1ThrowsFormatException(string s)
        {
            Assert.Throws<FormatException>(() => Address.ParseA1(s));
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
            Assert.Equal(true, absolute.IsColAbs);
            Assert.Equal(true, absolute.IsRowAbs);
            Assert.Equal(absolute.ColRow, address.ColRow);
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
            Assert.Equal(false, absolute.IsColAbs);
            Assert.Equal(false, absolute.IsRowAbs);
            Assert.Equal(absolute.ColRow, address.ColRow);
        }

        [Fact]
        public void EqualityWithRelativeAnchor()
        {
            Assert.True(new Address(12, 34).Equals(new Address(12, 34)));
        }

        [Fact]
        public void EqualityWithAbsoluteAnchor()
        {
            Assert.True(new Address(12, 34, true).Equals(new Address(12, 34, true)));
        }

        [Fact]
        public void EqualityWithMixedAnchor()
        {
            Assert.True(new Address(true, 12, false, 34).Equals(new Address(true, 12, false, 34)));
        }

        [Fact]
        public void InequalityWithRelativeAnchor()
        {
            Assert.False(new Address(12, 34).Equals(new Address(34, 12)));
        }

        [Fact]
        public void InequalityWithAbsoluteAnchor()
        {
            Assert.False(new Address(12, 34).Equals(new Address(12, 34, true)));
        }

        [Fact]
        public void InequalityWithMixedAnchor()
        {
            Assert.False(new Address(false, 12, true, 34).Equals(new Address(true, 12, false, 34)));
        }
    }
}
