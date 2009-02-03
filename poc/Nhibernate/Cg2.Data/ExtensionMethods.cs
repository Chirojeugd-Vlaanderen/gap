// Deze file bevat een aantal domme extension methods voor
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