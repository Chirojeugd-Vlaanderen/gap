// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace System.ComponentModel.DataAnnotations
{
	/// <summary>
	/// 
	/// </summary>
	public class VerplichtAttribute : RequiredAttribute
	{
		public VerplichtAttribute()
		{
			ErrorMessageResourceType = typeof(Properties.Resources);
			ErrorMessageResourceName = "RequiredAttribute_ValidationError";
		}
	}
}
