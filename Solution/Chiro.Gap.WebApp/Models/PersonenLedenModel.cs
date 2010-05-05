// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Collections.Generic;

using Chiro.Gap.ServiceContracts;

namespace Chiro.Gap.WebApp.Models
{
	public class PersonenLedenModel : MasterViewModel
	{
		/// <summary>
		/// Informatie over een te tonen of te wijzigen persoon
		/// </summary>
		public PersoonLidInfo PersoonLidInfo
		{
			get;
			set;
		}

		public IEnumerable<AfdelingDetail> AlleAfdelingen
		{
			get;
			set;
		}
	}
}