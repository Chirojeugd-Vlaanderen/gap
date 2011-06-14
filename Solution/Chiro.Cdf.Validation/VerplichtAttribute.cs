// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace System.ComponentModel.DataAnnotations
{
	/// <summary>
    /// Naar het Nederlands vertaalde vorm van <see cref="T:System.ComponentModel.DataAnnotations.RequiredAttribute" /> class. 
	/// </summary>
    /// <seealso cref="T:System.Web.Mvc.VerplichtAttributeAdapter"/>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class VerplichtAttribute : RequiredAttribute
	{
		public VerplichtAttribute()
		{
			ErrorMessageResourceType = typeof(Properties.Resources);
			ErrorMessageResourceName = "RequiredAttribute_ValidationError";
		}
	}
}
