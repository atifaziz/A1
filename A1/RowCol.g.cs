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

    partial struct Row : IEquatable<Row>, IComparable<Row>
    {
        public static readonly Row Zero = default(Row);

        readonly int _n;

        public Row(int n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));
            _n = n;
        }

        public bool Equals(Row other) => _n == other._n;
        public override bool Equals(object obj) => obj is Row && Equals((Row) obj);
        public override int GetHashCode() => _n;

        public int CompareTo(Row other) => _n.CompareTo(other._n);

        public override string ToString() => _n.ToString();

        public static Row operator +(Row col, int n) => new Row(col._n + n);
        public static Row operator -(Row col, int n) => new Row(col._n - n);
        public static Row operator ++(Row col) => col + 1;
        public static Row operator --(Row col) => col - 1;

        public static bool operator ==(Row left, Row right) => left.Equals(right);
        public static bool operator !=(Row left, Row right) => !left.Equals(right);

        public static explicit operator Row(int value) => new Row(value);
        public static implicit operator int(Row col) => col._n;
    }

    partial struct Col : IEquatable<Col>, IComparable<Col>
    {
        public static readonly Col Zero = default(Col);

        readonly int _n;

        public Col(int n)
        {
            if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));
            _n = n;
        }

        public bool Equals(Col other) => _n == other._n;
        public override bool Equals(object obj) => obj is Col && Equals((Col) obj);
        public override int GetHashCode() => _n;

        public int CompareTo(Col other) => _n.CompareTo(other._n);

        public override string ToString() => _n.ToString();

        public static Col operator +(Col col, int n) => new Col(col._n + n);
        public static Col operator -(Col col, int n) => new Col(col._n - n);
        public static Col operator ++(Col col) => col + 1;
        public static Col operator --(Col col) => col - 1;

        public static bool operator ==(Col left, Col right) => left.Equals(right);
        public static bool operator !=(Col left, Col right) => !left.Equals(right);

        public static explicit operator Col(int value) => new Col(value);
        public static implicit operator int(Col col) => col._n;
    }
}
