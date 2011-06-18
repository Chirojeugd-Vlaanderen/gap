using System;
using System.Collections.Generic;
using System.Linq;
using Algemeen.Data;
using KipTest.Domain;

namespace KipTest.Workers
{
	public class CursusManager
	{
		private readonly IEntities _entities;

		public CursusManager(IEntities entitities)
		{
		    _entities = entitities;
		}

		public IEnumerable<Cursus> ToekomstigeOphalen()
		{
		    return (from c in _entities.Alles<Cursus>()
		            where c.StartDatum > DateTime.Now
		            select c).ToArray();
		}

		public Cursus Ophalen(int cursusID)
		{
		    return _entities.Ophalen<Cursus>(cursusID);
		}

		public Cursus Maken(string naam, DateTime startDatum, DateTime stopDatum)
		{
		    var c = new Cursus {Naam = naam, StartDatum = startDatum, StopDatum = stopDatum};
		    _entities.Toevoegen(c);
		    return c;
		}

        public void WijzigingenBewaren()
        {
            _entities.WijzigingenBewaren();
        }

	    public Deelnemer Inschrijven(Cursus cursus, string deelnemerNaam)
	    {
	        var d = new Deelnemer {Naam = deelnemerNaam, Cursus = cursus};
	        return _entities.Toevoegen(d);
	    }

	    public string[] DeelnemersOphalen(int cursusId)
	    {
	        return (from dln in _entities.Alles<Deelnemer>()
	                where dln.CursusID == cursusId
	                select dln.Naam).ToArray();
	    }
	}
}
