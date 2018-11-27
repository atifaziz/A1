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

    [Flags]
    public enum AddressTraits
    {
        None,
        AbsoluteRow = 1,
        AbsoluteColumn = 2,
        Absolute = AbsoluteRow | AbsoluteColumn,
    }

    public readonly struct Address : IEquatable<Address>
    {
        public RowCol RowCol { get; }
        public AddressTraits Traits { get; }

        public Address(Row row, Col col) :
            this(row, col, AddressTraits.None) {}

        public Address(Row row, Col col, AddressTraits traits) :
            this(new RowCol(row, col), traits) {}

        [Obsolete("Use constructor accepting " + nameof(AddressTraits) + " instead.")]
        public Address(Row row, Col col, bool isAbs) :
            this(row, col, isAbs ? AddressTraits.Absolute : AddressTraits.None) {}

        [Obsolete("Use constructor accepting " + nameof(AddressTraits) + " instead.")]
        public Address(Col col, Row row, bool isAbs) :
            this(row, col, isAbs) {}

        [Obsolete("Use constructor accepting " + nameof(AddressTraits) + " instead.")]
        public Address(RowCol rc, bool isAbs) :
            this(rc.Row, rc.Col, isAbs ? AddressTraits.Absolute : AddressTraits.None) {}

        [Obsolete("Use constructor accepting " + nameof(AddressTraits) + " instead.")]
        public Address(bool isColAbs, Col col, bool isRowAbs, Row row) :
            this(row, col, GetTraits(isRowAbs, isColAbs)) {}

        [Obsolete("Use constructor accepting " + nameof(AddressTraits) + " instead.")]
        public Address(bool isRowAbs, Row row, bool isColAbs, Col col) :
            this(row, col, GetTraits(isRowAbs, isColAbs)) {}

        public Address(RowCol rc) :
            this(rc, AddressTraits.None) {}

        public Address(RowCol rc, AddressTraits traits) =>
            (RowCol, Traits) = (rc, traits);

        public Row Row => RowCol.Row;
        public Col Col => RowCol.Col;

        public bool IsRowAbs => HasTraits(AddressTraits.AbsoluteRow);
        public bool IsColAbs => HasTraits(AddressTraits.AbsoluteColumn);

        bool HasTraits(AddressTraits traits) => (Traits & traits) == traits;

        static AddressTraits GetTraits(bool isRowAbs, bool isColAbs)
            => AddressTraits.None
             | (isRowAbs ? AddressTraits.AbsoluteRow : AddressTraits.None)
             | (isColAbs ? AddressTraits.AbsoluteColumn : AddressTraits.None);

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

        public Address MakeAbsolute() => new Address(Row, Col, AddressTraits.Absolute);
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
            => !TryParseA1(range, 0, out var i, out var from)
             ? fromErrorSelector(range, i, context)
             : i == range.Length
             ? selector(from, from)
             : range[i] != ':'
             ? separatorErrorSelector(range, i, context)
             : !TryParseA1(range, ++i, out i, out var to)
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
            => s != null && TryParseA1(s, 0, out var i, out var address) && i == s.Length
             ? address
             : (Address?) null;

        public static bool TryParseA1(string s, int index, out int stopIndex, out Address result)
        {
            if (s == null) throw new ArgumentNullException(nameof(s));
            if (index < 0 || index > s.Length) throw new ArgumentOutOfRangeException(nameof(index), index, null);

            bool abscol, absrow;
            var i = index;
            var address
                = s.Length > index
                  && A1Convert.TryAlphaColumnNumber(s, index + ((abscol = s[i] == '$') ? 1 : 0), out i, out var col)
                  && i < s.Length && ((absrow = s[i] == '$') || s[i] >= '0' && s[i] <= '9')
                  && col <= A1Convert.MaxColumn && Int.TryParse(s, i + (absrow ? 1 : 0), out i, out var row)
                  && row >= 1
                  ? new Address(new Row(row), new Col(col), GetTraits(absrow, abscol))
                  : (Address?) null;

            stopIndex = i;

            switch (address)
            {
                case Address a:
                    result = a;
                    return true;
                default:
                    result = default;
                    return false;
            }
        }
    }
}
