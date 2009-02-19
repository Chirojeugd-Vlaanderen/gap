using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cg2.Orm.DataInterfaces;
using Cg2.Data.Ef;
using Cg2.Orm;

namespace Cg2.Workers
{
    public class PersonenManager
    {
        private IPersonenDao _dao = new PersonenDao();

        /// <summary>
        /// Data Access Object dat aangesproken kan worden
        /// voor CRUD-operaties
        /// </summary>
        public IPersonenDao Dao
        {
            get { return _dao; }
        }

        /// <summary>
        /// Maak persoon lid
        /// </summary>
        /// <param name="p">Lid te maken persoon</param>
        /// <param name="g">Groep waarvan persoon lid moet worden</param>
        /// <returns>Persoon met bijhorend lidobject</returns>
        public Persoon LidMaken(Persoon p, Groep g)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Voegt een CommunicatieVorm toe aan een persoon
        /// </summary>
        /// <param name="p">persoon</param>
        /// <param name="type">communicatietype</param>
        /// <param name="nr">telefoonnr, e-mailadres,...</param>
        /// <param name="voorkeur">true indien voorkeur</param>
        public void CommunicatieToevoegen(Persoon p, CommunicatieType type
            , string nr, bool voorkeur)
        {
            CommunicatieVorm cv = new CommunicatieVorm();
            cv.Nummer = nr;
            cv.Type = type;
            cv.Persoon = p;

            // TODO: validatie, en checken op dubbels

            if (voorkeur)
            {
                CommunicatieVorm bestaandeCv
                    = (from CommunicatieVorm v in p.Communicatie
                       where v.Type == type && v.Voorkeur
                       select v).SingleOrDefault<CommunicatieVorm>();
                if (bestaandeCv != null)
                {
                    bestaandeCv.Voorkeur = false;
                }
            }

            cv.Voorkeur = voorkeur;

            p.Communicatie.Add(cv);
        }
    }
}
