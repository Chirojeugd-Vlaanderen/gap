// Code gevonden op http://www.codeproject.com/KB/architecture/attachobjectgraph.aspx
// en aangepast voor gebruik met IBasisEntiteit
// (waar ID == 0 <=> nieuwe entiteit)


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Metadata.Edm;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
using Arebis.Reflection;


namespace Cg2.EfWrapper.Entity
{
	/// <summary>
	/// Extension methods on ObjectContext.
	/// </summary>
	public static class ObjectContextExtension
	{
		/// <summary>
		/// Attaches an entire objectgraph to the context.
		/// </summary>
        /// <remarks>Als je achteraf context.SaveChanges aanroept,
        /// en je wil op zoek naar nieuwe ID's, kijk dan niet in
        /// je originele graaf, maar in de graaf van de return
        /// value van deze functie!</remarks>
		public static T AttachObjectGraph<T>(this ObjectContext context, T entity, params Expression<Func<T, object>>[] paths) where T: IBasisEntiteit
		{
			return AttachObjectGraphs(context, new T[] { entity }, paths)[0];
		}

        /// <summary>
        /// Attacht objectgraphs aan de context
        /// </summary>
        /// <typeparam name="T">Type van de te attachen entiteit(en),
        /// moet IBasisEntiteit implementeren.</typeparam>
        /// <param name="context">Objectcontext waaraan te attachen</param>
        /// <param name="entities">Collectie van entiteiten</param>
        /// <param name="paths">Lambda-expressies die aangeven welke relaties
        /// mee geattacht moeten worden.  
        /// Bijvoorbeeld: p=>p.Naam, p=>p.PersoonsAdres.First().Adres.
        /// (Die First staat er enkel om verder te kunnen navigeren, en
        /// moet hier niet geinterpreteerd worden als het eerste element.
        /// </param>
        /// <returns>Een array met geattachte objectgraphs</returns>
        /// <remarks>FIXME: de 'TeVerwijderen'-vlag voor de rootentity's
        /// wordt nog genegeerd.</remarks>
		public static T[] AttachObjectGraphs<T>(this ObjectContext context, IEnumerable<T> entities, params Expression<Func<T, object>>[] paths) where T:IBasisEntiteit
		{
			T[] unattachedEntities = entities.ToArray();
			T[] attachedEntities = new T[unattachedEntities.Length];
			Type entityType = typeof(T);

            // Deze method doet enkel iets als er daadwerkelijk
            // iets in unattachedEntities zit.

			if (unattachedEntities.Length > 0)
			{
				// Workaround to ensure the assembly containing the entity type is loaded:
				// (see: https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=3405138&SiteID=1)
				try { context.MetadataWorkspace.LoadFromAssembly(entityType.Assembly); }
				catch { }

                // In onderstaande region worden de al bestaande root
                // entity's gequery'd, om op die manier al in de
                // object context terecht te komen.
                //
                // Dit is een optimalisatie voor als er meerdere root
                // entity's zouden zijn.  Op die manier wordt de database
                // niet iedere keer opnieuw bevraagd bij
                // AddOrAttachInstance (zie na onderstaande region)

				#region Automatic preload root entities

				// Create a WHERE clause for preload the root entities:
				StringBuilder where = new StringBuilder("(1=0)");
				List<ObjectParameter> pars = new List<ObjectParameter>();
				int pid = 0;
				foreach(T entity in unattachedEntities)
				{
					// If the entity has an ID:

                    if (entity.ID != 0)
                    {
                        // Het zou kunnen dat de EntityKey verdwenen is, dus
                        // voor alle zekerheid genereren we hem terug.
                        //
                        // (Entity key verdwijnt typisch als webformgegevens
                        // aan een entity gebind worden.)

                        entity.EntityKey = context.CreateEntityKey(typeof(T).Name, entity);

                        //where.Append(" OR ((1=1)");
                        //foreach (EntityKeyMember keymember in entity.EntityKey.EntityKeyValues)
                        //{
                        //    string pname = String.Format("p{0}", pid++);
                        //    where.Append(" AND (it.[");
                        //    where.Append(keymember.Key);
                        //    where.Append("] = @");
                        //    where.Append(pname);
                        //    where.Append(")");
                        //    pars.Add(new ObjectParameter(pname, keymember.Value));
                        //}
                        //where.Append(")");

                        // Vervangen door:

                        string pname = String.Format("p{0}", pid++);
                        where.Append(String.Format(" OR it.ID = @{0}", pname));
                        pars.Add(new ObjectParameter(pname, entity.ID));

                        // en nu maar hopen dat het werkt :)

                    }
                    else
                    {
                        // In de oorspronkelijke code werd een nieuwe entiteit
                        // gemarkeerd door de entity key null te maken.  Om
                        // zeker te zijn, zet ik hem hier expliciet.

                        entity.EntityKey = null;
                    }
				}

                // Als er bestaande objecten waren, dan is er een where,
                // en zijn er queryparameters.  In dat geval voeren we
                // de query uit, zodat de bestaande objecten in de
                // objectcontext terecht komen.

				// If WHERE clause not empty, construct and execute query:
				if (pars.Count > 0)
				{
					// Construct query:
					ObjectQuery<T> query = (ObjectQuery<T>)context.PublicGetProperty(GetEntitySetName(context, typeof(T)));
					foreach (var path in paths)
						query = query.Include(path);
					query = query.Where(where.ToString(), pars.ToArray());

					// Execute query and load entities:
					//Console.WriteLine(query.ToTraceString());
					query.Execute(MergeOption.AppendOnly).ToArray();
				}

				#endregion Automatic preload root entities

				// Attach the root entities:

                // De unattachedEntities (momenteel alles) worden geattacht
                // of geadd.  De geattachte exemplaren komen in 
                // attachedEntities terecht.
				for(int i=0; i<unattachedEntities.Length; i++)
					attachedEntities[i] = (T)context.AddOrAttachInstance(unattachedEntities[i], true);

                if (paths != null)
                {
                    // Collect property paths into a tree:

                    // Ik vermoed dat er een tree wordt gemaakt, waarbij
                    // de nodes de Properties zijn die de te attachen
                    // graaf bepalen.

                    TreeNode<ExtendedPropertyInfo> root = new TreeNode<ExtendedPropertyInfo>(null);
                    foreach (var path in paths)
                    {
                        List<ExtendedPropertyInfo> members = new List<ExtendedPropertyInfo>();
                        EntityFrameworkHelper.CollectRelationalMembers(path, members);
                        root.AddPath(members);
                    }

                    // Navigate over all properties:

                    // root bevat de tree, die bepaalt via welke property's de
                    // graaf opgebouwd wordt.  NavigatePropertySet zal de graaf
                    // bekijken in unattachedEntities, en reconstrueren in
                    // attachedEntities.

                    for (int i = 0; i < unattachedEntities.Length; i++)
                        NavigatePropertySet(context, root, unattachedEntities[i], attachedEntities[i]);
                }
			}

			// Return the attached root entities:
			return attachedEntities;
		}

