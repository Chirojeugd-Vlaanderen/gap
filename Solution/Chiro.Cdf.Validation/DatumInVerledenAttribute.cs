// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace System.ComponentModel.DataAnnotations
{
	/// <summary>
    /// Attribuut om na te gaan of een datum wel in het verleden ligt
	/// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class DatumInVerledenAttribute : ValidationAttribute
	{
	    /// <summary>
	    /// Initializes a new instance of the <see cref="DatumInVerledenAttribute"/> class.
	    /// </summary>
	    public DatumInVerledenAttribute()
		{
			ErrorMessageResourceType = typeof(Properties.Resources);
			ErrorMessageResourceName = "PastDate_ValidationError";
		}

	    /// <summary>
	    /// TODO (#190): documenteren
	    /// </summary>
	    /// <param name="value">
	    /// </param>
	    /// <returns>
	    /// </returns>
	    public override bool IsValid(object value)
		{
            // null is ok.  Als de datum niet null mag zijn, moet je maar decoreren met [Verplicht]

		    return value == null || (value is DateTime && (DateTime)value <= DateTime.Now);
		}
	}
}
