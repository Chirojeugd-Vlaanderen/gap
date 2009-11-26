using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Diagnostics;
using System.Text;

using AutoMapper;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Orm;
using Chiro.Gap.ServiceContracts;
using Chiro.Gap.Workers;


namespace Chiro.Gap.Services
{
	// NOTE: If you change the class name "GroepenService" here, you must also update the reference to "GroepenService" in Web.config.
	public class GroepenService : IGroepenService
	{
		#region Manager Injection

		private readonly GroepenManager _groepenMgr;
		private readonly WerkJaarManager _werkjaarMgr;
		private readonly IAutorisatieManager _autorisatieMgr;
		private readonly GelieerdePersonenManager _gelieerdePersonenMgr = Factory.Maak<GelieerdePersonenManager>();

		public GroepenService(GroepenManager gm, WerkJaarManager wm, GelieerdePersonenManager gpm, IAutorisatieManager am)
		{
			_groepenMgr = gm;
			_werkjaarMgr = wm;
			_autorisatieMgr = am;
			_gelieerdePersonenMgr = gpm;
		}

		#endregion

		#region IGroepenService Members

		public GroepInfo OphalenInfo(int GroepId)
		{
			var gr = Ophalen(GroepId);
			return Mapper.Map<Groep, GroepInfo>(gr);
		}

		public Groep Bewaren(Groep g)
		{
			try
			{
				return _groepenMgr.Bewaren(g);
			}
			catch (Exception e)
			{
				// TODO: fatsoenlijke exception handling
				throw new FaultException(e.Message, new FaultCode("Optimistic Concurrency Exception"));
			}
		}

		public Groep Ophalen(int groepID)
		{
			var result = _groepenMgr.Ophalen(groepID);
			return result;
		}

		public Groep OphalenMetAdressen(int groepID)
		{
			var result = _groepenMgr.OphalenMetAdressen(groepID);
			return result;
		}

		public Groep OphalenMetFuncties(int groepID)
		{
			var result = _groepenMgr.OphalenMetFuncties(groepID);
			return result;
		}

		public Groep OphalenMetAfdelingen(int groepID)
		{
			var result = _groepenMgr.OphalenMetAfdelingen(groepID);
			return result;
		}

		public Groep OphalenMetVrijeVelden(int groepID)
		{
			var result = _groepenMgr.OphalenMetVrijeVelden(groepID);
			return result;
		}

		public int RecentsteGroepsWerkJaarIDGet(int groepID)
		{
			return _werkjaarMgr.RecentsteGroepsWerkJaarIDGet(groepID);
		}

		/// <summary>
		/// TODO: Documentatie bijwerken, en naam veranderen in HuidgWerkJaarOphalen
		/// (of iets gelijkaardig; zie coding standaard). 
		/// Deze documentatie is alleszins onvolledig, want ze gaat ervan uit dat groepen
		/// nooit ophouden te bestaan.  Wat moet deze functie teruggeven als de groep
		/// geen werking meer heeft?
		/// 
		/// Geeft het huidige werkjaar van de gegeven groep terug. Dit is gegarandeerd het huidige jaartal wanneer de
		/// huidige dag tussen de deadline voor het nieuwe werkjaar en de begindatum van het volgende werkjaar ligt.
		/// In de tussenperiode hangt het ervan af of de groep de overgang al heeft gemaakt, en dit is te zien aan 
		/// het laatst gemaakte groepswerkjaar
		/// </summary>
		/// <param name="groepID"></param>
		/// <returns></returns>
		public int HuidigWerkJaarGet(int groepID)
		{
			return _werkjaarMgr.HuidigWerkJaarGet(groepID);
		}

		/// <summary>
		/// Maakt een nieuwe afdeling voor een gegeven groep
		/// </summary>
		/// <param name="groepID">ID van de groep</param>
		/// <param name="naam">naam van de afdeling</param>
		/// <param name="afkorting">afkorting van de afdeling (voor lijsten, overzichten,...)</param>
		public void AfdelingToevoegen(int groepID, string naam, string afkorting)
		{
			Groep g = _groepenMgr.Ophalen(groepID);
			_groepenMgr.AfdelingToevoegen(g, naam, afkorting);
			_groepenMgr.BewarenMetAfdelingen(g);
		}

		public void AanmakenAfdelingsJaar(Groep g, Afdeling aj, OfficieleAfdeling oa, int geboortejaarbegin, int geboortejaareind)
		{
			_groepenMgr.AfdelingsJaarToevoegen(g, aj, oa, geboortejaarbegin, geboortejaareind);
		}


		public IList<OfficieleAfdeling> OphalenOfficieleAfdelingen()
		{
			return _groepenMgr.OfficieleAfdelingenOphalen();
		}

		public IEnumerable<GroepInfo> OphalenMijnGroepen()
		{
			var result = _autorisatieMgr.GekoppeldeGroepenGet();
			return Mapper.Map<IEnumerable<Groep>, IEnumerable<GroepInfo>>(result);
		}

		#endregion

		#region Categorieen

		//TODO een efficiente manier vinden om een bepaalde eigenschap toe te voegen aan een al geladen element.
		//of anders in de workers methoden aanbieden om lambda expressies mee te geven: dan eerst bepalen wat allemaal nodig is, dan 1 keer laden
		//en dan zijn we terug bij het idee om in het object bij te houden wat hij allemaal heeft geladen
		//
		// Bedenking van Johan: Lambda-expressies lijken me niet wenselijk in de businesslaag, omdat je
		// niet kan controleren of de gebruiker het recht wel heeft de zaken gespecifieerd in de expressie op
		// te vragen.
		public Groep OphalenMetCategorieen(int groepID)
		{
			var result = _groepenMgr.OphalenMetCategorieen(groepID);
			return result;
		}

		/// <summary>
		/// Maakt een nieuwe categorie voor de groep met ID <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">ID van de groep waarvoor nieuwe categorie wordt gemaakt</param>
		/// <param name="naam">naam voor de nieuwe categorie</param>
		/// <param name="code">code voor de nieuwe categorie</param>
		public void CategorieToevoegen(int groepID, string naam, string code)
		{
			Groep g = _groepenMgr.Ophalen(groepID);
			_groepenMgr.CategorieToevoegen(g, naam, code);
			_groepenMgr.BewarenMetCategorieen(g);
		}

		public void CategorieVerwijderen(int categorieID)
		{
			_groepenMgr.CategorieVerwijderen(categorieID);
		}

		public void CategorieAanpassen(Categorie c)
		{
			// Nog niet klaar
			throw new NotImplementedException();
		}

		#endregion categorieen
	}
}
