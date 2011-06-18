using System;
using System.Collections.Generic;
using Algemeen.Data;

namespace KipTest.Domain
{
	public class Cursus: IBasisEntiteit
	{
		public int ID { get; set; }
		public string Naam { get; set; }
		public DateTime StartDatum { get; set; }
		public DateTime StopDatum { get; set; }

		public virtual ICollection<Deelnemer> Deelnemers { get; set; }
	}
}
