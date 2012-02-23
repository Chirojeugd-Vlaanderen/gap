// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Data;
using System.Data.EntityClient;
using System.Data.SqlClient;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
    /// <summary>
    /// Gegevenstoegangsobject voor deelnemers aan een uitstap
    /// </summary>
    public class DeelnemersDao : Dao<Deelnemer, ChiroGroepEntities>, IDeelnemersDao
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
                try
                {
                    commando.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    // Blijkbaar wordt er hier een exception gethrowd, die zegt dat
                    // biv.spDeelnemerVerwijderen niet wordt gevonden.  Desondanks wordt
                    // ze wel uitgevoerd.  Hoe dat komt, begrijp ik langs geen kanten.

                    // Hoe dan ook, als ik hier dus een SqlException met nummer 15151 tegenkom,
                    // negeer ik ze gewoon

                    if (ex.Number != 15151)
                    {
                        // Andere exceptions zijn onverwacht, en throw ik dus maar.
                        throw;
                    }
                }

                storeConnection.Close();
            }
        }
    }
}
