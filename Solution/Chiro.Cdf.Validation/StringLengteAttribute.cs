// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace System.ComponentModel.DataAnnotations
{
	/// <summary>
	/// 
	/// </summary>
	public class StringLengteAttribute : StringLengthAttribute
	{
		public StringLengteAttribute(int maximumLength)
			: base(maximumLength)
		{
			ErrorMessageResourceType = typeof(Properties.Resources);
			ErrorMessageResourceName = "StringLengthAttribute_ValidationError";
		}
	}
}
