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
    using Xunit;

    public class ColRowTest
    {
        [Fact]
        public void InitEmpty()
        {
            var colrow = new ColRow();
            Assert.Equal(0, colrow.Col);
            Assert.Equal(0, colrow.Row);
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
    }
}
