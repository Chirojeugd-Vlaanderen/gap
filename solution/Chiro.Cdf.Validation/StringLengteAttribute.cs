// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace System.ComponentModel.DataAnnotations
{
	/// <summary>
	/// Attribuut voor validatie op maximumlengte van de input
	/// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
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