		/// <summary>
        /// Adds or attaches the entity to the context. If the entity has an EntityKey,
        /// the entity is attached, otherwise a clone of it is added.
		/// </summary>
		/// <param name="context">context waaraan te attachen</param>
		/// <param name="entity">te attachen entiteit</param>
		/// <param name="applyPropertyChanges">geeft aan of property changes in de
        /// context gemarkeerd moeten worden</param>
		/// <returns>De geattachte entiteit</returns>
		public static IBasisEntiteit AddOrAttachInstance(this ObjectContext context, IBasisEntiteit entity, bool applyPropertyChanges)
		{
			if (entity.ID == 0)
			{
				IBasisEntiteit attachedEntity = GetShallowEntityClone(entity) as IBasisEntiteit;

                Debug.Assert(attachedEntity != null);

				context.AddObject(context.GetEntitySetName(entity.GetType()), attachedEntity);
				entity.EntityKey = attachedEntity.EntityKey;
                
				return attachedEntity;
			}
			else
			{
                EntityKey entityKey = entity.EntityKey;

				IBasisEntiteit attachedEntity = context.GetObjectByKey(entityKey) as IBasisEntiteit;

                Debug.Assert(attachedEntity != null);

				if (applyPropertyChanges)
					context.ApplyPropertyChanges(entityKey.EntitySetName, entity);
				return attachedEntity;
			}
		}

