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
﻿// Deze file bevat een aantal domme extension methods voor
// cg2.data.

using System.Runtime.Serialization;

namespace Cg2.Data.Nh
{
    /// <summary>
    /// Klasse met wat rap-rap statische methods
    /// </summary>
    public static partial class Utils
    {
        /// <summary>
        /// Maak een identieke kloon van een object, via 
        /// datacontractserializatie
        /// </summary>
        /// <typeparam name="T">Type van het object</typeparam>
        /// <param name="originalObject">Te klonen object</param>
        /// <returns>Identieke kloon</returns>
        public static T DeepClone<T>(this T originalObject)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                var binaryFormatter = new DataContractSerializer(typeof(T));
                binaryFormatter.WriteObject(stream, originalObject);
                stream.Position = 0;
                return (T)binaryFormatter.ReadObject(stream);
            }
        }
    }
}