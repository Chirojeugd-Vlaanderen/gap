using System.Data.Entity;
using Algemeen.Data.Entity;
using KipTest.Domain;

namespace KipTest.Data
{
	public class TestEntities: Entities
	{
        // Deze zijn nodig voor rechtstreekse toegang
        // Entity-framework-4.1-magie mapt automatisch

        private DbSet<Cursus> Cursussen { get; set; }
        private DbSet<Deelnemer> Deelnemers { get; set; }
	}
}
