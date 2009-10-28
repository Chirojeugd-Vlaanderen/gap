using System;
namespace Chiro.Gap.Workers
{
    public interface IAutorisatieManager
    {
        System.Collections.Generic.IList<int> EnkelMijnGelieerdePersonen(System.Collections.Generic.IEnumerable<int> gelieerdePersonenIDs);
        System.Collections.Generic.IList<int> EnkelMijnPersonen(System.Collections.Generic.IList<int> personenIDs);
        System.Collections.Generic.IEnumerable<Chiro.Gap.Orm.Groep> GekoppeldeGroepenGet();
        bool IsGavGelieerdePersoon(int gelieerdePersoonID);
        bool IsGavGroep(int groepID);
        bool IsGavGroepsWerkJaar(int groepsWerkJaarID);
        bool IsGavPersoon(int persoonID);
        bool IsGavAfdeling(int afdelingsID);
        bool IsGavLid(int lidID);
        bool IsGavCategorie(int categorieID);
        bool IsGavCommVorm(int commvormID);

        string GebruikersNaamGet();
    }
}
