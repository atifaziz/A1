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

        public string FormatA1() => A1Convert.NumberColumnAlpha(Col + 1) + (Row + 1).ToString(CultureInfo.InvariantCulture);

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
            var col = A1Convert.AlphaColumnNumber(s.Substring(i, len));
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
    }
}
