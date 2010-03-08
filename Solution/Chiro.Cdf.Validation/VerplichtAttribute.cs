using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
	public class VerplichtAttribute : RequiredAttribute
	{
		public VerplichtAttribute()
			: base()
		{
			this.ErrorMessageResourceType = typeof(Properties.Resources);
			this.ErrorMessageResourceName = "RequiredAttribute_ValidationError";
		}
	}
}
