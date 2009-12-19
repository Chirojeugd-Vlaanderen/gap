using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chiro.Gap.Validatie
{
    public class Validator<T>: IValidator<T>
    {
        /// <summary>
        /// Valideert een object.  De bedoeling is dat er hier een aantal
        /// generieke zaken getest kunnen worden.  (Bijv. 'maxlengths'
        /// die gegeven zijn via attributen.)
        /// 
        /// Voorlopig zijn er zo nog geen attributen, dus retourneert
        /// deze functie gewoonweg <c>true</c>.
        /// </summary>
        /// <param name="teValideren"><c>true</c> indien validatie ok</param>
        /// <returns></returns>
        public virtual bool Valideer(T teValideren)
        {
            return true;
        }
    }
}
