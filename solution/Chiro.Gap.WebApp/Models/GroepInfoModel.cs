﻿// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	public class GroepInfoModel : MasterViewModel
	{
        public GroepInfoModel()
		{
            Info = new GroepInfo();
		}

        public GroepInfo Info { get; set; }
	}
}