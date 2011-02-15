// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

using Chiro.Cdf.Data.Entity;
using Chiro.Gap.Orm;
using Chiro.Gap.Orm.DataInterfaces;

namespace Chiro.Gap.Data.Ef
{
	/// <summary>
	/// Gegevenstoegangsobject voor personen
	/// </summary>
	public class PersonenDao : Dao<Persoon, ChiroGroepEntities>, IPersonenDao
	{

		/// <summary>
		/// Haalt alle Personen op die op een zelfde
		/// adres wonen als de gelieerde persoon met het gegeven ID.
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van gegeven gelieerde
		/// persoon.</param>
		/// <returns>Lijst met Personen (inc. persoonsinfo)</returns>
		/// <remarks>Als de persoon nergens woont, is hij toch zijn eigen
		/// huisgenoot.  Ik geef hier enkel Personen, geen GelieerdePersonen,
		/// omdat ik niet geinteresseerd ben in eventuele dubbels als ik 
		/// GAV ben van verschillende groepen.</remarks>
		public IList<Persoon> HuisGenotenOphalen(int gelieerdePersoonID)
		{
			List<PersoonsAdres> paLijst;
			List<Persoon> resultaat;

			using (var db = new ChiroGroepEntities())
			{
				db.PersoonsAdres.MergeOption = MergeOption.NoTracking;

				// FIXME: enkel persoonsadressen van personen van groepen
				// waarvan je GAV bent

				var persoonsAdressen = (
					from pa in db.PersoonsAdres.Include("Persoon")
					where pa.Adres.PersoonsAdres.Any(
					l => l.Persoon.GelieerdePersoon.Any(
					gp => gp.ID == gelieerdePersoonID))
					select pa);

				// Het zou interessant zijn als ik hierboven al 
				// pa.GelieerdePersoon zou 'selecten'.  Maar gek genoeg wordt
				// dan GelieerdePersoon.Persoon niet meegenomen.
				// Als workaround selecteer ik zodadelijk uit
				// persoonsAdressen de GelieerdePersonen.

				paLijst = persoonsAdressen.ToList();

				// Als de persoon nergens woont, dan is deze lijst leeg.  In dat geval
				// halen we gewoon de gelieerde persoon zelf op.
			}

			resultaat = (from pa in paLijst
						 select pa.Persoon).Distinct().ToList();

			if (resultaat.Count == 0)
			{
				// Als de persoon toevallig geen adressen heeft, is het resultaat
				// leeg.  Dat willen we niet; ieder is zijn eigen huisgenoot,
				// ook al woont hij/zij nergens.  Ipv een leeg resultaat,
				// wordt dan gewoon de gevraagde persoon opgehaald.

				resultaat.Add(CorresponderendePersoonOphalen(gelieerdePersoonID));

				// FIXME: Er wordt veel te veel info opgehaald.
			}

			return resultaat;
		}

		/// <summary>
		/// Haalt de persoon op die correspondeert met een gelieerde persoon.
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van de gelieerde
		/// persoon.</param>
		/// <returns>Persoon inclusief adresinfo</returns>
		public Persoon CorresponderendePersoonOphalen(int gelieerdePersoonID)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.Persoon.MergeOption = MergeOption.NoTracking;

