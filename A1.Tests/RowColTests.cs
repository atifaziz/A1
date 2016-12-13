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

    public class RowColTests
    {
        [Fact]
        public void InitEmpty()
        {
            var colrow = new RowCol();
            Assert.Equal(1, colrow.Col);
            Assert.Equal(1, colrow.Row);
        }

        [Fact]
        public void TopLeft()
        {
            Assert.Equal(1, RowCol.TopLeft.Col);
            Assert.Equal(1, RowCol.TopLeft.Row);
        }

        [Theory]
        [InlineData(12, 34)]
        [InlineData(45, 67)]
        [InlineData(67, 89)]
        public void Init(int row, int col)
        {
            var colrow = new RowCol((Row) row, (Col) col);
            Assert.Equal(col, colrow.Col);
            Assert.Equal(row, colrow.Row);
        }

        [Fact]
        public void Equality()
        {
            Assert.True(new RowCol((Row) 12, (Col) 34).Equals(new RowCol((Row) 12, (Col) 34)));
        }

        [Fact]
        public void InEquality()
        {
            Assert.False(new RowCol((Row) 12, (Col) 34).Equals(new RowCol((Row) 34, (Col) 12)));
        }

        [Fact]
        public void OffsetToWithNullSelectorThrows()
        {
            var e = Assert.Throws<ArgumentNullException>(() => RowCol.TopLeft.OffsetTo<object>(RowCol.TopLeft, null));
            Assert.Equal("selector", e.ParamName);
        }

        [Theory]
        [InlineData( 1,  1,  1,  1,   0,   0)]
        [InlineData( 2,  2,  2,  2,   0,   0)]
        [InlineData( 1,  1,  2,  2,   1,   1)]
        [InlineData( 1,  1,  3,  3,   2,   2)]
        [InlineData( 2,  2,  3,  3,   1,   1)]
        [InlineData( 2,  2,  4,  4,   2,   2)]
        [InlineData(13, 35, 57, 79,  44,  44)]
        [InlineData(79, 57, 35, 13, -44, -44)]
        [InlineData(18,  6, 20, 72,   2,  66)]
        [InlineData(18, 92,  6, 72, -12, -20)]
        public void OffsetTo(int c1, int r1, int c2, int r2, int x, int y)
        {
            var cr1 = new RowCol((Row) r1, (Col) c1);
            var cr2 = new RowCol((Row) r2, (Col) c2);
            var offset = cr1.OffsetTo(cr2, (dy, dx) => new { Height = dy, Width = dx });
            Assert.Equal(y, offset.Height);
            Assert.Equal(x, offset.Width);
        }

        [Fact]
        public void SizeWithNullSelectorThrows()
        {
            var e = Assert.Throws<ArgumentNullException>(() => RowCol.TopLeft.Size<object>(RowCol.TopLeft, null));
            Assert.Equal("selector", e.ParamName);
        }

        [Theory]
        [InlineData( 1,  1,  1,  1,   1,   1)]
        [InlineData( 2,  2,  2,  2,   1,   1)]
        [InlineData( 1,  1,  2,  2,   2,   2)]
        [InlineData( 1,  1,  3,  3,   3,   3)]
        [InlineData( 2,  2,  3,  3,   2,   2)]
        [InlineData( 2,  2,  4,  4,   3,   3)]
        [InlineData(13, 35, 57, 79,  45,  45)]
        [InlineData(18,  6, 20, 72,   3,  67)]
        public void Size(int c1, int r1, int c2, int r2, int width, int height)
        {
            var cr1 = new RowCol((Row) r1, (Col) c1);
            var cr2 = new RowCol((Row) r2, (Col) c2);
            var offset = cr1.Size(cr2, (h, w) => new { Height = h, Width = w });
            Assert.Equal(height, offset.Height);
            Assert.Equal(width, offset.Width);
        }

        [Theory]
        [InlineData(2, 1, 1, 2)] // -width, +height
        [InlineData(1, 2, 2, 1)] // +width, -height
        [InlineData(2, 2, 1, 1)] // -width, -height
        public void SizeCannotBeNegative(int c1, int r1, int c2, int r2)
        {
            var cr1 = new RowCol((Row) r1, (Col) c1);
            var cr2 = new RowCol((Row) r2, (Col) c2);
            var e = Assert.Throws<ArgumentException>(() => cr1.Size(cr2, (h, w) => new { Height = h, Width = w }));
            Assert.Equal("other", e.ParamName);
        }
    }
}
