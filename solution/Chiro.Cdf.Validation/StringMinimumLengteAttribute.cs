﻿// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

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