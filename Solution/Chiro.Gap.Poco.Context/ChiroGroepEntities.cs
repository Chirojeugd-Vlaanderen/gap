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
﻿using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Chiro.Cdf.Poco;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.Poco.Context
{
    public partial class ChiroGroepEntities : DbContext, IContext
    {
        public ChiroGroepEntities()
            : base("name=ChiroGroepEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<Groep> Groep { get; set; }
        public DbSet<Persoon> Persoon { get; set; }
        public DbSet<CommunicatieVorm> CommunicatieVorm { get; set; }
        public DbSet<GelieerdePersoon> GelieerdePersoon { get; set; }
        public DbSet<Adres> Adres { get; set; }
        public DbSet<PersoonsAdres> PersoonsAdres { get; set; }
        public DbSet<GroepsWerkJaar> GroepsWerkJaar { get; set; }
        public DbSet<Lid> Lid { get; set; }
        public DbSet<Gav> Gav { get; set; }
        public DbSet<GebruikersRecht> GebruikersRecht { get; set; }
        public DbSet<Afdeling> Afdeling { get; set; }
        public DbSet<AfdelingsJaar> AfdelingsJaar { get; set; }
        public DbSet<OfficieleAfdeling> OfficieleAfdeling { get; set; }
        public DbSet<Categorie> Categorie { get; set; }
        public DbSet<CommunicatieType> CommunicatieType { get; set; }
        public DbSet<Functie> Functie { get; set; }
        public DbSet<StraatNaam> StraatNaam { get; set; }
        public DbSet<WoonPlaats> WoonPlaats { get; set; }
        public DbSet<PersoonsVerzekering> PersoonsVerzekering { get; set; }
        public DbSet<VerzekeringsType> VerzekeringsType { get; set; }
        public DbSet<Land> Land { get; set; }
        public DbSet<Plaats> Plaats { get; set; }
        public DbSet<Uitstap> Uitstap { get; set; }
        public DbSet<Deelnemer> Deelnemer { get; set; }
        public DbSet<Abonnement> Abonnement { get; set; }
        public DbSet<Publicatie> Publicatie { get; set; }
    }
}