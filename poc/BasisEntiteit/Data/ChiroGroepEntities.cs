using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Data
{
    /// <summary>
    /// There are no comments for ChiroGroepEntities in the schema.
    /// </summary>
    public partial class ChiroGroepEntities : global::System.Data.Objects.ObjectContext
    {
        /// <summary>
        /// Initializes a new ChiroGroepEntities object using the connection string found in the 'ChiroGroepEntities' section of the application configuration file.
        /// </summary>
        public ChiroGroepEntities() :
            base("name=ChiroGroepEntities", "ChiroGroepEntities")
        {
            this.OnContextCreated();
        }
        /// <summary>
        /// Initialize a new ChiroGroepEntities object.
        /// </summary>
        public ChiroGroepEntities(string connectionString) :
            base(connectionString, "ChiroGroepEntities")
        {
            this.OnContextCreated();
        }
        /// <summary>
        /// Initialize a new ChiroGroepEntities object.
        /// </summary>
        public ChiroGroepEntities(global::System.Data.EntityClient.EntityConnection connection) :
            base(connection, "ChiroGroepEntities")
        {
            this.OnContextCreated();
        }
        partial void OnContextCreated();
        /// <summary>
        /// There are no comments for Groep in the schema.
        /// </summary>
        public global::System.Data.Objects.ObjectQuery<Groep> Groep
        {
            get
            {
                if ((this._Groep == null))
                {
                    this._Groep = base.CreateQuery<Groep>("[Groep]");
                }
                return this._Groep;
            }
        }
        private global::System.Data.Objects.ObjectQuery<Groep> _Groep;
        /// <summary>
        /// There are no comments for Groep in the schema.
        /// </summary>
        public void AddToGroep(Groep groep)
        {
            base.AddObject("Groep", groep);
        }
    }

}
