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

    public class ColRowTests
    {
        [Fact]
        public void InitEmpty()
        {
            var colrow = new ColRow();
            Assert.Equal(0, colrow.Col);
            Assert.Equal(0, colrow.Row);
        }

        [Fact]
        public void Zero()
        {
            Assert.Equal(0, ColRow.Zero.Col);
            Assert.Equal(0, ColRow.Zero.Row);
        }

        [Theory]
        [InlineData(12, 34)]
        [InlineData(45, 67)]
        [InlineData(67, 89)]
        public void Init(int col, int row)
        {
            var colrow = new ColRow(col, row);
            Assert.Equal(col, colrow.Col);
            Assert.Equal(row, colrow.Row);
        }

        [Fact]
        public void Equality()
        {
            Assert.True(new ColRow(12, 34).Equals(new ColRow(12, 34)));
        }

        [Fact]
        public void InEquality()
        {
            Assert.False(new ColRow(12, 34).Equals(new ColRow(34, 12)));
        }

        [Fact]
        public void OffsetToWithNullSelectorThrows()
        {
            var e = Assert.Throws<ArgumentNullException>(() => ColRow.Zero.OffsetTo<object>(ColRow.Zero, null));
            Assert.Equal("selector", e.ParamName);
        }

        [Theory]
        [InlineData( 0,  0,  0,  0,   0,   0)]
        [InlineData( 1,  1,  1,  1,   0,   0)]
        [InlineData( 0,  0,  1,  1,   1,   1)]
        [InlineData( 0,  0,  2,  2,   2,   2)]
        [InlineData( 1,  1,  2,  2,   1,   1)]
        [InlineData( 1,  1,  3,  3,   2,   2)]
        [InlineData(12, 34, 56, 78,  44,  44)]
        [InlineData(78, 56, 34, 12, -44, -44)]
        [InlineData(17,  5, 19, 71,   2,  66)]
        [InlineData(17, 91,  5, 71, -12, -20)]
        public void OffsetTo(int c1, int r1, int c2, int r2, int x, int y)
        {
            var cr1 = new ColRow(c1, r1);
            var cr2 = new ColRow(c2, r2);
            var offset = cr1.OffsetTo(cr2, (dx, dy) => new { X = dx, Y = dy });
            Assert.Equal(x, offset.X);
            Assert.Equal(y, offset.Y);
        }
    }
}
