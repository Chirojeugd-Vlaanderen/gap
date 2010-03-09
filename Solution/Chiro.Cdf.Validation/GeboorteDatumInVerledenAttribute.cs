// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
	public class GeboorteDatumInVerledenAttribute : ValidationAttribute
	{
		public GeboorteDatumInVerledenAttribute()
			: base()
		{
			this.ErrorMessageResourceType = typeof(Properties.Resources);
			this.ErrorMessageResourceName = "GeboorteDatumInVerledenAttribute_ValidationError";
		}

		public override bool IsValid(object value)
		{
			// Moeten we nog nagaan of value van het type DateTime is? Of mogen we
			// ervan uitgaan dat deze attribute alleen toegepast wordt op datums?
			if ((DateTime)value > DateTime.Now)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
