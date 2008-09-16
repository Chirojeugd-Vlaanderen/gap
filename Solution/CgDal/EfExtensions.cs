using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.Objects.DataClasses;

using System.IO;
using System.Runtime.Serialization;

namespace CgDal
{
    /// <summary>
    /// Een klasse met extension methods voor het Entity Framework.  Deze 
    /// methods zijn nodig om het E.F. te gebruiken in een layered omgeving.
    /// </summary>
    public static class EfExtensions
    {
        /// <summary>
        /// Kloont een entityobject
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

        /// <summary>
        /// Volgens artikel 
        /// http://blogs.msdn.com/cesardelatorre/archive/2008/09/04/updating-data-using-entity-framework-in-n-tier-and-n-layer-applications-short-lived-ef-contexts.aspx:
        /// 
        /// This other method is actually another extensor method which 
        /// applies all my entity property changes but into all the 
        /// referenced entities within our EF model
        /// 
        /// Ik denk dat het zo ineen zit:
        /// 
        /// Het oude object wordt eerst opnieuw aan de context gekoppeld.
        /// De bedoeling is dat alle 'echte' properties, en alle
        /// referentieobjecten van het nieuwe naar het oude object
        /// overgezet worden.  Het oude object houdt bij wat wijzigt,
        /// zodat context.SaveChanges de wijzigingen kan persisteren.
        /// 
        /// De 'echte' properties worden van nieuw naar oud gekopieerd
        /// via 'context.ApplyPropertyChanges'.  Deze method neemt echter
        /// geen referenties mee.  Vandaar deze extentiemethode, die
        /// de referenties van oud naar nieuw moet kopieren.
        /// 
        /// <remarks>
        /// Deze functie past enkel referenties naar objecten aan, geen
        /// collecties.
        /// 
        /// Verder denk ik dat de parameter 'context' voor niks nodig is.
        /// </remarks>
        /// 
        /// </summary>
        /// <param name="context">Objectcontext</param>
        /// <param name="newEntity">Al dan niet gewijzigde entityobject</param>
        /// <param name="oldEntity">Oorspronkelijke entityobject</param>
        public static void ApplyReferencePropertyChanges(this ObjectContext context
                                                         , IEntityWithRelationships newEntity
                                                         , IEntityWithRelationships oldEntity)
        {
            foreach (var relatedEnd in oldEntity.RelationshipManager.GetAllRelatedEnds())
            {
                var oldRef = relatedEnd as EntityReference;

                // We gebruiken 'relatedEnd as EntityReference', en niet '(EnityReference)relatedEnd'.
                // De 'as' operator geeft null als de conversie niet mogelijk is.
                //
                // In dit geval zou het kunnen dat 'relatedEnd' geen EntityReference is, maar een
                // collectie.  (Bijv: de leden van een groep.)  In dat geval wordt oldRef null.
                //
                // (De leden van een groep kunnen bijgevolg niet aangepast worden via het
                // groepsobject.  Als een lid bijv. aan een groep toegevoegd moet worden,
                // dan moet dit gebeuren via het lidobject.)

                if (oldRef != null)
                {
                    // Aangezien oldRef niet null is, hebben we te doen met
                    // een echte referentie; niet met een collectie.

                    var newRef = newEntity.RelationshipManager.GetRelatedEnd(oldRef.RelationshipName
                                                                             , oldRef.TargetRoleName) as EntityReference;
                    // vind de overeenkomstige nieuwe referentie, en vervang
                    // in het oude (geattachte) object.

                    oldRef.EntityKey = newRef.EntityKey;
                }
            }
        }
    }
}
