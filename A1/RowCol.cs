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

    public struct Col : IEquatable<Col>, IComparable<Col>
    {
        public static readonly Col Zero = default(Col);

        readonly int _x;

        public Col(int x)
        {
            if (x < 0) throw new ArgumentOutOfRangeException(nameof(x));
            _x = x;
        }

        public bool Equals(Col other) => _x == other._x;
        public override bool Equals(object obj) => obj is Col && Equals((Col) obj);
        public override int GetHashCode() => _x;

        public int CompareTo(Col other) => _x.CompareTo(other._x);

        public override string ToString() => _x.ToString();

        public static Col operator +(Col col, int x) => new Col(col._x + x);
        public static Col operator -(Col col, int x) => new Col(col._x - x);
        public static Col operator ++(Col col) => col + 1;
        public static Col operator --(Col col) => col - 1;

        public static RowCol operator +(Col col, Row row) => new RowCol(row, col);

        public static bool operator ==(Col left, Col right) => left.Equals(right);
        public static bool operator !=(Col left, Col right) => !left.Equals(right);

        public static explicit operator Col(int value) => new Col(value);
        public static implicit operator int(Col col) => col._x;
    }

    public struct Row : IEquatable<Row>, IComparable<Row>
    {
        public static readonly Row Zero = default(Row);

        readonly int _y;

        public Row(int y)
        {
            if (y < 0) throw new ArgumentOutOfRangeException(nameof(y));
            _y = y;
        }

        public bool Equals(Row other) => _y == other._y;
        public override bool Equals(object obj) => obj is Row && Equals((Row) obj);
        public override int GetHashCode() => _y;

        public int CompareTo(Row other) => _y.CompareTo(other._y);

        public override string ToString() => _y.ToString();

        public static Row operator +(Row row, int y) => new Row(row._y + y);
        public static Row operator -(Row row, int y) => new Row(row._y - y);
        public static Row operator ++(Row row) => row + 1;
        public static Row operator --(Row row) => row - 1;

        public static RowCol operator +(Row row, Col col) => new RowCol(row, col);

        public static bool operator ==(Row left, Row right) => left.Equals(right);
        public static bool operator !=(Row left, Row right) => !left.Equals(right);

        public static explicit operator Row(int value) => new Row(value);
        public static implicit operator int(Row row) => row._y;
    }

    public struct RowCol : IEquatable<RowCol>
    {
        public static readonly RowCol Zero = new RowCol(Row.Zero, Col.Zero);

        public Row Row { get; }
        public Col Col { get; }

        public RowCol(Row row, Col col) { Col = col; Row = row; }

        public bool Equals(RowCol other) => Col == other.Col && Row == other.Row;
        public override bool Equals(object obj) => obj is RowCol && Equals((RowCol) obj);
        public override int GetHashCode() => unchecked((Col * 397) ^ Row);

        public string FormatA1() => A1Convert.NumberColumnAlpha(Col + 1) + (Row + 1);

        public override string ToString() => "(" + Row + "," + Col + ")";

        public bool IsContainedIn(RowCol a, RowCol b) =>
            a.Col <= b.Col
            && a.Row <= b.Row
            && Col >= a.Col
            && Row >= a.Row
            && Col <= b.Col
            && Row <= b.Row;

        [Pure]
        public T OffsetTo<T>(RowCol other, Func<int, int, T> selector)
        {
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            return selector((int) other.Row - Row, (int) other.Col - Col);
        }

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

        public static bool operator ==(RowCol left, RowCol right) => left.Equals(right);
        public static bool operator !=(RowCol left, RowCol right) => !left.Equals(right);
    }
}
