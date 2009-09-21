using System;
namespace Cg2.Workers
{
    public interface IAutorisatieManager
    {
        System.Collections.Generic.IList<int> EnkelMijnGelieerdePersonen(System.Collections.Generic.IList<int> gelieerdePersonenIDs);
        System.Collections.Generic.IList<int> EnkelMijnPersonen(System.Collections.Generic.IList<int> personenIDs);
        System.Collections.Generic.IEnumerable<Cg2.Orm.Groep> GekoppeldeGroepenGet();
        bool IsGavGelieerdePersoon(int gelieerdePersoonID);
        bool IsGavGroep(int groepID);
        bool IsGavGroepsWerkJaar(int groepsWerkJaarID);
        bool IsGavPersoon(int persoonID);
    }
}
