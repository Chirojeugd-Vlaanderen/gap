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
	public class StringLengteAttribute : StringLengthAttribute
	{
		public StringLengteAttribute(int maximumLength)
			: base(maximumLength)
		{
			this.ErrorMessageResourceType = typeof(Properties.Resources);
			this.ErrorMessageResourceName = "StringLengthAttribute_ValidationError";
		}
	}
}