				return
					(from foo in
					 	db.Persoon.Include(p => p.PersoonsAdres.First().Adres.StraatNaam).Include(
					 		p => p.PersoonsAdres.First().Adres.WoonPlaats)
					 where foo.GelieerdePersoon.Any(bar => bar.ID == gelieerdePersoonID)
					 select foo).FirstOrDefault();
			}
		}

		/// <summary>
		///  Haalt een lijst op van personen, op basis van een lijst <paramref name="gelieerdePersoonIDs"/>.
		/// </summary>
		/// <param name="gelieerdePersoonIDs">ID's van *GELIEERDE* personen, waarvan de corresponderende persoonsobjecten
		/// opgehaald moeten worden.</param>
		/// <param name="paths">Bepaalt welke gekoppelde entiteiten mee opgehaald moeten worden.</param>
		/// <returns>De gevraagde personen></returns>
		public IEnumerable<Persoon> OphalenViaGelieerdePersoon(IEnumerable<int> gelieerdePersoonIDs, params Expression<Func<Persoon, object>>[] paths)
		{
			IList<Persoon> result;

			using (var db = new ChiroGroepEntities())
			{
				//// Ik wil dit bereiken:

				// var query = (from p in db.Persoon
				//             where
				//                p.GelieerdePersoon.Any(gp => gelieerdePersoonIDs.Contains(gp.ID))
				//             select p) as ObjectQuery<Persoon>;

				// Maar omdat die 'contains' op die manier niet 'out of the box' werkt, probeer ik
				// het zo:
				
				var query =
					(from gp in
					 	db.GelieerdePersoon.Where(Utility.BuildContainsExpression<GelieerdePersoon, int>(gp => gp.ID, gelieerdePersoonIDs))
					 select gp.Persoon) as ObjectQuery<Persoon>;

				result = IncludesToepassen(query, paths).ToList();
			}

			result = Utility.DetachObjectGraph(result);

			return result;
		}

		/// <summary>
		/// Verlegt alle referenties van <paramref name="dubbel"/> naar <paramref name="origineel"/>, en verwijdert vervolgens
		/// <paramref name="dubbel"/>.
		/// </summary>
		/// <param name="origineel">Te behouden persoon</param>
		/// <param name="dubbel">Te verwijderen persoon, die eigenlijk gewoon dezelfde is als <paramref name="origineel"/></param>
		/// <remarks>Het is niet proper dit soort van logica in de data access te doen.  Anderzijds zou het een 
		/// heel gedoe zijn om dit in de businesslaag te implementeren, omdat er heel wat relaties verlegd moeten worden.
		/// Dat wil zeggen: relaties verwijderen en vervolgens nieuwe maken.  Dit zou een heel aantal 'TeVerwijderens' met zich
		/// meebrengen, wat het allemaal zeer complex zou maken.  Vandaar dat we gewoon via een stored procedure werken.<para />
		/// LET OP: na de aanroep van deze functie zijn 'origineel' en 'dubbel' niet meer bruikbaar.
		/// </remarks>
		public void DubbelVerwijderen(Persoon origineel, Persoon dubbel)
		{
			// Wilde gok op basis van http://ur1.ca/3915c

			using (var db = new ChiroGroepEntities())
			{
				var entityConnection = (EntityConnection) db.Connection;
				var storeConnection = entityConnection.StoreConnection;
				var commando = storeConnection.CreateCommand();

				commando.CommandText = "data.spDubbelePersoonVerwijderen";
				commando.CommandType = CommandType.StoredProcedure;
				commando.Parameters.Add(new SqlParameter("foutPID", dubbel.ID));
				commando.Parameters.Add(new SqlParameter("juistPID", origineel.ID));

				storeConnection.Open();
				commando.ExecuteNonQuery();
				storeConnection.Close();
			}

		}

		/// <summary>
		/// Personen opzoeken op (exacte) naam en voornaam.
		/// Persoon en adressen worden opgehaald.
		/// </summary>
		/// <param name="naam">Exacte naam om op te zoeken</param>
		/// <param name="voornaam">Exacte voornaam om op te zoeken</param>
		/// <returns>Lijst met gevonden gelieerde personen</returns>
		public IEnumerable<Persoon> ZoekenOpNaam(string naam, string voornaam)
		{
			using (var db = new ChiroGroepEntities())
			{
				db.Persoon.MergeOption = MergeOption.NoTracking;
				return (
					from p in db.Persoon
						.Include(prs => prs.PersoonsAdres)
					where String.Compare(p.Naam, naam, true) == 0
					&& String.Compare(p.VoorNaam, voornaam, true) == 0
					select p).ToArray();
			}
		}
	}
}
