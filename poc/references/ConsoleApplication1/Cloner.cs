using System;
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
