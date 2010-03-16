// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.ServiceModel;
using System.Text;

using AutoMapper;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Fouten.Exceptions;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;
using Chiro.Gap.ServiceContracts.Mappers;
using Chiro.Gap.Workers;

namespace Chiro.Gap.Services
{
	// OPM: als je de naam van de class "LedenService" hier verandert, moet je ook de sectie "Services" in web.config aanpassen.
	public class LedenService : ILedenService
	{
		#region Manager Injection

		private readonly GelieerdePersonenManager _gpm;
		private readonly GroepenManager _grm;
		private readonly LedenManager _lm;

		public LedenService(GelieerdePersonenManager gpm, LedenManager lm, GroepenManager grm)
		{
			this._gpm = gpm;
			this._lm = lm;
			this._grm = grm;
		}

		#endregion

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IEnumerable<int> LedenMakenEnBewaren(IEnumerable<int> gelieerdePersoonIDs)
		{
			String result = String.Empty;
			IList<Lid> leden = new List<Lid>();
			foreach (int gpID in gelieerdePersoonIDs)
			{
				GelieerdePersoon gp = _gpm.DetailsOphalen(gpID);

				try
				{
					Lid l = _lm.KindMaken(gp);
					leden.Add(l);
				}
				catch (BestaatAlException)
				{
					/*code is reentrant*/
				}
				catch (OngeldigeActieException ex)
				{
					result += ex.Message + "\n";
				}
			}
			if (!result.Equals(String.Empty))
			{
				throw new FaultException<OngeldigeActieException>(new OngeldigeActieException(result));
			}

			foreach (Lid l in leden)
			{
				_lm.LidBewaren(l);
			}
			return (from l in leden
					select l.ID).ToList<int>();
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IEnumerable<int> LeidingMakenEnBewaren(IEnumerable<int> gelieerdePersoonIDs)
		{
			String result = String.Empty;
			IList<Lid> leden = new List<Lid>();
			foreach (int gpID in gelieerdePersoonIDs)
			{
				GelieerdePersoon gp = _gpm.DetailsOphalen(gpID);

				try
				{
					Lid l = _lm.LeidingMaken(gp);
					leden.Add(l);
				}
				catch (BestaatAlException)
				{
					/*code is reentrant*/
				}
				catch (OngeldigeActieException ex)
				{
					result += ex.Message + "\n";
				}
			}
			if (!result.Equals(String.Empty))
			{
				throw new OngeldigeActieException(result);
			}

			foreach (Lid l in leden)
			{
				_lm.LidBewaren(l);
			}
			return (from l in leden
					select l.ID).ToList<int>();
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public LidInfo Bewaren(LidInfo lidinfo)
		{
			Lid lid = _lm.Ophalen(lidinfo.LidID, LidExtras.Geen);

			Debug.Assert(lid is Leiding || lid is Kind);

			if (lid is Kind && lidinfo.Type == LidType.Leiding)
			{
				throw new NotImplementedException();
			}
			else if (lid is Leiding && lidinfo.Type == LidType.Kind)
			{
				throw new NotImplementedException();
			}

			if (lid is Kind)
			{
				Kind kind = (Kind)lid;
				kind.LidgeldBetaald = lidinfo.LidgeldBetaald;
				kind.NonActief = lidinfo.NonActief;
			}
			else
			{
				Leiding leiding = (Leiding)lid;
				leiding.DubbelPuntAbonnement = lidinfo.DubbelPunt;
				leiding.NonActief = lidinfo.NonActief;
			}

			return Mapper.Map<Lid, LidInfo>(_lm.LidBewaren(lid));
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public LidInfo BewarenMetAfdelingen(LidInfo lidinfo)
		{
			Bewaren(lidinfo);

			Lid lid = _lm.Ophalen(lidinfo.LidID, LidExtras.Groep | LidExtras.Afdelingen);

			try
			{
				_lm.AanpassenAfdelingenVanLid(lid, lidinfo.AfdelingIdLijst);
			}
			catch (OngeldigeActieException ex)
			{
				// TODO
				throw ex;
			}

			return Mapper.Map<Lid, LidInfo>(_lm.LidBewaren(lid));
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public Boolean Verwijderen(int id)
		{
			return _lm.LidVerwijderen(id);
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void BewarenMetAfdelingen(int lidID, IList<int> afdelingsIDs)
		{
			Lid l = _lm.Ophalen(lidID, LidExtras.Groep | LidExtras.Afdelingen);
			_lm.AanpassenAfdelingenVanLid(l, afdelingsIDs);
			_lm.LidBewaren(l);
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<LidInfo> PaginaOphalen(int groepsWerkJaarID, out int paginas)
		{
			var result = _lm.PaginaOphalen(groepsWerkJaarID, out paginas);
			return Mapper.Map<IList<Lid>, IList<LidInfo>>(result);
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public IList<LidInfo> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID, out int paginas)
		{
			IList<Lid> result = _lm.PaginaOphalenVolgensAfdeling(groepsWerkJaarID, afdelingsID, out paginas);
			return Mapper.Map<IList<Lid>, IList<LidInfo>>(result);
		}

		/// <summary>
		/// Haalt lid op, inclusief gelieerde persoon en persoon
		/// </summary>
		/// <param name="lidID">ID op te halen lid</param>
		/// <param name="extras">geeft aan welke extra entiteiten mee opgehaald moeten worden</param>
		/// <returns>Lidinfo met gelieerdepersoon en persoon</returns>
		public LidInfo Ophalen(int lidID, LidExtras extras)
		{
			var lid = _lm.Ophalen(lidID, extras);
			return Mapper.Map<Lid, LidInfo>(lid);
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public LidInfo BewarenMetFuncties(LidInfo lid)
		{
			// TODO
			throw new NotImplementedException();
		}

		/* zie #273 */
		// [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public LidInfo BewarenMetVrijeVelden(LidInfo lid)
		{
			// TODO
			throw new NotImplementedException();
		}
	}
}