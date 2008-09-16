using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CgDal;
using System.Data;

namespace CgBll
{
    /// <summary>
    /// Business link layer voor AdresType.  Ik ben er niet zeker van
    /// of er voor elk entity wel een aparte klasse gemaakt moet worden.
    /// Dat zou nog wel nuttig kunnen zijn, maar is het dan niet 
    /// interessanter om de context te delen?
    /// </summary>
    public class AdresTypeBll
    {
        private ChiroGroepEntities context;

        protected AdresType AdresTypeGet(int adresTypeID)
        {
            var q = from a in context.AdresType
                    where a.AdresTypeID == adresTypeID
                    select a;

            AdresType result = q.First();

            context.Detach(result);
            return result;
        }

        public AdresTypeBll()
        {
            context = new ChiroGroepEntities();
        }

        public AdresType Thuis {get {return AdresTypeGet(1);}}
        public AdresType Kot { get { return AdresTypeGet(2); } }
        public AdresType Werk { get { return AdresTypeGet(3); } }
        public AdresType Overig { get { return AdresTypeGet(4); } }
    }
}
