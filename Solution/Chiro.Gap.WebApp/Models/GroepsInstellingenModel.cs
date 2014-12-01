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

using System.Collections.Generic;
using Chiro.Gap.Domain;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
    /// <summary>
    /// Model voor overzicht van algemene groepsinfo
    /// </summary>
    /// <remarks>
    /// Aangezien ik de info van een ChiroGroep nodig heb, en de members van IMasterViewModel
    /// hiervan een subset zijn, map ik deze via een impliciete implementatie van IMasterViewModel.
    /// </remarks>
    public class GroepsInstellingenModel : IMasterViewModel
    {
        public GroepsInstellingenModel()
        {
            Detail = new GroepDetail();
            NieuweCategorie = new CategorieInfo();
            NieuweFunctie = new FunctieDetail();
            Types = new List<LidType>();
            NonActieveAfdelingen = new List<AfdelingInfo>();
            Mededelingen = new List<Mededeling>();
        }

        public bool IsLive { get; set; }
        public GroepDetail Detail { get; set; }
        public GroepInfo Info { get { return Detail; } }
        public CategorieInfo NieuweCategorie { get; set; }
        public FunctieDetail NieuweFunctie { get; set; }
        
        public IEnumerable<LidType> Types { get; set; }
       
        public List<AfdelingInfo> NonActieveAfdelingen { get; set; }

        #region IMasterViewModel Members

        int IMasterViewModel.GroepID
        {
            get { return Info.ID; }
        }

        string IMasterViewModel.GroepsNaam
        {
            get { return Info.Naam; }
        }

        string IMasterViewModel.Plaats
        {
            get { return Info.Plaats; }
        }

        string IMasterViewModel.StamNummer
        {
            get { return Info.StamNummer; }
        }

        public string Titel { get; set; }

        public bool? MeerdereGroepen { get; set; }

        public IList<Mededeling> Mededelingen { get; set; }

        public int HuidigWerkJaar { get; set; }

        public bool IsInOvergangsPeriode { get; set; }

        #endregion

        bool IMasterViewModel.IsLive
        {
            get { throw new System.NotImplementedException(); }
        }

        string IMasterViewModel.Titel
        {
            get { throw new System.NotImplementedException(); }
        }

        bool? IMasterViewModel.MeerdereGroepen
        {
            get { throw new System.NotImplementedException(); }
        }

        IList<Mededeling> IMasterViewModel.Mededelingen
        {
            get { throw new System.NotImplementedException(); }
        }

        int IMasterViewModel.HuidigWerkJaar
        {
            get { throw new System.NotImplementedException(); }
        }

        bool IMasterViewModel.IsInOvergangsPeriode
        {
            get { throw new System.NotImplementedException(); }
        }

        GebruikersDetail IMasterViewModel.Ik
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}
