// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

namespace System.ComponentModel.DataAnnotations
{
	/// <summary>
	/// 
	/// </summary>
	public class GeboorteDatumInVerledenAttribute : ValidationAttribute
	{
		public GeboorteDatumInVerledenAttribute()
		{
			ErrorMessageResourceType = typeof(Properties.Resources);
			ErrorMessageResourceName = "GeboorteDatumInVerledenAttribute_ValidationError";
		}

		public override bool IsValid(object value)
		{
            // Moeten we nog nagaan of value van het type DateTime is? Of mogen we
			// ervan uitgaan dat deze attribute alleen toegepast wordt op datums?
            // Natuurlijk moet dat nog gecontroleerd worden. Object kan namelijk alles zijn, en ook null
		    if (value == null || !(value is DateTime))
		    {
		        return false;
		    }
		    else
		    {
		        return (DateTime)value <= DateTime.Now;
		    }
			
			
		}
	}
}
