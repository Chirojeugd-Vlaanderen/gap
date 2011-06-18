using System;
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

        public string Hallo()
        {
            return "Hello world!";
        }
    }
}
