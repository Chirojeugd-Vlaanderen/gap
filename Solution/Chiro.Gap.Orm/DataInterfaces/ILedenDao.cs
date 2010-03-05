using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	public interface ILedenDao : IDao<Lid>
	{
		IList<Lid> AllesOphalen(int groepsWerkJaarID);

		IList<Lid> PaginaOphalenVolgensAfdeling(int groepsWerkJaarID, int afdelingsID);

		/// <summary>
		/// Zoekt lid op op basis van GelieerdePersoonID en GroepsWerkJaarID
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van gelieerde persoon</param>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar</param>
		/// <returns>Lidobject indien gevonden, anders null</returns>
		Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID);

		/// <summary>
		/// Haalt de leden op die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de functie bepaald door <paramref name="functieID"/> hebben
		/// </summary>
		/// <param name="functieID">ID van een functie</param>
		/// <param name="groepsWerkJaarID">ID van een groepswerkjaar</param>
		/// <param name="paths">bepaalt de mee op te halen gekoppelde entiteiten</param>
		/// <returns>Lijst leden die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de functie bepaald door <paramref name="functieID"/> hebben.</returns>
		IList<Lid> OphalenUitFunctie(
			int functieID, 
			int groepsWerkJaarID, 
			params Expression<Func<Lid, object>>[] paths);

		/// <summary>
		/// Haalt de leden op die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de gepredefinieerde functie met type <paramref name="f"/> hebben.
		/// </summary>
		/// <param name="f">type gepredefinieerde functie</param>
		/// <param name="groepsWerkJaarID">ID van een groepswerkjaar</param>
		/// <param name="paths">bepaalt de mee op te halen gekoppelde entiteiten</param>
		/// <returns>Lijst leden die in het groepswerkjaar bepaald door <paramref name="groepsWerkJaarID"/>
		/// de gepredefinieerde functie met type <paramref name="f"/> hebben.</returns>
		IList<Lid> OphalenUitFunctie(
			GepredefinieerdeFunctieType f,
			int groepsWerkJaarID,
			params Expression<Func<Lid, object>>[] paths);


		/// <summary>
		/// Haalt lid met gerelateerde entity's op, op basis van 
		/// GelieerdePersoonID en GroepsWerkJaarID
		/// </summary>
		/// <param name="gelieerdePersoonID">ID van gelieerde persoon</param>
		/// <param name="groepsWerkJaarID">ID van groepswerkjaar</param>
		/// <param name="paths">lambda-expressies die de extra op te halen
		/// informatie definieren</param>
		/// <returns>Lidobject indien gevonden, anders null</returns>
		Lid Ophalen(int gelieerdePersoonID, int groepsWerkJaarID, params Expression<Func<Lid, object>>[] paths);

		Lid OphalenMetDetails(int lidID);

		//void LidMaken(int gelieerdeID);
	}
}
