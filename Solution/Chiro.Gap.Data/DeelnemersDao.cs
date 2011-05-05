using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
    public class DeelnemersDao: Dao<Deelnemer,ChiroGroepEntities>, IDeelnemersDao
    {
        /// <summary>
        /// Verwijdert de gegeven <paramref name="deelnemer"/> uit de database.
        /// </summary>
        /// <param name="deelnemer">Te verwijderen deelnemer</param>
        public void Verwijderen(Deelnemer deelnemer)
        {
            // Wilde gok op basis van http://ur1.ca/3915c

            using (var db = new ChiroGroepEntities())
            {
                var entityConnection = (EntityConnection)db.Connection;
                var storeConnection = entityConnection.StoreConnection;
                var commando = storeConnection.CreateCommand();

                commando.CommandText = "biv.spDeelnemerVerwijderen";
                commando.CommandType = CommandType.StoredProcedure;
                commando.Parameters.Add(new SqlParameter("deelnemerID", deelnemer.ID));

                storeConnection.Open();
                commando.ExecuteNonQuery();
                storeConnection.Close();
            }
        }
    }
}
