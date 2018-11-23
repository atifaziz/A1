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

        public static (Address From, Address To) ParseA1Range(string range) =>
            ParseA1Range(range, ValueTuple.Create);

        public static T ParseA1Range<T>(string range, Func<Address, Address, T> selector) =>
            TryParseA1Range(range, 0,
                (r, i, _) => throw new FormatException($"The first address in the range '{r}' is not a valid A1 cell reference style. Parsing failed at position {i + 1}."),
                (r, i, _) => throw new FormatException($"The separator at position {i + 1} in the range '{r}' must be a colon (:)."),
                (r, i, _) => throw new FormatException($"The second address in the range '{r}' is not a valid A1 cell reference style. Parsing failed at position {i + 1}."),
                (r, i, _) => throw new FormatException($"The range '{r}' is incorrectly terminated at position {i + 1}."),
                selector);

        public static bool TryParseA1Range(string range, out Address from, out Address to)
        {
            bool parsed;
            (parsed, from, to) = TryParseA1Range(range, default, (a, b) => (true, a, b));
            return parsed;
        }

        public static (Address From, Address To)? TryParseA1Range(string range) =>
            TryParseA1Range(range, ((Address, Address)?) null, (from, to) => (from, to));

        public static T TryParseA1Range<T>(string range, T error, Func<Address, Address, T> selector)
        {
            T Error(string r, int i, T err) => err;
            return TryParseA1Range(range, error, Error, Error, Error, Error, selector);
        }

        static TResult TryParseA1Range<TContext, TResult>(string range,
            TContext context,
            Func<string, int, TContext, TResult> fromErrorSelector,
            Func<string, int, TContext, TResult> separatorErrorSelector,
            Func<string, int, TContext, TResult> toErrorSelector,
            Func<string, int, TContext, TResult> endErrorSelector,
            Func<Address, Address, TResult> selector)
            => !TryParseA1(range, 0, range.Length, out var i, out var from)
             ? fromErrorSelector(range, i, context)
             : i == range.Length
             ? selector(from, from)
             : range[i] != ':'
             ? separatorErrorSelector(range, i, context)
             : !TryParseA1(range, ++i, range.Length, out i, out var to)
             ? toErrorSelector(range, i, context)
             : i < range.Length
             ? endErrorSelector(range, i, context)
             : selector(from, to);

        public static Address ParseA1(string s)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            var result = TryParseA1(s);
            if (result == null)
                throw new FormatException($"'{s}' is not a valid A1 cell reference style.");
            return result.Value;
        }

        public static Address? TryParseA1(string s)
            => s != null && TryParseA1(s, 0, s.Length, out var i, out var address) && i == s.Length
             ? address
             : (Address?) null;

        public static bool TryParseA1(string s, int index, int endIndex, out int stopIndex, out Address result)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (index < 0 || index > s.Length) throw new ArgumentOutOfRangeException(nameof(index), index, null);
            if (endIndex < 0 || endIndex > s.Length) throw new ArgumentOutOfRangeException(nameof(endIndex), endIndex, null);
            if (index > endIndex) throw new ArgumentOutOfRangeException(nameof(index), index, null);

            bool abscol, absrow;
            var i = index;
            var address
                = endIndex > index
                  && A1Convert.TryAlphaColumnNumber(s, index + ((abscol = s[i] == '$') ? 1 : 0), endIndex, out i, out var col)
                  && i < endIndex && ((absrow = s[i] == '$') || s[i] >= '0' && s[i] <= '9')
                  && col <= A1Convert.MaxColumn && Int.TryParse(s, i + (absrow ? 1 : 0), endIndex, out i, out var row)
                  && row >= 1
                  ? new Address(abscol, new Col(col), absrow, new Row(row))
                  : (Address?) null;

            switch (address)
            {
                case Address a:
                    stopIndex = i;
                    result = a;
                    return true;
                default:
                    stopIndex = i;
                    result = default;
                    return false;
            }
        }
    }
}
