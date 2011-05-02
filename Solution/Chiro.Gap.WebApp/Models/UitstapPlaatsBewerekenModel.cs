using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model voor bewerken van de plaats van een uitstap.  (duh)
    /// </summary>
    public class UitstapPlaatsBewerekenModel : UitstapModel, IAdresBewerkenModel
    {
        public UitstapPlaatsBewerekenModel()
        {
            Uitstap = new UitstapDetail { Adres = new AdresInfo() };
        }

        #region Implementation of IAdresBewerkenModel

        public IEnumerable<LandInfo> AlleLanden { get; set; }

        public IEnumerable<WoonPlaatsInfo> BeschikbareWoonPlaatsen { get; set; }

        public string Land
        {
            get { return Uitstap.Adres.LandNaam; }
            set { Uitstap.Adres.LandNaam = value; }
        }

        public int PostNr
        {
            get { return Uitstap.Adres.PostNr; }
            set { Uitstap.Adres.PostNr = value; }
        }

        public string PostCode
        {
            get { return Uitstap.Adres.PostCode; }
            set { Uitstap.Adres.PostCode = value; }
        }

        public string Straat
        {
            get { return Uitstap.Adres.StraatNaamNaam; }
            set { Uitstap.Adres.StraatNaamNaam = value; }
        }

        public int? HuisNr
        {
            get { return Uitstap.Adres.HuisNr; }
            set { Uitstap.Adres.HuisNr = value; }
        }

        public string Bus
        {
            get { return Uitstap.Adres.Bus; }
            set { Uitstap.Adres.Bus = value; }
        }

        public string WoonPlaats
        {
            get { return Uitstap.Adres.WoonPlaatsNaam; }
            set { Uitstap.Adres.WoonPlaatsNaam = value; }
        }

        public string WoonPlaatsBuitenLand { get; set; }

        #endregion
    }
}