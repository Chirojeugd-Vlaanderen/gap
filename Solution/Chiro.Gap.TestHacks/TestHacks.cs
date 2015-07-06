using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Chiro.Gap.TestHacks.Properties;

namespace Chiro.Gap.TestHacks
{
    public class TestHacks
    {
        /// <summary>
        /// Erg lelijke hack die direct in de database schrijft om een gebruiker met naam 
        /// <paramref name="userName"/> toegang te geven tot een testgroep.
        /// </summary>
        /// <param name="userName">naam van de gebruiker die toegang moet krijgen</param>
        public static void TestGroepToevoegen(string userName)
        {
            using (var connection = new SqlConnection(Settings.Default.ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand("auth.spWillekeurigeGroepToekennen", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };
                command.Parameters.Add(new SqlParameter("@login", userName));
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
