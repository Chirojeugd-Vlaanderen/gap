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
using System.Diagnostics;

namespace CgBll
{
    /// <summary>
    /// business layer class voor PersoonsAdres (test)
    /// </summary>
    public class PersoonsAdresBll
    {
        private ChiroGroepEntities context;

        /// <summary>
        /// Standaardconstructor; creert context
        /// </summary>
        public PersoonsAdresBll()
        {
            context = new ChiroGroepEntities();
        }

        public void PersoonsAdresUpdaten(PersoonsAdres bijgewerktAdres
            , PersoonsAdres oorspronkelijkAdres)
        {
            try
            {
                Debug.WriteLine("AdresTypeID van bijgewerktAdres: " + bijgewerktAdres.AdresType.AdresTypeID);

                // De oorspronkelijke gevens opnieuw attachen aan
                // de ObjectContext.

                context.Attach(oorspronkelijkAdres);
                
                // Nieuwe gegevens overzetten naar oorspronkelijkAdres
                // zodat de objectcontext ze opmerkt.

                context.ApplyReferencePropertyChanges(bijgewerktAdres
                    , oorspronkelijkAdres);
                context.ApplyPropertyChanges(bijgewerktAdres.EntityKey.EntitySetName
                    , bijgewerktAdres);
                context.SaveChanges();
            }
            catch (OptimisticConcurrencyException)
            {
                throw;
            }
        }
    }
}
