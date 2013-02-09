using System;
using System.Collections.Generic;
using System.Linq;

using Chiro.Cdf.Poco;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.WorkerInterfaces;

using GebruikersRecht = Chiro.Gap.ServiceContracts.DataContracts.GebruikersRecht;

namespace Chiro.Gap.Services
{
    /// <summary>
    /// Interface voor de service voor gebruikersrechtenbeheer.
    /// </summary>
    public class GebruikersService : IGebruikersService
    {
        /// <summary>
        /// _context is verantwoordelijk voor het tracken van de wijzigingen aan de
        /// entiteiten. Via _context.SaveChanges() kunnen wijzigingen gepersisteerd
        /// worden.
        /// 
        /// Context is IDisposable. De context wordt aangemaakt door de IOC-container,
        /// en gedisposed op het moment dat de service gedisposed wordt. Dit gebeurt
        /// na iedere call.
        /// </summary>
        private readonly IContext _context;

        // Repositories, verantwoordelijk voor data access.
        private readonly IRepository<Gav> _rechtenRepo;
        private readonly IRepository<Groep> _groepenRepo;
        private readonly IRepository<GelieerdePersoon> _gelieerdePersonenRepo;

        // Managers voor niet-triviale businesslogica

        private readonly IGebruikersRechtenManager _gebruikersRechtenMgr;
        private readonly IAutorisatieManager _autorisatieMgr;
        private readonly GavChecker _gav;

        /// <summary>
        /// Nieuwe groepenservice
        /// </summary>
        /// <param name="autorisatieMgr">Verantwoordelijke voor autorisatie</param>
        /// <param name="gebruikersRechtenMgr">Businesslogica aangaande gebruikersrechten</param>
        /// <param name="repositoryProvider">De repository provider levert alle nodige repository's op.</param>
        public GebruikersService(IAutorisatieManager autorisatieMgr,
                              IGebruikersRechtenManager gebruikersRechtenMgr,
                              IRepositoryProvider repositoryProvider)
        {
            _context = repositoryProvider.ContextGet();
            _rechtenRepo = repositoryProvider.RepositoryGet<Gav>();

            _groepenRepo = repositoryProvider.RepositoryGet<Groep>();
            _gelieerdePersonenRepo = repositoryProvider.RepositoryGet<GelieerdePersoon>();

            _gebruikersRechtenMgr = gebruikersRechtenMgr;
            _autorisatieMgr = autorisatieMgr;
            _gav = new GavChecker(_autorisatieMgr);
        }

        public GavChecker Gav
        {
            get { return _gav; }
        }

        /// <summary>
        /// Als de persoon met gegeven <paramref name="gelieerdePersoonId"/> nog geen account heeft, wordt er een
        /// account voor gemaakt. Aan die account worden dan de meegegeven <paramref name="gebruikersRechten"/>
        /// gekoppeld.  Gebruikersrechten zijn standaard 14 maanden geldig.
        /// </summary>
        /// <param name="gelieerdePersoonId">Id van gelieerde persoon die rechten moet krijgen</param>
        /// <param name="gebruikersRechten">Rechten die de account moet krijgen. Mag leeg zijn. Bestaande 
        /// gebruikersrechten worden zo mogelijk verlengd als ze in <paramref name="gebruikersRechten"/> 
        /// voorkomen, eventuele bestaande rechten niet in <paramref name="gebruikersRechten"/> blijven
        /// onaangeroerd.
        /// </param>
        public void RechtenToekennen(int gelieerdePersoonId, GebruikersRecht[] gebruikersRechten)
        {
            var gelieerdePersoon = _gelieerdePersonenRepo.ByID(gelieerdePersoonId);
            Gav.Check(gelieerdePersoon);
            
            var account = _gebruikersRechtenMgr.AccountZoekenOfMaken(gelieerdePersoon);

            RechtenToekennen(account, gebruikersRechten);
       }

