// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace Chiro.Gap.Orm.SyncInterfaces
{
	/// <summary>
	/// Interface voor het overzetten van verzekeringsgegevens naar Kipadmin.
	/// </summary>
	public interface IVerzekeringenSync
	{
		/// <summary>
		/// Zet de gegeven <paramref name="persoonsVerzekering"/> over naar Kipadmin.
		/// </summary>
		/// <param name="persoonsVerzekering">Over te zetten persoonsverzekering</param>
		/// <param name="gwj">Bepaalt werkjaar en groep die factuur zal krijgen</param>
		void Bewaren(PersoonsVerzekering persoonsVerzekering, GroepsWerkJaar gwj);
	}
}