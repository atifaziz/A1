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

namespace A1
{
    using System;
    using System.Globalization;

    public struct ColRow : IEquatable<ColRow>
    {
        public static readonly ColRow Zero = new ColRow(0, 0);

        public int Col { get; }
        public int Row { get; }

        public ColRow(int col, int row)
        {
            if (col < 0) throw new ArgumentOutOfRangeException(nameof(col));
            if (row < 0) throw new ArgumentOutOfRangeException(nameof(row));
            Col = col; Row = row;
        }

        public bool Equals(ColRow other) => Col == other.Col && Row == other.Row;
        public override bool Equals(object obj) => obj is ColRow && Equals((ColRow) obj);
        public override int GetHashCode() => unchecked((Col * 397) ^ Row);

        public string FormatA1() => A1Convert.NumberColumnAlpha(Col + 1) + (Row + 1).ToString(CultureInfo.InvariantCulture);

        public override string ToString() =>
            "(" + Col.ToString(CultureInfo.InvariantCulture) + "," + Row.ToString(CultureInfo.InvariantCulture) + ")";

        public bool IsContainedIn(ColRow a, ColRow b) =>
            a.Col <= b.Col
            && a.Row <= b.Row
            && Col >= a.Col
            && Row >= a.Row
            && Col <= b.Col
            && Row <= b.Row;

        public static bool operator ==(ColRow left, ColRow right) => left.Equals(right);
        public static bool operator !=(ColRow left, ColRow right) => !left.Equals(right);
    }
}
