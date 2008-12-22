using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using System.Data;
using System.Runtime.Serialization;

namespace Core.Domain
{
    /// <summary>
    /// Dit is de basisklasse voor alle entiteiten in het model.
    /// Het is een abstracte klasse; je kan enkel subklasses instantieren.
    /// 
    /// Aangepast uit 'Pro LINQ Object Relational Mapping with C# 2008'.
    /// 
    /// 'Persistence Ignorance' gaat niet samen met het Entity Framework;
    /// we moeten bepaalde interfaces implementeren, en onze entiteiten zullen
    /// bepaalde attributen moeten gebruiken.
    /// </summary>
    /// <typeparam name="IdT">Type van het ID dat voor de klasse gebruikt zal
    /// worden (int, string, whatever,...). </typeparam>
    ///
    [DataContract]
    public abstract class BasisEntiteit: IEntityWithRelationships, IEntityWithChangeTracker, IEntityWithKey
    {
        #region Entity Framework overhead

        // Implementatie van de interfaces
        // TODO: Ik heb de access modifiers gewoon overgenomen uit het boek,
        // maar dat moet toch eens nagekeken worden denk ik.
        
        RelationshipManager _relationships = null;
        IEntityChangeTracker _changeTracker = null;
        EntityKey _entityKey = null;

        RelationshipManager IEntityWithRelationships.RelationshipManager
        {
            get
            {
                if (_relationships == null)
                {
                    _relationships = RelationshipManager.Create(this);
                }
                return _relationships;
            }
        }

        void IEntityWithChangeTracker.SetChangeTracker
            (IEntityChangeTracker changeTracker)
        {
            _changeTracker = changeTracker;
        }

        public void PropertyChanging(string propName)
        {
            if (_changeTracker != null)
            {
                _changeTracker.EntityMemberChanging(propName);
            }
        }

        public void PropertyChanged(string propName)
        {
            if (_changeTracker != null)
            {
                _changeTracker.EntityMemberChanged(propName);
            }
        }

        [DataMember]
        System.Data.EntityKey IEntityWithKey.EntityKey
        {
            get
            {
                return _entityKey;
            }
            set
            {
                // Set the EntityKey property, if it is not set.
                // Report the change if the change tracker exists.
                if (_changeTracker != null)
                {
                    _changeTracker.EntityMemberChanging("-EntityKey-");
                    _entityKey = value;
                    _changeTracker.EntityMemberChanging("-EntityKey-");
                }
                else
                {
                    _entityKey = value;
                }
            }
        }
        #endregion

        public const int DefaultID = 0;

        private int _id = DefaultID;

        /// <summary>
        /// There are no comments for Property GroepID in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty = true, IsNullable = false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public virtual int ID
        {
            get
            {
                return this._id;
            }
            set
            {
                this.PropertyChanging("ID");
                this._id = value;
                this.PropertyChanged("ID");
            }
        }

        public BasisEntiteit()
        {
        }

        /// <summary>
        /// Vergelijkt deze entiteit met een andere.  Twee entiteiten zijn
        /// gelijk als geen van beide null is, ze beiden hetzelfde ID hebben
        /// of beiden 'transient' zijn met dezelfde 'business signature'.
        /// </summary>
        /// <param name="obj">Entiteit om mee te vergelijken</param>
        /// <returns>True indien deze entiteit dezlefde is als 'obj'.</returns>
        public sealed override bool Equals(object obj)
        {
            BasisEntiteit teVergelijken = obj as BasisEntiteit;

            return (teVergelijken != null) 
                && (HeeftZelfdeNietStandaardId(teVergelijken)
                || ((IsTransient() || teVergelijken.IsTransient())
                    && HeeftZelfdeBusinessSignature(teVergelijken)));
        }

        /// <summary>
        /// Controleert of de entiteit 'transient' is.  Een 'transiente'
        /// entiteit is niet gekoppeld met de database, en heeft geen ID.
        /// </summary>
        /// <returns>True voor transiente entiteiten</returns>
        public bool IsTransient()
        {
            return ID == DefaultID;
        }

        /// <summary>
        /// Dit is overgenomen uit het boek.  Maar aangezien we iets verderop
        /// 'ToString' overriden, denk ik dat alle entity's van hetzelfde type
        /// zo dezelfde hashcode zullen krijgen.  Zou dat de bedoeling zijn?
        /// 
        /// Misschien moeten de entiteiten zelf ToString nog eens overriden...
        /// </summary>
        /// <returns>een hashcode voor deze entiteit.</returns>
        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// Vergelijkt de hashcode van deze entiteit met die van een andere.
        /// </summary>
        /// <param name="teVergelijken">entiteit waarmee vergeleken moet 
        /// worden</param>
        /// <returns>true indien beide entiteiten dezelfde hashcode 
        /// hebben.</returns>
        private bool HeeftZelfdeBusinessSignature(BasisEntiteit teVergelijken)
        {
            return GetHashCode().Equals(teVergelijken.GetHashCode());
        }

        /// <summary>
        /// Controleert of deze entiteit hetzelfde 'echte' (niet 0 of null) ID
        /// heeft als een andere entiteit.
        /// </summary>
        /// <param name="teVergelijken">entiteit waarvan ID vergeleken moet
        /// worden</param>
        /// <returns>true indien ID's gelijk en niet 0/null zijn</returns>
        private bool HeeftZelfdeNietStandaardId(BasisEntiteit teVergelijken)
        {
            return (ID != DefaultID)
                && (teVergelijken.ID != DefaultID)
                && ID == teVergelijken.ID;
        }

        /// <summary>
        /// Bepaal de stringrepresentatie van entiteit.
        /// </summary>
        /// <returns>de klassenaam van het object</returns>
        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            // FIXME: Onderstaande lijn is nog altijd niet goed genoeg,
            // want deze ToString wordt bepaald aan de hand van de
            // hashcode, en die wordt dan weer gebruikt om te kijken of
            // objecten met ID 0 verschillend zijn.  Met onderstaande
            // 'ToString' krijgen objecten met ID 0 steeds dezelfde
            // hashcode.

            str.Append(" Klasse: ").Append(GetType().FullName).Append(ID.ToString());
            return str.ToString();
        }
    }
}
