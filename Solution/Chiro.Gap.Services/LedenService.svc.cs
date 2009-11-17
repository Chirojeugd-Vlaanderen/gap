using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.Workers;
using Chiro.Gap.Orm;
using Chiro.Cdf.Ioc;
using Chiro.Gap.ServiceContracts.Mappers;
using AutoMapper;

namespace Chiro.Gap.Services
{
	// NOTE: If you change the class name "LedenService" here, you must also update the reference to "LedenService" in Web.config.
	public class LedenService : ILedenService
	{

		#region Manager Injection

		private readonly GelieerdePersonenManager gpm;
		private readonly LedenManager lm;

		public LedenService(GelieerdePersonenManager gpm, LedenManager lm)
		{
			this.gpm = gpm;
			this.lm = lm;
		}

		#endregion

		public string LidMakenEnBewaren(int gelieerdePersoonID)
		{
			GelieerdePersoon gp = gpm.DetailsOphalen(gelieerdePersoonID);

			Lid l = lm.LidMaken(gp);
			lm.LidBewaren(l);
			// TODO: feedback aanpassen
			return string.Format("{0} is toegevoegd als lid.", gp.Persoon.VolledigeNaam);
		}

		/// <summary>
		/// ook om te maken en te deleten
		/// </summary>
		/// <param name="persoon"></param>
		public void Bewaren(Lid lid)
		{
			lm.LidBewaren(lid);
		}

		public Boolean Verwijderen(int id)
		{
			return lm.LidVerwijderen(id);
		}

		public void BewarenMetAfdelingen(Lid lid)
		{
			//TODO
			throw new NotImplementedException();
		}

		public void BewarenMetFuncties(Lid lid)
		{
			//TODO
			throw new NotImplementedException();
		}

		public void BewarenMetVrijeVelden(Lid lid)
		{
			//TODO
			throw new NotImplementedException();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lid"></param>
		public void LidOpNonactiefZetten(Lid lid)
		{
			lm.LidOpNonactiefZetten(lid);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lid"></param>
		public void LidActiveren(Lid lid)
		{
			lm.LidActiveren(lid);
		}

		/// <summary>
		/// Haalt een pagina op met info over alle leden in een
		/// gegeven groepswerkjaar.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <returns>Lijst met LidInfo</returns>
		public IList<LidInfo> PaginaOphalen(int groepsWerkJaarID)
		{
			IList<Lid> result = lm.PaginaOphalen(groepsWerkJaarID);
			return Mapper.Map<IList<Lid>, IList<LidInfo>>(result);
		}

		public IList<LidInfo> PaginaOphalenVoorAfdeling(int groepsWerkJaarID, int afdelingsID)
		{
			IList<Lid> result = lm.PaginaOphalen(groepsWerkJaarID, afdelingsID);
			return Mapper.Map<IList<Lid>, IList<LidInfo>>(result);
		}
	}
}