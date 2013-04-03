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

using System.Globalization;

namespace System.ComponentModel.DataAnnotations
{
	/// <summary>
    /// Attribuut voor validatie op minimumlengte van de input
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class StringMinimumLengteAttribute : ValidationAttribute
	{
		// Fields
		public int MinimumLength
		{
			get;
			set;
		}

		// Methods

// ReSharper disable ConvertToLambdaExpression
		public StringMinimumLengteAttribute(int minimumLength)
            : base(delegate { return Properties.Resources.StringMinimumLengthAttribute_ErrorMessage; })
// ReSharper restore ConvertToLambdaExpression
		{
			if (minimumLength < 0)
			{
				throw new ArgumentOutOfRangeException("minimumLength", minimumLength, Properties.Resources.StringMinimumLengthAttribute_InvalidMinimumLenght);
			}
			MinimumLength = minimumLength;
		}

		public override string FormatErrorMessage(string name)
		{
			return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, new object[] { name, MinimumLength });
		}

		public override bool IsValid(object value)
		{
			if (value != null)
			{
				return (((string)value).Length >= MinimumLength);
			}
			return false;
		}
	}
}