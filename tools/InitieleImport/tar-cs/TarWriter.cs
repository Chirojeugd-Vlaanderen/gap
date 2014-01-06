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
using System.IO;

namespace tar_cs
{
    public class TarWriter : LegacyTarWriter
    {

        public TarWriter(Stream writeStream) : base(writeStream)
        {
        }

        protected override void WriteHeader(string name, DateTime lastModificationTime, long count, int userId, int groupId, int mode)
        {
            var tarHeader = new UsTarHeader()
            {
                FileName = name,
                LastModification = lastModificationTime,
                SizeInBytes = count,
                UserId = userId,
                UserName = Convert.ToString(userId,8),
                GroupId = groupId,
                GroupName = Convert.ToString(groupId,8),
                Mode = mode
            };
            OutStream.Write(tarHeader.GetHeaderValue(), 0, tarHeader.HeaderSize);
        }

        protected virtual void WriteHeader(string name, DateTime lastModificationTime, long count, string userName, string groupName, int mode)
        {
            var tarHeader = new UsTarHeader()
            {
                FileName = name,
                LastModification = lastModificationTime,
                SizeInBytes = count,
                UserId = userName.GetHashCode(),
                UserName = userName,
                GroupId = groupName.GetHashCode(),
                GroupName = groupName,
                Mode = mode
            };
            OutStream.Write(tarHeader.GetHeaderValue(), 0, tarHeader.HeaderSize);
        }


        public virtual void Write(string name, long dataSizeInBytes, string userName, string groupName, int mode, DateTime lastModificationTime, WriteDataDelegate writeDelegate)
        {
            var writer = new DataWriter(OutStream,dataSizeInBytes);
            WriteHeader(name, lastModificationTime, dataSizeInBytes, userName, groupName, mode);
            while(writer.CanWrite)
            {
                writeDelegate(writer);
            }
            AlignTo512(dataSizeInBytes, false);
        }


        public void Write(Stream data, long dataSizeInBytes, string fileName, string userId, string groupId, int mode,
                          DateTime lastModificationTime)
        {
            WriteHeader(fileName,lastModificationTime,dataSizeInBytes,userId, groupId, mode);
            WriteContent(dataSizeInBytes,data);
            AlignTo512(dataSizeInBytes,false);
        }
    }
}