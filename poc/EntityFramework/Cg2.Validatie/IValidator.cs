using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cg2.Validatie
{
    public interface IValidator<T>
    {
        bool Valideer(T teValideren);
    }
}
