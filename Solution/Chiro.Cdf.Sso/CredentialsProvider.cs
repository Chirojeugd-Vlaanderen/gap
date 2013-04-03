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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Chiro.Cdf.Sso
{
    /// <summary>
    /// De CredentialsProvider genereert credentials om mee aan te loggen op externe
    /// sites.
    /// </summary>
    public class CredentialsProvider
    {
        private readonly byte[] _encryptieSleutel;
        private readonly byte[] _hashSleutel;

        /// <summary>
        /// Standaardconstructor
        /// </summary>
        /// <param name="encryptieSleutel">Sleutel die gebruikt zal worden voor de AES-encryptie van de credentials</param>
        /// <param name="hashSleutel">Sleutel die gebruikt zal worden voor het signen van de geencrypteerde credentials</param>
        public CredentialsProvider(string encryptieSleutel, string hashSleutel)
        {
            _encryptieSleutel = Convert.FromBase64String(encryptieSleutel);
            _hashSleutel = Convert.FromBase64String(hashSleutel);
        }

        /// <summary>
        /// Levert credentials op voor single sign on.
        /// </summary>
        /// <param name="userInfo">Te encrypteren userinfo, in de vorm: naam;stamnr;e-mail;datum</param>
        /// <returns>Credentials, op basis van de info van de UserInfoProvider, geencrypteerd en gehasht
        /// met de respectievelijke sleutels.</returns>
        public Credentials Genereren(string userInfo)
        {
            string geencrypteerdeUserInfo = Encrypteren(userInfo);

            return new Credentials
                       {
                           GeencrypteerdeUserInfo = geencrypteerdeUserInfo,
                           Hash = Hashen(geencrypteerdeUserInfo)
                       };
        }

        /// <summary>
        /// Berekent de hmac-sha1-hash van <paramref name="tekst"/>, base-64-geencodeerd
        /// </summary>
        /// <param name="tekst">tekst waarvan hash te berekenen</param>
        /// <returns>hmac-sha1-hash van <paramref name="tekst"/>, base-64-geencodeerd</returns>
        private string Hashen(string tekst)
        {
            string resultaat;

            if (_hashSleutel == null)
            {
                throw new ArgumentException("Hashsleutel niet gezet.");
            }

            using (var hmacsha1 = new HMACSHA1(_hashSleutel))
            {
                var bytes = new UTF8Encoding().GetBytes(tekst);
                var gehasht = hmacsha1.ComputeHash(bytes);
                resultaat = Convert.ToBase64String(gehasht);
            }

            return resultaat;
        }

        /// <summary>
        /// AES-encrypteert <paramref name="bytes"/> met CBC-vercijfering, gebruik makende van een
        /// initialisatievector. Deze initialisatievector wordt op zijn beurt AES-geencrypteerd met
        /// ECB-vercijfering. Het resultaat van deze method is de concatenatie van de geencrypteerde
        /// vector en de geencrypteerde <paramref name="bytes"/>, als base-64 string.
        /// </summary>
        /// <param name="bytes">Te encrypteren tekst, UTF8-encoded</param>
        /// <returns>De concatenatie van de geencrypteerde
        /// vector en de geencrypterde <paramref name="bytes"/>.</returns>
        /// <remarks>Heeft het wel zin om met een initialisatievector te werken, als je die toch
        /// meegeeft in het resultaat? Of is dit enkel security by obscurity?</remarks>
        private string Encrypteren(byte[] bytes)
        {
            string resultaat;
            if (_encryptieSleutel == null)
            {
                throw new ArgumentException("Encryptiesleutel niet gezet.");
            }

            using (var aes = new AesManaged())
            {
                aes.Key = _encryptieSleutel;               
                aes.GenerateIV();   // initialisatievector genereren

                // Encrypteer bytes
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.Zeros;
                var cbcEncryptor = aes.CreateEncryptor();
                var geencrypteerdeBytes = Encrypteren(bytes, cbcEncryptor);

                aes.Mode = CipherMode.ECB;
                var ecbEncrpytor = aes.CreateEncryptor();
                var geencrypteerdeVector = Encrypteren(aes.IV, ecbEncrpytor);

                resultaat = Convert.ToBase64String(geencrypteerdeVector.Concat(geencrypteerdeBytes).ToArray());
            }

            return resultaat;
        }

        /// <summary>
        /// Gebruikt de meegeleverde <paramref name="encryptor"/> om <paramref name="bytes"/>
        /// te encrypteren.
        /// </summary>
        /// <param name="bytes">Te encrypteren bytes</param>
        /// <param name="encryptor">Encryptor</param>
        /// <returns>Geencrypteerde <paramref name="bytes"/></returns>
        private static IEnumerable<byte> Encrypteren(byte[] bytes, ICryptoTransform encryptor)
        {
            byte[] geencrypteerdeBytes;

            // constructie met streams overgenomen uit voorbeeld op MSDN:
            // http://msdn.microsoft.com/en-us/library/system.security.cryptography.aesmanaged.aspx

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    using (var binaryWriter = new BinaryWriter(cryptoStream))
                    {
                        // te encrypteren tekst in binaryWriter-stream duwen
                        binaryWriter.Write(bytes);
                    }
                    // geencrypteerde butes uit memorystream trekken
                    geencrypteerdeBytes = memoryStream.ToArray();
                }
            }
            return geencrypteerdeBytes;
        }

        /// <summary>
        /// AES-encrypteert <paramref name="tekst"/> met CBC-vercijfering, gebruik makende van een
        /// initialisatievector. Deze initialisatievector wordt op zijn beurt AES-geencrypteerd met
        /// ECB-vercijfering. Het resultaat van deze method is de concatenatie van de geencrypteerde
        /// vector en de geencrypteerde <paramref name="tekst"/>.
        /// </summary>
        /// <param name="tekst">Te encrypteren tekst</param>
        /// <returns>De concatenatie van de geencrypteerde
        /// vector en de geencrypterde <paramref name="tekst"/>.</returns>
        /// <remarks>Heeft het wel zin om met een initialisatievector te werken, als je die toch
        /// meegeeft in het resultaat? Of is dit enkel security by obscurity?</remarks>
        private string Encrypteren(string tekst)
        {
            return Encrypteren(new UTF8Encoding().GetBytes(tekst));
        }
    }
}
