using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Algemeen.Data;

namespace KipTest.Domain
{
	public class Deelnemer: IBasisEntiteit
	{
		public int ID { get; set; }
		public int CursusID { get; set; }
		public string Naam { get; set; }
		public virtual Cursus Cursus { get; set; }
	}
}
