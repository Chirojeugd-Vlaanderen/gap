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
using System.Security.Permissions;
using Chiro.Gap.Fouten.Exceptions;

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

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
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
				catch (BestaatAlException){ /*code is reentrant*/ }
				catch (OngeldigeActieException ex)
				{
					result += ex.Message + "\n";
				}
			}
			if (!result.Equals(String.Empty))
			{
				throw new OngeldigeActieException(result);
			}

			foreach(Lid l in leden)
			{
				_lm.LidBewaren(l);
			}
			return (from l in leden
					select l.ID).ToList<int>();
        }

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
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
				catch (BestaatAlException) { /*code is reentrant*/ }
				catch (OngeldigeActieException ex)
				{
					result += ex.Message + "\n";
				}
			}
			if (!result.Equals(String.Empty))
			{
				throw new OngeldigeActieException(result);
			}

			foreach(Lid l in leden)
			{
				_lm.LidBewaren(l);
			}
			return (from l in leden
					select l.ID).ToList<int>();
		}

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void Bewaren(LidInfo lid)
		{
			throw new NotImplementedException();

			/*Lid l = _lm.Ophalen(lid.LidID);
			if (lid.Type == LidType.Kind)
			{
				Kind kind = new Kind();

				//3 weken bedenktijd
				kind.EindeInstapPeriode = DateTime.Now.AddDays(21); //TODO IN MOOIE CONFIGFILE STEKEN OFZO

				l = (Lid)kind;
			}
			else
			{
				Leiding leiding = new Leiding();

				leiding.

				//TODO afdelingjaren en dubbelpunt

				l = (Lid)leiding;
			}

			GelieerdePersoon gp = _gpm.Ophalen(lid.PersoonInfo.GelieerdePersoonID);
			l.GelieerdePersoon = gp;
			l.LidgeldBetaald = false;
			l.NonActief = false;

			l.GroepsWerkJaar = _grm.RecentsteGroepsWerkJaarGet(gp.Groep.ID);
			gp.Lid.Add(l);
			_lm.LidBewaren(l);*/
		}

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public Boolean Verwijderen(int id)
		{
            //wat moet het gedrag hiervan juist zijn (inactief of niet ...)
            throw new NotImplementedException();
			//return _lm.LidVerwijderen(id);
		}

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void BewarenMetAfdelingen(int lidID, IList<int> afdelingsIDs)
		{
            Lid l = _lm.OphalenMetAfdelingen(lidID);
            _lm.AanpassenAfdelingenVanLid(l, afdelingsIDs);
            _lm.LidBewaren(l);
		}

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<LidInfo> PaginaOphalen(int groepsWerkJaarID, out int paginas)
        {
            var result = _lm.PaginaOphalen(groepsWerkJaarID, out paginas);
            return Mapper.Map<IList<Lid>, IList<LidInfo>>(result);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<LidInfo> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID, out int paginas)
        {
            IList<Lid> result = _lm.PaginaOphalenVolgensAfdeling(groepsWerkJaarID, afdelingsID, out paginas);
            return Mapper.Map<IList<Lid>, IList<LidInfo>>(result);
        }

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public LidInfo LidOphalenMetAfdelingen(int lidID)
        {
            return Mapper.Map<Lid, LidInfo>(_lm.OphalenMetAfdelingen(lidID));
        }

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void BewarenMetFuncties(LidInfo lid)
		{
			//TODO
			throw new NotImplementedException();
		}

		[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void BewarenMetVrijeVelden(LidInfo lid)
		{
			//TODO
			throw new NotImplementedException();
		}
	}
}