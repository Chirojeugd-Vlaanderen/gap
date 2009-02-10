using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Cg2.Orm
{
    [DataContract]
    public enum GeslachtsType
    {
        [EnumMember] Man = 1
        , [EnumMember] Vrouw = 2
        , [EnumMember] Onbekend = 0
    }

    public partial class Persoon : IBasisEntiteit
    {
        public Persoon()
        {
            BusinessKey = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            return this.MyEquals(obj);
        }

        public override int GetHashCode()
        {
            return this.MyGetHashCode();
        }

        public GeslachtsType Geslacht
        {
            get { return (GeslachtsType)this.GeslachtsInt; }
            set { this.GeslachtsInt = (int)value; }
        }

        public void CommunicatieToevoegen(CommunicatieType type, string nr
            , bool voorkeur)
        {
            CommunicatieVorm cv = new CommunicatieVorm();
            cv.Nummer = nr;
            cv.Type = type;
            cv.Persoon = this;

            // TODO: validatie, en checken op dubbels

            if (voorkeur)
            {
                CommunicatieVorm bestaandeCv 
                    = (from CommunicatieVorm v in Communicatie
                       where v.Type == type && v.Voorkeur
                       select v).SingleOrDefault<CommunicatieVorm>();
                if (bestaandeCv != null)
                {
                    bestaandeCv.Voorkeur = false;
                }
            }

            cv.Voorkeur = voorkeur;

            this.Communicatie.Add(cv);
        }
    }
}
