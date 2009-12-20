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
		private readonly LedenManager _lm;

		public LedenService(GelieerdePersonenManager gpm, LedenManager lm)
		{
			this._gpm = gpm;
			this._lm = lm;
		}

		#endregion

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public string LidMakenEnBewaren(int gelieerdePersoonID)
		{
			GelieerdePersoon gp = _gpm.DetailsOphalen(gelieerdePersoonID);

            try
            {
                Lid l = _lm.LidMaken(gp);
                _lm.LidBewaren(l);
                return string.Format("{0} is toegevoegd als lid.", gp.Persoon.VolledigeNaam);
            }
            catch (BestaatAlException e)
            {
                return "De persoon is al lid in dit werkjaar";
            }
		}

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public String LedenMakenEnBewaren(IEnumerable<int> gelieerdePersoonIDs)
        {
            String result = "";
            bool bestonden = false;
            foreach (int gpID in gelieerdePersoonIDs)
            {
                GelieerdePersoon gp = _gpm.DetailsOphalen(gpID);

                try
                {
                    Lid l = _lm.LidMaken(gp);
                    _lm.LidBewaren(l);
                    result = result + gp.Persoon.VolledigeNaam + ", ";
                }
                catch (BestaatAlException e)
                {
                    bestonden = true;
                }
            }
            
            // TODO: feedback aanpassen
            return result.Substring(0, result.Length-2) + " zijn toegevoegd als lid." + (bestonden? " Sommige waren al lid.":"");
        }

		/// <summary>
		/// ook om te maken en te deleten
		/// </summary>
		/// <param name="persoon"></param>
        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void Bewaren(Lid lid)
		{
			_lm.LidBewaren(lid);
		}

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public Boolean Verwijderen(int id)
		{
            throw new NotImplementedException();
			//return _lm.LidVerwijderen(id);
		}

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void BewarenMetAfdelingen(int lidID, IList<int> afdelingsIDs)
		{
            Lid l = _lm.OphalenMetAfdelingen(lidID);
            _lm.UpdatenAfdelingen(l, afdelingsIDs);
            _lm.LidBewaren(l);
		}

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void BewarenMetFuncties(Lid lid)
		{
			//TODO
			throw new NotImplementedException();
		}

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void BewarenMetVrijeVelden(Lid lid)
		{
			//TODO
			throw new NotImplementedException();
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

        /*[PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public IList<LidInfo> PaginaOphalenVolgensCategorie(int categorieID, int groepsWerkJaarID, int pagina, int paginaGrootte, out int aantalTotaal)
        {
            //TODO
            throw new NotImplementedException();
        }*/

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lid"></param>
        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void LidOpNonactiefZetten(Lid lid)
		{
			_lm.LidOpNonactiefZetten(lid);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="lid"></param>
        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
		public void LidActiveren(Lid lid)
		{
			_lm.LidActiveren(lid);
		}

		/// <summary>
		/// Haalt een pagina op met info over alle leden in een
		/// gegeven groepswerkjaar.
		/// </summary>
		/// <param name="groepsWerkJaarID">ID van het groepswerkjaar</param>
		/// <returns>Lijst met LidInfo</returns>
		/*public IList<LidInfo> PaginaOphalen(int groepsWerkJaarID)
		{
			IList<Lid> result = _lm.PaginaOphalen(groepsWerkJaarID);
			return Mapper.Map<IList<Lid>, IList<LidInfo>>(result);
		}*/

        [PrincipalPermission(SecurityAction.Demand, Role = SecurityGroepen.Gebruikers)]
        public LidInfo LidOphalenMetAfdelingen(int lidID)
        {
            return Mapper.Map<Lid, LidInfo>(_lm.OphalenMetAfdelingen(lidID));
        }
	}
}