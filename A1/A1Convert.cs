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

    public static class A1Convert
    {
        public const int MaxColumn = 16384;

        const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        static readonly string[] A1Cache = new string[MaxColumn];

        public static int AlphaColumnNumber(string alpha)
        {
            if (alpha == null) throw new ArgumentNullException(nameof(alpha));
            var c1 = 0;
            var m = 1;
            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var ch in alpha)
            {
                if (ch < 'A' || ch > 'Z')
                    throw new ArgumentException("Input string may be only composed of characters from A through to Z.", nameof(alpha));
                var n = ch - 'A' + 1;
                c1 = c1 * m + n;
                m = 26;
            }
            return c1;
        }

        public static string NumberColumnAlpha(int number)
        {
            if (number < 1 || number > MaxColumn)
                throw new ArgumentOutOfRangeException(nameof(number), $"Number must be between 1 and {MaxColumn}.");
            var i = number - 1;
            return A1Cache[i] ?? (A1Cache[i] = ColumnAlphaCore(number));
        }

        static string ColumnAlphaCore(int num)
        {
            // A..Z     = 1..26
            // AA..ZZ   = 27..702
            // AAA..XFD = 703..16384
            var chars = new char[num >= 703 ? 3 : num >= 27 ? 2 : 1];
            var i = chars.Length;
            do
            {
                num -= 1;
                var r = num %Alphabet.Length;
                num = num /Alphabet.Length;
                chars[--i] = Alphabet[r];
            } while (num > 0);
            return new string(chars);
        }
    }
}