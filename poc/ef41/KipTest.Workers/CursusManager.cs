﻿using System;
using System.Collections.Generic;
using System.Linq;
using Algemeen.Data;
using KipTest.Domain;

namespace KipTest.Workers
{
	public class CursusManager
	{
		private readonly IRepository _repository;

		public CursusManager(IRepository entitities)
		{
		    _repository = entitities;
		}

		public IEnumerable<Cursus> ToekomstigeOphalen()
		{
		    return (from c in _repository.Alles<Cursus>()
		            where c.StartDatum > DateTime.Now
		            select c).ToArray();
		}

		public Cursus Ophalen(int cursusID)
		{
		    return Ophalen(cursusID, false);
		}

        public Cursus Ophalen(int cursusID, bool metDeelnemers)
        {
            return metDeelnemers ? _repository.Ophalen<Cursus>(cursusID, cs=>cs.Deelnemers) : _repository.Ophalen<Cursus>(cursusID);
        }

		public Cursus Maken(string naam, DateTime startDatum, DateTime stopDatum)
		{
		    var c = new Cursus {Naam = naam, StartDatum = startDatum, StopDatum = stopDatum};
		    _repository.Toevoegen(c);
		    return c;
		}

        public void WijzigingenBewaren()
        {
            _repository.WijzigingenBewaren();
        }

	    public Deelnemer Inschrijven(Cursus cursus, string deelnemerNaam)
	    {
	        var d = new Deelnemer {Naam = deelnemerNaam, Cursus = cursus};
	        return _repository.Toevoegen(d);
	    }

	    public string[] DeelnemersOphalen(int cursusId)
	    {
	        return (from dln in _repository.Alles<Deelnemer>()
	                where dln.CursusID == cursusId
	                select dln.Naam).ToArray();
	    }

	    public void DeelnemersVerwijderen(Deelnemer[] pineuten)
	    {
            foreach (var p in pineuten)
            {
                _repository.Verwijderen(p);
            }
	    }

	    public void DeelnemersVerhuizen(Deelnemer[] pineuten, Cursus nieuweCursus)
	    {
            foreach (var p in pineuten)
            {
                p.Cursus = nieuweCursus;
            }
        }
	}
}