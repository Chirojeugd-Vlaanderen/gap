// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	public class AfdelingInfoModel : MasterViewModel
	{
		public AfdelingInfoModel()
		{
			Info = new AfdelingInfo();
		}

		public AfdelingInfo Info { get; set; }
	}
}
