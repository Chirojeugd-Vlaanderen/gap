// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Cdf.Data;

namespace Chiro.Gap.Orm.DataInterfaces
{
	/// <summary>
	/// Interface voor een gegevenstoegangsobject voor CommunicatieVormen
	/// </summary>
	public interface ICommunicatieVormDao : IDao<CommunicatieVorm>
	{
		/// <summary>
		/// Zoekt een lijst van communicatievormen waarbij de waarde (het 'nummer',
		/// maar dat kan ook bv. een mailadres zijn) overeenkomt met de zoekterm
		/// </summary>
		/// <param name="zoekString">De zoekterm</param>
		/// <returns>Een lijst van communicatievormen die de zoekterm als waarde hebben</returns>
		IList<CommunicatieVorm> ZoekenOpNummer(string zoekString);

		/// <summary>
		/// Zoekt alle communicatievormen van een persoon (dus over de verschillende gelieerde
		/// personen heen).  Inclusief communicatietype.
		/// </summary>
		/// <param name="persoonID">ID van de persoon van dewelke we geinteresseerd zijn in de communicatie</param>
		/// <returns>Lijstje met communicatievormen</returns>
		IEnumerable<CommunicatieVorm> ZoekenOpPersoon(int persoonID);
	}
}
