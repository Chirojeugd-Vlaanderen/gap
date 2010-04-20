// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Chiro.Gap.Domain;


namespace Chiro.Gap.ServiceContracts.FaultContracts
{
	/// <summary>
	/// Faultcontract voor als er iets mis is met een object.  Dit contract bevat verschillende foutberichten, die
	/// betrekking kunnen hebben op property's van het object.
	/// </summary>
	/// <typeparam name="TFoutCode">Type van de foutcodes gebruikt in de foutboodschappen</typeparam>
	/// <remarks>TODO: Dit wordt blijkbaar enkel gebruikt voor adressen.  Is heel die constructie dan wel
	/// nodig? Misschien is een AdresFault wel even goed.</remarks>
	[DataContract]
	public class OngeldigObjectFault : GapFault 
	{
		/// <summary>
		/// Dictionary die voor elke fout een dictionary-entry (component, foutbericht) bevat.
		/// </summary>
		[DataMember]
		public Dictionary<string, FoutBericht> Berichten { get; set; }
	}
}
