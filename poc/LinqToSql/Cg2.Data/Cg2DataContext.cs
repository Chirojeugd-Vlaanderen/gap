using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq.Mapping;
using System.Configuration;
using Cg2.Core.Domain;

namespace Cg2.Data.LTS
{
    public sealed class Cg2DataContext: System.Data.Linq.DataContext
    {
        static XmlMappingSource map 
            = XmlMappingSource.FromXml(System.IO.File.ReadAllText(@"f:\development\cg2\poc\LinqToSql\Cg2.Data\ChiroGroep.map"));

        static string connectionString
            = ConfigurationManager.ConnectionStrings["ChiroGroep"].ToString();

        public Cg2DataContext() : base(connectionString, map) { }
        public Cg2DataContext(string connection) : base(connection, map) { }
    }
}
