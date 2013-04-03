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
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.Objects.DataClasses;

using System.IO;
using System.Runtime.Serialization;

namespace ConsoleApplication1
{
    /// <summary>
    /// Extension method voor het entity framework
    /// </summary>
    public static class Cloner
    {
        /// <summary>
        /// Kloon t een entityobject
        /// </summary>
        /// <typeparam name="T">Klasse van het entityobject</typeparam>
        /// <param name="entityObject">Te klonen object</param>
        /// <returns>Identieke kopie van het entityobject</returns>
        public static T CloneSerializing<T>(this T entityObject) where T : EntityObject, new()
        {
            // het 'where'-stuk bepaalt dat T moet erven van 'EntityObject'.  De ', new()' bepaalt
            // dat T een constructor moet hebben zonder argumenten

            // Serializeer het object naar een memory stream, en deserializeer het naar de kloon
            DataContractSerializer serializer = new DataContractSerializer(entityObject.GetType());
            MemoryStream stream = new MemoryStream();

            serializer.WriteObject(stream, entityObject);
            stream.Position = 0;
            T clonedObject = (T)serializer.ReadObject(stream);
            return clonedObject;
        }
    }
}
