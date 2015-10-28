/*
 * Copyright 2015 Chirojeugd-Vlaanderen vzw. See the NOTICE file at the 
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
using System.Security.Cryptography;
using System.Text;

namespace Chiro.Mailchimp.Sync.Models
{
    public class Member
    {
        public string id
        {
            get
            {
                byte[] hash;
                using (var md5 = MD5.Create())
                {
                    hash = md5.ComputeHash(Encoding.UTF8.GetBytes(email_address));
                }
                var sb = new StringBuilder();
                foreach (var t in hash)
                {
                    sb.Append(t.ToString("x2"));
                }
                return sb.ToString();
            }
        }
        public string email_address { get; set; }
        public MergeFields merge_fields { get; set; }
        public string status { get; set; }
    }
}
