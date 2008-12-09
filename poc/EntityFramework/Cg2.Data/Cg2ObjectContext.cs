using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.Domain;

namespace Cg2.Data
{
    public class Cg2ObjectContext: global::System.Data.Objects.ObjectContext
    {
        #region entiteitcontainers
        private global::System.Data.Objects.ObjectQuery<Groep> _groepen;
        private global::System.Data.Objects.ObjectQuery<ChiroGroep> _chiroGroepen;
        #endregion

        #region constructors
        public Cg2ObjectContext() : base("name=Cg2ObjectContext", "Cg2ObjectContext") { }
        public Cg2ObjectContext(string connectionString) : base(connectionString, "Cg2ObjectContext") { }
        public Cg2ObjectContext(global::System.Data.EntityClient.EntityConnection connection) : base(connection, "Cg2ObjectContext") { }
        #endregion

        public global::System.Data.Objects.ObjectQuery<Groep> Groepen
        {
            get
            {
                if (this._groepen == null)
                {
                    this._groepen = base.CreateQuery<Groep>("[Groep]");
                }
                return this._groepen;
            }
        }

        public global::System.Data.Objects.ObjectQuery<ChiroGroep> ChiroGroepen
        {
            get
            {
                if (this._chiroGroepen == null)
                {
                    this._chiroGroepen = base.CreateQuery<ChiroGroep>("[ChiroGroep]");
                }
                return this._chiroGroepen;
            }
        }

    }
}
