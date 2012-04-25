// <copyright company="Chirojeugd-Vlaanderen vzw">
//   Copyright (c) 2007-2012 Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>
// <summary>
//   Provides an adapter for the <see cref="T:System.ComponentModel.DataAnnotations.StringLengteAttribute" /> attribute.
// </summary>

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace System.Web.Mvc
{
	/// <summary>Provides an adapter for the <see cref="T:System.ComponentModel.DataAnnotations.StringLengteAttribute" /> attribute.</summary>
	public class StringLengteAttributeAdapter : DataAnnotationsModelValidator<StringLengteAttribute>
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Web.Mvc.StringLengteAttributeAdapter" /> class.</summary>
		/// <param name="metadata">The model metadata.</param>
		/// <param name="context">The controller context.</param>
		/// <param name="attribute">The StringLengte attribute.</param>
		public StringLengteAttributeAdapter(ModelMetadata metadata, ControllerContext context, StringLengteAttribute attribute)
			: base(metadata, context, attribute)
		{
		}

		/// <summary>Gets a list of required-value client validation rules.</summary>
		/// <returns>A list of required-value client validation rules.</returns>
		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
		{
			return new[] { new ModelClientValidationStringLengthRule(ErrorMessage, 0, Attribute.MaximumLength) };
		}
	}

	/// <summary>Provides an adapter for the <see cref="T:System.ComponentModel.DataAnnotations.StringLengteAttribute" /> attribute.</summary>
	public class StringMinimumLengteAttributeAdapter : DataAnnotationsModelValidator<StringMinimumLengteAttribute>
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Web.Mvc.StringMinimumLengteAttributeAdapter" /> class.</summary>
		/// <param name="metadata">The model metadata.</param>
		/// <param name="context">The controller context.</param>
		/// <param name="attribute">The StringLengte attribute.</param>
		public StringMinimumLengteAttributeAdapter(ModelMetadata metadata, ControllerContext context, StringMinimumLengteAttribute attribute)
			: base(metadata, context, attribute)
		{
		}

		/// <summary>Gets a list of required-value client validation rules.</summary>
		/// <returns>A list of required-value client validation rules.</returns>
		public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
		{
			return new[] { new ModelClientValidationStringLengthRule(ErrorMessage, Attribute.MinimumLength, int.MaxValue) };
		}
	}
}
