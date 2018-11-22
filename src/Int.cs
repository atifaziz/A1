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
    using System.Diagnostics;

    static class Int
    {
        public static bool TryParse(string s, int index, int endIndex, out int stopIndex, out int result)
        {
            Debug.Assert(s != null);
            Debug.Assert(index >= 0 && index <= s.Length);
            Debug.Assert(index >= 0 && index <= s.Length);
            Debug.Assert(index <= endIndex);

            result = 0;
            var i = index;
            for (; i < endIndex; i++)
            {
                var ch = s[i];
                var unit = ch - '0';
                if (unit < 0 || unit > 9)
                    break;
                result = result * 10 + unit;
            }

            stopIndex = i;
            return i > index;
        }
    }
}
