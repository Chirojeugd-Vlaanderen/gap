using System;
using Chiro.Kip.Log;
using Chiro.Kip.ServiceContracts;
using Adres = Chiro.Kip.ServiceContracts.DataContracts.Adres;

namespace Chiro.Kip.Services
{
	/// <summary>
	/// Klasse die persoons- en lidgegevens overzet van GAP naar Kipadmin.
	/// 
	/// BELANGRIJK: Oorspronkelijk werden voor de meeste methods geen personen over de lijn gestuurd, maar enkel
	/// AD-nummers.  Het idee daarachter was dat toch enkel gegevens van personen met AD-nummer naar kipadmin
	/// gesynct moeten worden.
	/// 
	/// Maar met het AD-nummer alleen kom je er niet.  Het kan namelijk goed zijn dat een persoon gewijzigd wordt
	/// tussen het moment dat hij voor het eerst lid wordt, en het moment dat hij zijn AD-nummer krijgt.  Deze
	/// wijzigingen willen we niet verliezen.
	/// 
	/// Het PersoonID van GAP meesturen helpt in de meeste gevallen.  Maar dat kan mis gaan op het moment dat een persoon
	/// uit kipadmin nog dubbel in GAP zit.  Vooraleer deze persoon zijn AD-nummer krijgt, weten we dat immers niet.
	/// 
	/// Vandaar dat nu alle methods volledige persoonsobjecten gebruiken, zodat het opzoeken van een persoon zo optimaal
	/// mogelijk kan gebeuren.  Het persoonsobject een AD-nummer heeft, wordt er niet naar de rest gekeken.
	/// 
	/// TODO: De mapping van Persoon naar PersoonZoekInfo zou beter ergens op 1 plaats gedefinieerd worden, ipv in 
	/// elke method apart.
	/// </summary>
	public partial class SyncPersoonService : ISyncPersoonService
	{
		private readonly IMiniLog _log;

		/// <summary>
		/// Kipadmin kent het onderscheid postnummer/postcode niet.  Deze
		/// domme functie plakt de twee aan elkaar.
		/// </summary>
		/// <param name="adres">Adres</param>
		/// <returns>combinatie postnummer/postcode van adres</returns>
		private static string KipPostNr(Adres adres)
        {
            // Sommige groepen die het verschil postcode/postnr niet snappen, typen in beide vakjes hetzelfde.
            // Dat filteren we er hier ook maar uit.

            // TODO: test of het resultaat van KipPostNr niet langer wordt dan 10 tekens.

		    string resultaat = (String.IsNullOrEmpty(adres.PostCode) ||
		                        String.Compare(adres.PostNr.ToString(), adres.PostCode) == 0)
		                           ? adres.PostNr.ToString()
		                           : String.Format(
		                               "{0} {1}",
		                               adres.PostNr,
		                               adres.PostCode);
            
            // Voor de zekerheid beperken tot 10 karakters.

            if (resultaat.Length > 10)
            {
                resultaat = resultaat.Substring(0, 10);
            }

		    return resultaat;
        }

		/// <summary>
		/// Standaardconstructor
		/// </summary>
		public SyncPersoonService()
		{
			_log = new MiniLog();
		}
	}
}
