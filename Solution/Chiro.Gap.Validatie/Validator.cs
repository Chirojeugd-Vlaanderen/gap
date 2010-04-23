// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace Chiro.Gap.Validatie
{
	/// <summary>
	/// Klasse die gebruikt wordt om validatieregels te controleren
	/// </summary>
	/// <typeparam name="T">Object waarop de validator van toepassing is</typeparam>
	public class Validator<T> : IValidator<T>
	{
		/// <summary>
		/// Valideert een object.  De bedoeling is dat er hier een aantal
		/// generieke zaken getest kunnen worden.  (Bijv. 'maxlengths'
		/// die gegeven zijn via attributen.)
		/// <para />
		/// Voorlopig zijn er zo nog geen attributen, dus retourneert
		/// deze functie gewoonweg <c>true</c>.
		/// </summary>
		/// <param name="teValideren">Object dat gevalideerd moet worden</param>
		/// <returns><c>True</c> indien validatie oké</returns>
		public virtual bool Valideer(T teValideren)
		{
			return true;
		}
	}
}
