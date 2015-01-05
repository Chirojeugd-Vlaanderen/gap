/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
