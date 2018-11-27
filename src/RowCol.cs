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
    using System.Diagnostics.Contracts;

    public readonly partial struct Col
    {
        public static RowCol operator +(Col col, Row row) => new RowCol(row, col);
    }

    public readonly partial struct Row
    {
        public static RowCol operator +(Row row, Col col) => new RowCol(row, col);
    }

    public readonly struct RowCol : IEquatable<RowCol>
    {
        public static readonly RowCol TopLeft = new RowCol(Row.First, Col.First);

        public Row Row { get; }
        public Col Col { get; }

        public RowCol(Col col, Row row) : this(row, col) {}
        public RowCol(Row row, Col col) { Row = row; Col = col; }

        public bool Equals(RowCol other) => Row == other.Row && Col == other.Col;
        public override bool Equals(object obj) => obj is RowCol col && Equals(col);
        public override int GetHashCode() => unchecked((Row * 397) ^ Col);

        public string FormatA1() => A1Convert.NumberColumnAlpha(Col) + Row;

        public override string ToString() => "(" + Row + "," + Col + ")";

        public bool IsContainedIn(RowCol a, RowCol b) =>
            a.Col <= b.Col
            && a.Row <= b.Row
            && Col >= a.Col
            && Row >= a.Row
            && Col <= b.Col
            && Row <= b.Row;

        [Pure]
        public (int Rows, int Cols) OffsetTo(RowCol other) =>
            OffsetTo(other, ValueTuple.Create);

        [Pure]
        public T OffsetTo<T>(RowCol other, Func<int, int, T> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return selector((int) other.Row - Row, (int) other.Col - Col);
        }

        [Pure]
        public (int Height, int Width) Size(RowCol other) =>
            Size(other, ValueTuple.Create);

        [Pure]
        public T Size<T>(RowCol other, Func<int, int, T> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            var y = (int) other.Row - Row;
            if (y < 0) throw new ArgumentException($"Resulting height between {this} and {other} would be negative.", nameof(other));
            var x = (int) other.Col - Col;
            if (x < 0) throw new ArgumentException($"Resulting width between {this} and {other} would be negative.", nameof(other));
            return selector(y + 1, x + 1);
        }

        [Pure]
        public void Deconstruct(out Row row, out Col col) =>
            (row, col) = (Row, Col);

        public static bool operator ==(RowCol left, RowCol right) => left.Equals(right);
        public static bool operator !=(RowCol left, RowCol right) => !left.Equals(right);
    }
}
