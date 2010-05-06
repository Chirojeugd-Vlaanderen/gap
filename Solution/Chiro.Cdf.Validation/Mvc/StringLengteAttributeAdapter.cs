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
            return new ModelClientValidationStringLengthRule[] { new ModelClientValidationStringLengthRule(base.ErrorMessage, 0, base.Attribute.MaximumLength ) };
        }
    }
}
