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

        public Address(RowCol rc) :
            this(rc, AddressTraits.None) {}

        public Address(RowCol rc, AddressTraits traits) =>
            (RowCol, Traits) = (rc, traits);

        public Row Row => RowCol.Row;
        public Col Col => RowCol.Col;

        public bool IsRowAbs => HasTraits(AddressTraits.AbsoluteRow);
        public bool IsColAbs => HasTraits(AddressTraits.AbsoluteColumn);

        bool HasTraits(AddressTraits traits) => (Traits & traits) == traits;

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

        public static T ParseA1Range<T>(string range, Func<Address, Address, T> selector)
        {
            var (_, from, to) =
                TryParseA1Range(range, 0,
                    (r, i) => throw new FormatException($"The first address in the range '{r}' is not a valid A1 cell reference style. Parsing failed at position {i + 1}."),
                    (r, i) => throw new FormatException($"The separator at position {i + 1} in the range '{r}' must be a colon (:)."),
                    (r, i) => throw new FormatException($"The second address in the range '{r}' is not a valid A1 cell reference style. Parsing failed at position {i + 1}."),
                    (r, i) => throw new FormatException($"The range '{r}' is incorrectly terminated at position {i + 1}."),
                    ValueTuple.Create);

            return selector(from, to);
        }

        public static bool TryParseA1Range(string range, out Address from, out Address to) =>
            TryParseA1Range(range, 0, out _, out from, out to);

        public static (Address From, Address To)? TryParseA1Range(string range) =>
            TryParseA1Range(range, ((Address, Address)?) null, (from, to) => (from, to));

        public static T TryParseA1Range<T>(string range, T error, Func<Address, Address, T> selector)
        {
            if (range == null) throw new ArgumentNullException(nameof(range));
            if (selector == null) throw new ArgumentNullException(nameof(selector));

            return TryParseA1Range(range, 0, out _, out var from, out var to)
                 ? selector(from, to)
                 : error;
        }

        public static bool TryParseA1Range(string range, int index,
                                           out int stopIndex,
                                           out Address from, out Address to)
        {
            (bool, int, Address, Address)
                Error(string r, int i) =>
                    (false, i, default, default);

            bool success;
            (success, stopIndex, from, to) =
                TryParseA1Range(range, 0,
                                Error, Error, Error, Error,
                                (si, a, b) => (true, si, a, b));
            return success;
        }

        static T TryParseA1Range<T>(string range,
            int index,
            Func<string, int, T> fromErrorSelector,
            Func<string, int, T> separatorErrorSelector,
            Func<string, int, T> toErrorSelector,
            Func<string, int, T> endErrorSelector,
            Func<int, Address, Address, T> selector)
            => !TryParseA1(range, index, out var i, out var from)
             ? fromErrorSelector(range, i)
             : i == range.Length
             ? selector(i, from, from)
             : range[i] != ':'
             ? separatorErrorSelector(range, i)
             : !TryParseA1(range, ++i, out i, out var to)
             ? toErrorSelector(range, i)
             : i < range.Length
             ? endErrorSelector(range, i)
             : selector(i, from, to);

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
                  ? new Address(new Row(row), new Col(col), AddressTraits.None
                                                          | (absrow ? AddressTraits.AbsoluteRow : AddressTraits.None)
                                                          | (abscol ? AddressTraits.AbsoluteColumn : AddressTraits.None))
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
