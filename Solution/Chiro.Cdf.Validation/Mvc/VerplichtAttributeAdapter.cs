using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace System.Web.Mvc
{
    /// <summary>Provides an adapter for the <see cref="T:System.ComponentModel.DataAnnotations.VerplichtAttribute" /> attribute.</summary>
    public class VerplichtAttributeAdapter : DataAnnotationsModelValidator<VerplichtAttribute>
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Mvc.VerplichtAttributeAdapter" /> class.</summary>
        /// <param name="metadata">The model metadata.</param>
        /// <param name="context">The controller context.</param>
        /// <param name="attribute">The required attribute.</param>
        public VerplichtAttributeAdapter(ModelMetadata metadata, ControllerContext context, VerplichtAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        /// <summary>Gets a list of required-value client validation rules.</summary>
        /// <returns>A list of required-value client validation rules.</returns>
        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            return new[] { new ModelClientValidationRequiredRule(ErrorMessage) };
        }
    }
}
