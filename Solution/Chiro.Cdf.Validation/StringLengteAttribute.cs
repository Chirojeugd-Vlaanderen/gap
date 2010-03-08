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
