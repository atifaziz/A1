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
        public bool IsRowAbs { get; }
        public bool IsColAbs { get; }
        public RowCol RowCol { get; }
        public Row Row => RowCol.Row;
        public Col Col => RowCol.Col;

        public Address(Row row, Col col) :
            this(row, col, false) {}

        public Address(RowCol rc) :
            this(rc.Row, rc.Col, false) {}

        public Address(Row row, Col col, bool isAbs) :
            this(isAbs, row, isAbs, col) {}

        public Address(Col col, Row row, bool isAbs) :
            this(row, col, isAbs) {}

        public Address(RowCol rc, bool isAbs) :
            this(isAbs, rc.Row, isAbs, rc.Col) {}

        public Address(bool isColAbs, Col col, bool isRowAbs, Row row) :
            this(isRowAbs, row, isColAbs, col) {}

        public Address(bool isRowAbs, Row row, bool isColAbs, Col col)
        {
            IsColAbs = isColAbs;
            IsRowAbs = isRowAbs;
            RowCol = new RowCol(row, col);
        }

        public bool Equals(Address other) =>
               IsColAbs == other.IsColAbs
            && IsRowAbs == other.IsRowAbs
            && RowCol.Equals(other.RowCol);

        public override bool Equals(object obj) =>
            obj is Address address && Equals(address);

        public override int GetHashCode() =>
            unchecked ((((IsColAbs.GetHashCode() * 397) ^ IsRowAbs.GetHashCode()) * 397) ^ RowCol.GetHashCode());

        public override string ToString() =>
              FormatAbs(IsColAbs) + A1Convert.NumberColumnAlpha(Col)
            + FormatAbs(IsRowAbs) + Row;

        public Address MakeAbsolute() => new Address(Row, Col, true);
        public Address MakeRelative() => new Address(Row, Col);

        public static bool operator ==(Address left, Address right) => left.Equals(right);
        public static bool operator !=(Address left, Address right) => !left.Equals(right);

        static string FormatAbs(bool abs) => abs ? "$" : null;

        [Obsolete("This method will be removed in the next version.")]
        public static Tuple<Address, Address> ParseA1Range(string range) =>
            ParseA1Range(range, Tuple.Create);

        public static T ParseA1Range<T>(string range, Func<Address, Address, T> selector) =>
            TryParseA1Range(range, (r, fs, fa, ts, ta) =>
            {
                if (fa == null || ta == null)
                    throw new FormatException($"'{(fa == null ? fs : ts)}' is not a valid A1 cell reference style in the range '{r}'.");
                return selector(fa.Value, ta.Value);
            });

        public static T TryParseA1Range<T>(string range, T error, Func<Address, Address, T> selector) =>
            TryParseA1Range(range, (r, fs, fa, ts, ta) => fa == null || ta == null
                                                        ? error
                                                        : selector(fa.Value, ta.Value));

        static T TryParseA1Range<T>(string range, Func<string, string, Address?, string, Address?, T> selector)
        {
            var index = range.IndexOf(':');
            if (index < 0)
            {
                var r = TryParseA1(range);
                return selector(range, range, r, range, r);
            }
            var fs = range.Substring(0, index);
            var from = TryParseA1(fs);
            var ts = range.Substring(index + 1);
            var to = TryParseA1(ts);
            return selector(range, fs, from, ts, to);
        }

        public static Address ParseA1(string s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            var result = TryParseA1(s);
            if (result == null)
                throw new FormatException($"'{s}' is not a valid A1 cell reference style.");
            return result.Value;
        }

        public static Address? TryParseA1(string s)
        {
            if (string.IsNullOrEmpty(s))
                return null;
            var abscol = s[0] == '$';
            var i = abscol ? 1 : 0;
            var ii = i;
            int ch;
            while (ii < s.Length && (ch = s[ii] & ~32) >= 'A' && ch <= 'Z')
                ii++;
            var len = ii - i;
            if (len == 0)
                return null;
            var col = A1Convert.TryAlphaColumnNumber(s.Substring(i, len)) ?? 0;
            if (col == 0)
                return null;
            if (ii == s.Length)
                return null;
            var absrow = s[ii] == '$';
            return int.TryParse(s.Substring(ii + (absrow ? 1 : 0)), NumberStyles.None, CultureInfo.InvariantCulture, out var row)
                && row >= 1
                 ? new Address(absrow, new Row(row), abscol, new Col(col))
                 : (Address?) null;
        }
    }
}
