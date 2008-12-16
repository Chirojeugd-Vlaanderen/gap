using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Core.Domain;

namespace Cg2.Data
{
    public class Cg2ObjectContext: global::System.Data.Objects.ObjectContext
    {
        private global::System.Data.Objects.ObjectQuery<Groep> _groepen;

        #region constructors
        //public Cg2ObjectContext() : base(@"metadata=F:\development\cg2\poc\EntityFramework\WebServices\bin\Cg2Domain.csdl|F:\development\cg2\poc\EntityFramework\WebServices\bin\Cg2Domain.ssdl|F:\development\cg2\poc\EntityFramework\WebServices\bin\Cg2Domain.msl;provider=System.Data.SqlClient;provider connection string='Data Source=devserver;Initial Catalog=ChiroGroep;Integrated Security=True;MultipleActiveResultSets=True'") { }
        public Cg2ObjectContext() : base(@"metadata=F:\development\cg2\poc\EntityFramework\WebServices\bin\Cg2Domain.csdl|F:\development\cg2\poc\EntityFramework\WebServices\bin\Cg2Domain.ssdl|F:\development\cg2\poc\EntityFramework\WebServices\bin\Cg2Domain.msl;provider=System.Data.SqlClient;provider connection string='Data Source=DEVSERVER;Initial Catalog=ChiroGroep;Integrated Security=True;MultipleActiveResultSets=True'", "Cg2ObjectContext") 
        { } // (2de parameter verwijst naar Entity Container van csdl).

        //public Cg2ObjectContext() : base("name=Cg2ObjectContext", "Cg2ObjectContext") { }
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
    }
}
