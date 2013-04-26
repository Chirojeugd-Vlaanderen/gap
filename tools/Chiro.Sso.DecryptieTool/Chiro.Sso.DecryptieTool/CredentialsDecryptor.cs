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

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Chiro.Sso.DecryptieTool
{
    public class CredentialsDecryptor
    {
        private readonly byte[] _encryptieSleutel;
        private readonly byte[] _hashSleutel;

        /// <summary>
        /// Standaardconstructor
        /// </summary>
        /// <param name="encryptieSleutel">Sleutel die gebruikt was om credentials te encrypteren</param>
        /// <param name="hashSleutel">Sleutel die gebruikt was om credentials te hashen</param>
        public CredentialsDecryptor(string encryptieSleutel, string hashSleutel)
        {
            _encryptieSleutel = Convert.FromBase64String(encryptieSleutel);
            _hashSleutel = Convert.FromBase64String(hashSleutel);
        }

        /// <summary>
        /// Decrypteert de <paramref name="geencrypteerdeUserInfo"/>
        /// </summary>
        /// <param name="geencrypteerdeUserInfo">geencrypteerde user info</param>
        /// <returns>Gedecrypteerde userinformatie</returns>
        public UserInfo Decrypteren(string geencrypteerdeUserInfo)
        {
            byte[] bytes = Convert.FromBase64String(geencrypteerdeUserInfo);

            byte[] geencrypteerdeIv = bytes.Take(16).ToArray();    // eerste 16 bytes zijn geencrypteerde initialisatievector
            byte[] geencrypteerdeInfo = bytes.Skip(16).ToArray();  // de rest zijn de effectieve bytes

            byte[] iv;
            string info;

            using (var aes = new AesManaged())
            {
                aes.Key = _encryptieSleutel;
                // Constructie met streams; zie http://msdn.microsoft.com/en-us/library/system.security.cryptography.aesmanaged.aspx

                // Lees de initialisatievector

                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.Zeros;
                var ivDecryptor = aes.CreateDecryptor(aes.Key, null); // geen initialisatievector om initialisatievector te decrypten
                using (var memoryStream = new MemoryStream(geencrypteerdeIv))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, ivDecryptor, CryptoStreamMode.Read))
                    {
                        using (var binaryReader = new BinaryReader(cryptoStream))
                        {
                            iv = binaryReader.ReadBytes(geencrypteerdeIv.Length);
                        }
                    }
                }

                // Lees de user info

                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                var infoDecryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var memoryStream = new MemoryStream(geencrypteerdeInfo))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, infoDecryptor, CryptoStreamMode.Read))
                    {
                        using (var streamReader = new StreamReader(cryptoStream, Encoding.UTF8))
                        {
                            info = streamReader.ReadToEnd();
                        }
                    }
                }
            }

            var componenten = info.Split(';');

            return new UserInfo
                       {
                           Naam = componenten[0],
                           StamNr = componenten[1],
                           Email = componenten[2],
                           Datum = DateTime.Parse(componenten[3])
                       };

        }
    }
}