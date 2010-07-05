using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace System.Web.Mvc
{
    /// <summary>Provides an adapter for the <see cref="T:System.ComponentModel.DataAnnotations.DatumInVerledenAttribute" /> attribute.</summary>
    public class DatumInVerledenAttributeAdapter : DataAnnotationsModelValidator<DatumInVerledenAttribute>
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Web.Mvc.DatumInVerledenAttributeAdapter" /> class.</summary>
        /// <param name="metadata">The model metadata.</param>
        /// <param name="context">The controller context.</param>
        /// <param name="attribute">The required attribute.</param>
        public DatumInVerledenAttributeAdapter(ModelMetadata metadata, ControllerContext context, DatumInVerledenAttribute attribute)
            : base(metadata, context, attribute)
        {
        }

        /// <summary>Gets a list of required-value client validation rules.</summary>
        /// <returns>A list of required-value client validation rules.</returns>
        public override IEnumerable<ModelClientValidationRule> GetClientValidationRules()
        {
            return null;
            // TODO: find rule
            return new ModelClientValidationRequiredRule[] { new ModelClientValidationRequiredRule(base.ErrorMessage) };
        }
    }
}
