/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
//   Provides an adapter for the <see cref="T:System.ComponentModel.DataAnnotations.DatumInVerledenAttribute" /> attribute.
// </summary>

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
            // TODO find rule
            // return new[] { new ModelClientValidationRequiredRule(base.ErrorMessage) };
        }
    }
}
