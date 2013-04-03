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
﻿using System;
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