		/// <summary>
		/// Detaches an objectgraph given it's root object.
		/// </summary>
		/// <returns>The detached root object.</returns>
		public static T DetachObjectGraph<T>(this ObjectContext context, T entity)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				//NetDataContractSerializer serializer = new NetDataContractSerializer();
				//serializer.Serialize(stream, entity);
				//stream.Position = 0;
				//return (T)serializer.Deserialize(stream);
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, entity);
				stream.Position = 0;
				return (T)formatter.Deserialize(stream);
			}
		}

        /// <summary>
        /// Returns the EntitySetName for the given entity type.
        /// </summary>
        public static string GetEntitySetName(this ObjectContext context, Type entityType)
        {
            Type type = entityType;

            while (type != null)
            {
                // Use first EdmEntityTypeAttribute found:
                foreach (EdmEntityTypeAttribute typeattr in type.GetCustomAttributes(typeof(EdmEntityTypeAttribute), false))
                {
                    // Retrieve the entity container for the conceptual model:
                    var container = context.MetadataWorkspace.GetEntityContainer(context.DefaultContainerName, DataSpace.CSpace);

                    // Retrieve the name of the entityset matching the given types EdmEntityTypeAttribute:
                    string entitySetName = (from meta in container.BaseEntitySets
                                            where meta.ElementType.FullName == typeattr.NamespaceName + "." + typeattr.Name
                                            select meta.Name)
                                            .FirstOrDefault();

                    // If match, return the entitySetName:
                    if (entitySetName != null) return entitySetName;
                }

                // If no matching attribute or entitySetName found, try basetype:
                type = type.BaseType;
            }

            // Fail if no valid entitySetName was found:
            throw new InvalidOperationException(String.Format("Unable to determine EntitySetName of type '{0}'.", entityType));
        }

		#region Entity path marker methods

		/// <summary>
		/// Marker method to indicate this section of the path expression
		/// should not be loaded but only referenced.
		/// </summary>
		public static object ReferenceOnly(this IEntityWithKey entity)
		{
			throw new InvalidOperationException("The ReferenceOnly() method is a marker method in entity property paths and should not be effectively invoked.");
		}

		/// <summary>
		/// Marker method to indicate the instances the method is called on
		/// within path expressions should not be updated.
		/// </summary>
		public static object WithoutUpdate(this IEntityWithKey entity)
		{
			throw new InvalidOperationException("The WithoutUpdate() method is a marker method in entity property paths and should not be effectively invoked.");
		}

		#endregion

		#region Private implementation

        /// <summary>
 		/// Navigates a property path on detached instance to translate into attached instance.
        /// </summary>
        /// <param name="context">Objectcontext</param>
        /// <param name="propertynode">tree met daarin de property's die de graaf bepalen</param>
        /// <param name="owner">entiteit voor niet-geattachte graaf</param>
        /// <param name="attachedowner">entiteit voor geattachte graaf;
        /// bevat initieel enkel de parent.</param>
		private static void NavigatePropertySet(ObjectContext context, TreeNode<ExtendedPropertyInfo> propertynode, IBasisEntiteit owner, IBasisEntiteit attachedowner)
		{
			// Try to navigate each of the properties:

            // Dit is een recursieve functie.  Zolang er kinderen zijn in
            // propertynode, worden alle overeenkomstige property's uit
            // owner geattacht in attachedowner.

			foreach (TreeNode<ExtendedPropertyInfo> childnode in propertynode.Children)
			{
				ExtendedPropertyInfo property = childnode.Item;

                // property is de property van owner die we willen
                // attachen aan attachedowner.

				// Retrieve property value:
                object related = property.PropertyInfo.GetValue(owner, null);

				if (related is IEnumerable)
				{
					// Load current list in context:

                    // Het gaat om een 1-op-veel-relatie.  Als dat niet het geval is,
                    // wordt de veel-kant van attachedowner in de context geladen.

					object attachedlist = property.PropertyInfo.GetValue(attachedowner, null);
					RelatedEnd relatedEnd = (RelatedEnd)attachedlist;
					if (((EntityObject)attachedowner).EntityState != EntityState.Added && !relatedEnd.IsLoaded)
						relatedEnd.Load();

                    // attachedlist bevat de gerelateerde entity's van 
                    // het geattachte object

					// Recursively navigate through new members:
					List<IBasisEntiteit> newlist = new List<IBasisEntiteit>();
                    
					foreach (object relatedinstance in (IEnumerable)related)
					{
                        IBasisEntiteit be = relatedinstance as IBasisEntiteit;

                        // We gaan er in ons project van uit dat elke entiteit erft
                        // van IBasisEntiteit.  Is dat niet het geval, dan is er iets mis.

                        Debug.Assert(be != null);

                        // Als bovenstaande assertie failt, dan kan ben je
                        // waarschijnlijk vergeten een partial entityklasse
                        // uit te breiden met de IBasisEntiteitdingen.
                        // Kijk tijdens het debuggen naar het type van
                        // relatedinstance, en je weet dewelke het is.

                        // Attach of creeer gerelateerde entiteit van de
                        // veel-kant, en bewaar dat in newlist

						IBasisEntiteit attachedinstance = context.AddOrAttachInstance(be, !property.NoUpdate);
						newlist.Add(attachedinstance);

                        // Recursief aanroepen van NavigatePropertySet op de
                        // individuele entity's aan de veel-kant

						NavigatePropertySet(context, childnode, be, attachedinstance);
					}

					// Synchronise lists:
					IList<IBasisEntiteit> removedItems;

                    Debug.Assert(attachedlist is IEnumerable);
					SyncList((IEnumerable)attachedlist, newlist, out removedItems);

					// Delete removed items if association is owned:
					if (AssociationEndBehaviorAttribute.GetAttribute(property.PropertyInfo).Owned)
					{
						foreach (var removedItem in removedItems)
							context.DeleteObject(removedItem);
					}

				}
				else if (!typeof(IEnumerable).IsAssignableFrom(property.PropertyInfo.PropertyType))
				{
                    // 1-op-1-relatie, of '1-kant' van 1-op-veelrelatie

                    // Als dat nog niet het geval is, en het 'child' is niet nieuw, 
                    // wordt eerst de bestaande property aan de context geattacht,

					// Load reference of currently attached in context:
					RelatedEnd relatedEnd = (RelatedEnd)attachedowner.PublicGetProperty(property.PropertyInfo.Name + "Reference");
					if (((EntityObject)attachedowner).EntityState != EntityState.Added && !relatedEnd.IsLoaded)
						relatedEnd.Load();

					// Recursively navigate through new value (unless it's null):
					IBasisEntiteit attachedinstance;
					if (related == null)
						attachedinstance = null;
					else
					{
                        Debug.Assert(related is IBasisEntiteit);

						attachedinstance = context.AddOrAttachInstance(related as IBasisEntiteit, !property.NoUpdate);

                        // recursieve aanroep voor eventuele children van property
						NavigatePropertySet(context, childnode, related as IBasisEntiteit, attachedinstance);
					}

					// Synchronise value:
					property.PropertyInfo.SetValue(attachedowner, attachedinstance, null);
				}
			}
		}

		/// <summary>
		/// Returns a shallow clone of only the scalar properties.
		/// </summary>
		private static object GetShallowEntityClone(object entity)
		{
			object clone = Activator.CreateInstance(entity.GetType());
			foreach (PropertyInfo prop in entity.GetType().GetProperties())
			{
				if (typeof(RelatedEnd).IsAssignableFrom(prop.PropertyType)) continue;
				//if (typeof(EntityReference).IsAssignableFrom(prop.PropertyType)) continue;
				//if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && (!typeof(String).IsAssignableFrom(prop.PropertyType))) continue;
				if (typeof(IEntityWithKey).IsAssignableFrom(prop.PropertyType)) continue;
				try
				{
					prop.SetValue(clone, prop.GetValue(entity, null), null);
				}
				catch
				{
				}
			}
			return clone;
		}

		/// <summary>
        /// Synchronises a targetlist with a sourcelist by adding or removing items from the targetlist.
        /// The targetlist is untyped and controlled through reflection.
        /// 
        /// Oorspronkelijk werd er uit target verwijderd wat er niet
        /// in source zit.  Maar nu kijk ik naar de TeVerwijderen property
        /// van target zelf.
		/// </summary>
		/// <param name="targetlist">een-op-veel property van een bestaande 
        /// geattachte entiteit, zoals die uit de database gehaald is</param>
		/// <param name="sourcelist">diezelfde een-op-veel property, maar
        /// zoals het zou moeten zijn.  Ook hier zijn alle gerelateerde
        /// entity' s geattacht</param>
		/// <param name="removedItems">hierin zullen de te verwijderen objecten
        /// bewaard worden</param>
		private static void SyncList(IEnumerable targetlist, IList<IBasisEntiteit> sourcelist, out IList<IBasisEntiteit> removedItems)
		{
			List<IBasisEntiteit> toremove = new List<IBasisEntiteit>();

            // targetlist: zo zit het nu in de database
            // sourcelist: zo zouden we het willen
            // bedoeling:
            // targetlist: zelfde items als sourcelist
            // toremove: de te verwijderen entity's

            // De oorspronkelijke code werkte niet met IBasisEntiteit,
            // maar deze aangepaste code wel.  Daarom kan ik het
            // oorspronkelijke (hieronder uitgecommentarieerd) op een
            // gemakkelijkere manier realiseren.

            // List<IBasisEntiteit> localsourcelist = new List<IBasisEntiteit>(sourcelist);
            //
            //// Compare both lists:
            //foreach (object item in targetlist)
            //{
            //    bool found = false;
            //    for (int i = 0; i < localsourcelist.Count; i++)
            //    {
            //        if (Object.ReferenceEquals(localsourcelist[i], item))
            //        {
            //            localsourcelist[i] = null;
            //            found = true;
            //        }
            //    }
            //    if (!found)
            //        toremove.Add(item);
            //}
            //
            //// Add members not in targetlist:
            //foreach (object item in localsourcelist)
            //{
            //    if (Object.ReferenceEquals(item, null) == false)
            //        targetlist.PublicInvokeMethod("Add", item);
            //}

            //// Remove members not in sourcelist:
            //foreach (object item in toremove)
            //    targetlist.PublicInvokeMethod("Remove", item);

            for (int i = 0; i < sourcelist.Count; ++i)
            {
                // Ik hoop dat de info in TeVerwijderen niet
                // in de vlucht is verdwenen.

                // FIXME: die Add en Remove moeten toch ook
                // aangeroepen kunnen worden zonder reflectie.

                if (sourcelist[i].TeVerwijderen)
                {
                    toremove.Add(sourcelist[i]);
                    targetlist.PublicInvokeMethod("Remove", sourcelist[i]);
                }
                else
                {
                    // Zou ik moeten checken of sourcelist[i] al voorkomt
                    // in targetlist? In het beste geval negeert EF
                    // automatisch dubbele add's...

                    targetlist.PublicInvokeMethod("Add", sourcelist[i]);
                }
            }

			// Expose removed items:
			removedItems = toremove;
		}

		#endregion
	}
}
