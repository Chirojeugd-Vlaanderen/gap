using System;
using System.Linq;
using KipTest.ServiceContracts;
using KipTest.Workers;

namespace KipTest.Service
{
    public class CursusService : ICursusService
    {
        private readonly CursusManager _cursusManager;

        public CursusService(CursusManager cmgr)
        {
            // dependency injection

            _cursusManager = cmgr;
        }

        public int CursusMaken(string naam, DateTime startDatum, DateTime stopDatum)
        {
            var c = _cursusManager.Maken(naam, startDatum, stopDatum);
            _cursusManager.WijzigingenBewaren();
            return c.ID;
        }

        public int DeelnemerInschrijven(int cursusID, string deelnemerNaam)
        {
            var cursus = _cursusManager.Ophalen(cursusID);
            var deelnemer = _cursusManager.Inschrijven(cursus, deelnemerNaam);
            _cursusManager.WijzigingenBewaren();
            return deelnemer.ID;
        }

        public string[] DeelnemersOphalen(int cursusID)
        {
            return _cursusManager.DeelnemersOphalen(cursusID);
        }

        // Verwijdert alle deelnemers met de gegeven naam.
        // domme functie, maar POC
        public void DeelnemerVerwijderen(int cursusID, string naam)
        {
            // In praktijk zul je dit niet op deze manier implementeren.  Maar dit is
            // om aan te tonen dat je bij het bewaren van je aangepaste situatie geen
            // 'TeVerwijderen' en geen lambda-expressies meer nodig hebt.

            // Haal cursus op met deelnemers
            var cursus = _cursusManager.Ophalen(cursusID, true);

            var pineuten = (from d in cursus.Deelnemers
                            where String.Compare(d.Naam, naam, true) == 0
                            select d).ToArray();

            _cursusManager.DeelnemersVerwijderen(pineuten);
            _cursusManager.WijzigingenBewaren();
        }

        // Verhuist deelnemers met gegeven naam naar een andere cursus
        public void DeelnemerVerhuizen(int cursusVanID, int cursusTotID, string naam)
        {
            // In praktijk doe je dit natuurlijk efficienter.  Maar het is een voorbeeld.

            // Haal cursus op met deelnemers
            var cursus1 = _cursusManager.Ophalen(cursusVanID, true);
            var cursus2 = _cursusManager.Ophalen(cursusTotID, true);

            var pineuten = (from d in cursus1.Deelnemers
                            where String.Compare(d.Naam, naam, true) == 0
                            select d).ToArray();

            _cursusManager.DeelnemersVerhuizen(pineuten, cursus2);
            _cursusManager.WijzigingenBewaren();
        }

        public string Hallo()
        {
            return "Hello world!";
        }
    }
}
