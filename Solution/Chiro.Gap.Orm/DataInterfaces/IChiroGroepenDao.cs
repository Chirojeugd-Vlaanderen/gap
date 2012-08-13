using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
    public interface IChiroGroepenDao: IDao<ChiroGroep>
    {
        /// <summary>
        /// Bewaart de gegeven <paramref name="chiroGroep"/>, samen met de koppelingen bepaald in
        /// <paramref name="extras"/>
        /// </summary>
        /// <param name="chiroGroep">Te bewaren Chirogroep</param>
        /// <param name="extras">Te bewaren koppelingen</param>
        /// <returns>Bewaarde Chirogroep met koppelingen uit <paramref name="extras"/></returns>
        ChiroGroep Bewaren(ChiroGroep chiroGroep, ChiroGroepsExtras extras);
        
        /// <summary>
        /// Haalt de Chirogroep met ID <paramref name="groepID"/> op, samen met de gekoppelde
        /// entiteiten bepaald door <paramref name="extras"/>
        /// </summary>
        /// <param name="groepID">ID op te halen Chirogroep</param>
        /// <param name="extras">Bepaalt mee op te halen entiteiten</param>
        /// <returns>De gevraagde Chirogroep met de gevraagde gekoppelde entiteiten</returns>
        ChiroGroep Ophalen(int groepID, ChiroGroepsExtras extras);
    }
}