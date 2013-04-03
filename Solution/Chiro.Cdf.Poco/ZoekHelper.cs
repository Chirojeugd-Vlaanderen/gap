/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
﻿using System;
using System.Text;

namespace Chiro.Cdf.Poco
{
    /// <summary>
    /// Helper methods voor zoekopdrachten in de repository's.
    /// (Als het ding maar een naam heeft)
    /// </summary>
    public static class ZoekHelper
    {
        /// <summary>
        /// Past het soundex-algoritme toe op <paramref name="data"/>
        /// Gebaseerd op http://www.techno-soft.com/index.php?/c-implementation-of-sql-difference-and-soundex
        /// </summary>
        /// <param name="data">te processen string</param>
        /// <returns>het resultaat van soundex, toegepast op <paramref name="data"/></returns>
        /// <remarks>
        /// We gebruiken soundex voor 'zoek op naam ongeveer', maar dat is waarschijnlijk niet optimaal.
        /// Al was het maar omdat soundex gemaakt is voor Engelse woorden, en het erg veel beperkingen
        /// heeft. Maar voorlopig is het dus maar zo.
        /// </remarks>
        public static string Soundex(string data)
        {
            var result = new StringBuilder();

            if (!string.IsNullOrEmpty(data))
            {
                result.Append(Char.ToUpper(data[0]));
                string previousCode = string.Empty;

                for (int i = 1; i < data.Length; i++)
                {
                    string currentCode = EncodeChar(data[i]);

                    if (currentCode != previousCode)
                        result.Append(currentCode);

                    if (result.Length == 4) break;

                    if (!currentCode.Equals(string.Empty))
                        previousCode = currentCode;
                }
            }

            if (result.Length < 4)
                result.Append(new String('0', 4 - result.Length));

            return result.ToString();
        }

        /// <summary>
        /// Encodeert <paramref name="c"/> voor soundex.
        /// </summary>
        /// <param name="c">te encoderen karakter</param>
        /// <returns>Waarde van <paramref name="c"/> voor soundex</returns>
        private static string EncodeChar(char c)
        {
            switch (Char.ToLower(c))
            {
                case 'b':
                case 'f':
                case 'p':
                case 'v':
                    return "1";
                case 'c':
                case 'g':
                case 'j':
                case 'k':
                case 'q':
                case 's':
                case 'x':
                case 'z':
                    return "2";
                case 'd':
                case 't':
                    return "3";
                case 'l':
                    return "4";
                case 'm':
                case 'n':
                    return "5";
                case 'r':
                    return "6";
                default:
                    return string.Empty;
            }
        }

    }
}
