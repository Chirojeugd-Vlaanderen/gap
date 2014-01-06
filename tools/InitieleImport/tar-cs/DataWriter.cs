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
using System.IO;

namespace tar_cs
{
    internal class DataWriter : IArchiveDataWriter
    {
        private readonly long size;
        private long remainingBytes;
        private bool canWrite = true;
        private readonly Stream stream;

        public DataWriter(Stream data, long dataSizeInBytes)
        {
            size = dataSizeInBytes;
            remainingBytes = size;
            stream = data;
        }

        public int Write(byte[] buffer, int count)
        {
            if(remainingBytes == 0)
            {
                canWrite = false;
                return -1;
            }
            int bytesToWrite;
            if(remainingBytes - count < 0)
            {
                bytesToWrite = (int)remainingBytes;
            }
            else
            {
                bytesToWrite = count;
            }
            stream.Write(buffer,0,bytesToWrite);
            remainingBytes -= bytesToWrite;
            return bytesToWrite;
        }

        public bool CanWrite
        {
            get
            {
                return canWrite;
            }
        }
    }
}