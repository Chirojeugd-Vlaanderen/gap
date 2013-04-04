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
using System.Diagnostics;
using System.Net;
using System.Text;
using tar_cs;

namespace tar_cs
{
    internal class TarHeader : ITarHeader
    {
        private readonly byte[] buffer = new byte[512];
        private long headerChecksum;

        public TarHeader()
        {
            // Default values
            Mode = 511; // 0777 dec
            UserId = 61; // 101 dec
            GroupId = 61; // 101 dec
        }

        private string fileName;
        protected readonly DateTime TheEpoch = new DateTime(1970, 1, 1, 0, 0, 0);

        public virtual string FileName
        {
            get
            {
                return fileName.Replace("\0",string.Empty);
            } 
            set
            {
                if(value.Length > 100)
                {
                    throw new TarException("A file name can not be more than 100 chars long");
                }
                fileName = value;
            }
        }
        public int Mode { get; set; }

        public string ModeString
        {
            get { return AddChars(Convert.ToString(Mode, 8), 7, '0', true); }
        }

        public int UserId { get; set; }
        public virtual string UserName
        {
            get { return UserId.ToString(); }
            set { UserId = Int32.Parse(value); }
        }

        public string UserIdString
        {
            get { return AddChars(Convert.ToString(UserId, 8), 7, '0', true); }
        }

        public int GroupId { get; set; }
        public virtual string GroupName
        {
            get { return GroupId.ToString(); }
            set { GroupId = Int32.Parse(value); }
        }

        public string GroupIdString
        {
            get { return AddChars(Convert.ToString(GroupId, 8), 7, '0', true); }
        }

        public long SizeInBytes { get; set; }

        public string SizeString
        {
            get { return AddChars(Convert.ToString(SizeInBytes, 8), 11, '0', true); }
        }

        public DateTime LastModification { get; set; }

        public string LastModificationString
        {
            get
            {
                return AddChars(
                    ((long) (LastModification - TheEpoch).TotalSeconds).ToString(), 11, '0',
                    true);
            }
        }

        public string HeaderChecksumString
        {
            get { return AddChars(Convert.ToString(headerChecksum, 8), 6, '0', true); }
        }


        public virtual int HeaderSize
        {
            get { return 512; }
        }

        private static string AddChars(string str, int num, char ch, bool isLeading)
        {
            int neededZeroes = num - str.Length;
            while (neededZeroes > 0)
            {
                if (isLeading)
                    str = ch + str;
                else
                    str = str + ch;
                --neededZeroes;
            }
            return str;
        }

        public byte[] GetBytes()
        {
            return buffer;
        }

        public virtual bool UpdateHeaderFromBytes()
        {
            FileName = Encoding.ASCII.GetString(buffer, 0, 100);
        	Mode = 511;
        	UserId = 61;
        	GroupId = 61;
            if((buffer[124] & 0x80) == 0x80) // if size in binary
            {
                long sizeBigEndian = BitConverter.ToInt64(buffer,0x80);
                SizeInBytes = IPAddress.NetworkToHostOrder(sizeBigEndian);
            }
            else
            {
                SizeInBytes = Convert.ToInt64(Encoding.ASCII.GetString(buffer, 124, 11).Trim(), 8);
            }
            long unixTimeStamp = Convert.ToInt64(Encoding.ASCII.GetString(buffer,136,11));
            LastModification = TheEpoch.AddSeconds(unixTimeStamp);

            var storedChecksum = Convert.ToInt32(Encoding.ASCII.GetString(buffer,148,6).Trim());
            RecalculateChecksum(buffer);
            if (storedChecksum == headerChecksum)
            {
                return true;
            }

            RecalculateAltChecksum(buffer);
            return storedChecksum == headerChecksum;
        }

        private void RecalculateAltChecksum(byte[] buf)
        {
            Encoding.ASCII.GetBytes("        ").CopyTo(buf, 148);
            headerChecksum = 0;
            foreach(byte b in buf)
            {
                if((b & 0x80) == 0x80)
                {
                    headerChecksum -= b ^ 0x80;
                }
                else
                {
                    headerChecksum += b;
                }
            }
        }

        public virtual byte[] GetHeaderValue()
        {
            // Clean old values
            int i = 0;
            while (i < 512)
            {
                buffer[i] = 0;
                ++i;
            }

            if (string.IsNullOrEmpty(FileName)) throw new TarException("FileName can not be empty.");
            if (FileName.Length >= 100) throw new TarException("FileName is too long. It must be less than 100 bytes.");

            // Fill header
            Encoding.ASCII.GetBytes(AddChars(FileName, 100, '\0', false)).CopyTo(buffer, 0);
            Encoding.ASCII.GetBytes(ModeString).CopyTo(buffer, 100);
            Encoding.ASCII.GetBytes(UserIdString).CopyTo(buffer, 108);
            Encoding.ASCII.GetBytes(GroupIdString).CopyTo(buffer, 116);
            Encoding.ASCII.GetBytes(SizeString).CopyTo(buffer, 124);
            Encoding.ASCII.GetBytes(LastModificationString).CopyTo(buffer, 136);

            RecalculateChecksum(buffer);

            // Write checksum
            Encoding.ASCII.GetBytes(HeaderChecksumString).CopyTo(buffer, 148);

            return buffer;
        }

        protected virtual void RecalculateChecksum(byte[] buf)
        {
// Set default value for checksum. That is 8 spaces.
            Encoding.ASCII.GetBytes("        ").CopyTo(buf, 148);

            // Calculate checksum
            headerChecksum = 0;
            foreach (byte b in buf)
            {
                headerChecksum += b;
            }
        }
    }
}