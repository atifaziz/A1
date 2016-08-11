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
    using System.Diagnostics;
    using System.Globalization;

    [DebuggerDisplay("Col = {Col}, Row = {Row}")]
    public struct ColRow : IEquatable<ColRow>
    {
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

        public string FormatA1() =>
            NumA1(Col + 1) + (Row + 1).ToString(CultureInfo.InvariantCulture);

        public override string ToString() =>
            $"({Col.ToString(CultureInfo.InvariantCulture)}, {Row.ToString(CultureInfo.InvariantCulture)}) = {FormatA1()}";

        public bool IsContainedIn(ColRow a, ColRow b) =>
            a.Col <= b.Col
            && a.Row <= b.Row
            && Col >= a.Col
            && Row >= a.Row
            && Col <= b.Col
            && Row <= b.Row;

        public static Tuple<ColRow, ColRow> ParseA1Range(string range) =>
            ParseA1Range(range, Tuple.Create);

        public static T ParseA1Range<T>(string range, Func<ColRow, ColRow, T> seletor)
        {
            var index = range.IndexOf(':');
            if (index < 0)
            {
                var r = ParseA1(range);
                return seletor(r, r);
            }
            var from = ParseA1(range.Substring(0, index));
            var to = ParseA1(range.Substring(index + 1));
            return seletor(@from, to);
        }

        public static ColRow ParseA1(string s)
        {
            if (s.Length == 0)
                goto error;
            var abs = s[0] == '$';
            var i = abs ? 1 : 0;
            var ii = i;
            while (ii < s.Length && s[ii] >= 'A' && s[ii] <= 'Z')
                ii++;
            var len = ii - i;
            if (len == 0)
                goto error;
            var col = A1Num(s.Substring(i, len));
            int row;
            if (ii == s.Length)
                goto error;
            if (s[ii] == '$')
                ii++;
            if (!int.TryParse(s.Substring(ii), NumberStyles.None, CultureInfo.InvariantCulture, out row))
                goto error;
            return new ColRow(col - 1, row - 1);
            error:
            throw new FormatException($"'{s}' is not a valid A1 cell reference style.");
        }

        static int A1Num(string a1)
        {
            var c1 = 0;
            var m = 1;
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var ch in a1)
            {
                var n = ch - 'A' + 1;
                c1 = c1 * m + n;
                m = 26;
            }
            return c1;
        }

        const int MaxColumn = 16384;
        const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static readonly string[] A1Cache = new string[MaxColumn];

        static string NumA1(int num)
        {
            if (num < 1 || num > MaxColumn)
                throw new ArgumentOutOfRangeException(nameof(num), $"Number must be between 1 and {MaxColumn}.");
            var i = num - 1;
            return A1Cache[i] ?? (A1Cache[i] = NumA1Core(num));
        }

        static string NumA1Core(int num)
        {
            // A..Z     = 1..26
            // AA..ZZ   = 27..702
            // AAA..XFD = 703..16384
            var chars = new char[num >= 703 ? 3 : num >= 27 ? 2 : 1];
            var i = chars.Length;
            do
            {
                num -= 1;
                var r = num % Alphabet.Length;
                num = num / Alphabet.Length;
                chars[--i] = Alphabet[r];
            } while (num > 0);
            return new string(chars);
        }
    }
}