        /// <summary>
        /// Geeft de account met gegeven <paramref name="gebruikersNaam"/> de gegeven
        /// <paramref name="gebruikersRechten"/>.  Gebruikersrechten zijn standaard 14 maanden geldig.
        /// De gegeven accout moet bestaan; we moeten vermijden dat eender welke user zomaar accounts
        /// kan maken voor chiro.wereld.
        /// </summary>
        /// <param name="gebruikersNaam">gebruikersnaam van de account die rechten moet krijgen</param>
        /// <param name="gebruikersRechten">Rechten die de account moet krijgen.
        /// Bestaande gebruikersrechten worden zo mogelijk verlengd als ze in 
        /// <paramref name="gebruikersRechten"/> voorkomen, eventuele bestaande rechten niet in 
        /// <paramref name="gebruikersRechten"/> blijven onaangeroerd.
        /// </param>
        public void RechtenToekennenGebruiker(string gebruikersNaam, GebruikersRecht[] gebruikersRechten)
        {
            var account = (from g in _rechtenRepo.Select() where Equals(g.Login, gebruikersNaam) select g).FirstOrDefault();

            RechtenToekennen(account, gebruikersRechten);
        }

        private void RechtenToekennen(Gav account, GebruikersRecht[] gebruikersRechten)
        {
            // Momenteel ondersteunen we enkel GAV-rollen
            var nietOndersteund = (from gr in gebruikersRechten
                                   where gr.Rol != Rol.Gav
                                   select gr).FirstOrDefault();
            if (nietOndersteund != null)
            {
                throw new NotSupportedException(String.Format(Properties.Resources.RolNietOndersteund, nietOndersteund.Rol));
            }

            foreach (var groep in gebruikersRechten.Select(recht => _groepenRepo.ByID(recht.GroepID)))
            {
                Gav.Check(groep);
                _gebruikersRechtenMgr.ToekennenOfVerlengen(account, groep);
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// Neemt de alle gebruikersrechten van de gelieerde persoon met gegeven
        /// <paramref name="gelieerdePersoonId"/> af voor de groepen met gegeven <paramref name="groepIds"/>
        /// </summary>
        /// <param name="gelieerdePersoonId">Id van gelieerde persoon met af te nemen gebruikersrechten</param>
        /// <param name="groepIds">Id's van groepen waarvoor gebruikersrecht afgenomen moet worden.</param>
        /// <remarks>In praktijk gebeurt dit door de vervaldatum in het verleden te leggen.</remarks>
        public void RechtenAfnemen(int gelieerdePersoonId, int[] groepIds)
        {
            var persoon = _gelieerdePersonenRepo.ByID(gelieerdePersoonId);
            Gav.Check(persoon);
            var account = (from g in _rechtenRepo.Select() where g.Persoon.Any(e => e.GelieerdePersoon.Contains(persoon)) select g).FirstOrDefault();
            RechtenAfnemen(account, groepIds);
        }

        /// <summary>
        /// Neemt de alle gebruikersrechten van de gelieerde persoon met gegeven
        /// <paramref name="gebruikersNaam"/> af voor de groepen met gegeven <paramref name="groepIds"/>
        /// </summary>
        /// <param name="gebruikersNaam">gebruikersnaam van gelieerde persoon met af te nemen gebruikersrechten</param>
        /// <param name="groepIds">Id's van groepen waarvoor gebruikersrecht afgenomen moet worden.</param>
        /// <remarks>In praktijk gebeurt dit door de vervaldatum in het verleden te leggen.</remarks>
        public void RechtenAfnemenGebruiker(string gebruikersNaam, int[] groepIds)
        {
            var account = (from g in _rechtenRepo.Select() where Equals(g.Login, gebruikersNaam) select g).FirstOrDefault();
            RechtenAfnemen(account, groepIds);
        }

        private void RechtenAfnemen(Gav account, IEnumerable<int> groepIds)
        {
            Gav.Check(account);
            if (account == null)
            {
                throw FaultExceptionHelper.GeenGav();
            }

            var vervallenrechten = (from g in account.GebruikersRecht where groepIds.Contains(g.Groep.ID) select g).ToList();

            foreach (var vervallenrecht in vervallenrechten)
            {
                vervallenrecht.VervalDatum = DateTime.Today.AddDays(-1);
            }

            _context.SaveChanges();
        }
    }
}