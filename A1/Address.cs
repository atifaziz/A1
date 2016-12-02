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

    public struct Address : IEquatable<Address>
    {
        public bool IsColAbs { get; }
        public bool IsRowAbs { get; }
        public ColRow ColRow { get; }
        public int Col => ColRow.Col;
        public int Row => ColRow.Row;

        public Address(int col, int row) :
            this(col, row, false) {}

        public Address(int col, int row, bool isAbs) :
            this(isAbs, col, isAbs, row) {}

        public Address(bool isColAbs, int col, bool isRowAbs, int row)
        {
            IsColAbs = isColAbs;
            IsRowAbs = isRowAbs;
            ColRow = new ColRow(col, row);
        }

        public bool Equals(Address other) =>
               IsColAbs == other.IsColAbs
            && IsRowAbs == other.IsRowAbs
            && ColRow.Equals(other.ColRow);

        public override bool Equals(object obj) =>
            obj is Address && Equals((Address) obj);

        public override int GetHashCode() =>
            unchecked ((((IsColAbs.GetHashCode() * 397) ^ IsRowAbs.GetHashCode()) * 397) ^ ColRow.GetHashCode());

        public override string ToString() =>
              FormatAbs(IsColAbs) + A1Convert.NumberColumnAlpha(Col + 1)
            + FormatAbs(IsRowAbs) + (Row + 1).ToString(CultureInfo.InvariantCulture);

        public Address MakeAbsolute() => new Address(Col, Row, true);
        public Address MakeRelative() => new Address(Col, Row);

        public static bool operator ==(Address left, Address right) => left.Equals(right);
        public static bool operator !=(Address left, Address right) => !left.Equals(right);

        static string FormatAbs(bool abs) => abs ? "$" : null;

        public static Tuple<Address, Address> ParseA1Range(string range) =>
            ParseA1Range(range, Tuple.Create);

        public static T ParseA1Range<T>(string range, Func<Address, Address, T> seletor)
        {
            var index = range.IndexOf(':');
            if (index < 0)
            {
                var r = ParseA1(range);
                return seletor(r, r);
            }
            var from = ParseA1(range.Substring(0, index));
            var to = ParseA1(range.Substring(index + 1));
            return seletor(from, to);
        }

        public static Address ParseA1(string s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (s.Length == 0)
                goto error;
            var abscol = s[0] == '$';
            var i = abscol ? 1 : 0;
            var ii = i;
            int ch;
            while (ii < s.Length && (ch = s[ii] & ~32) >= 'A' && ch <= 'Z')
                ii++;
            var len = ii - i;
            if (len == 0)
                goto error;
            var col = A1Convert.AlphaColumnNumber(s.Substring(i, len));
            int row;
            if (ii == s.Length)
                goto error;
            var absrow = s[ii] == '$';
            if (!int.TryParse(s.Substring(ii + (absrow ? 1 : 0)), NumberStyles.None, CultureInfo.InvariantCulture, out row))
                goto error;
            return new Address(abscol, col - 1, absrow, row - 1);
            error:
            throw new FormatException($"'{s}' is not a valid A1 cell reference style.");
        }
    }
}