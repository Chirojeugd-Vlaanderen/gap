using System.Linq;

using Chiro.Gap.Poco.Model;
using Chiro.Gap.WorkerInterfaces;

namespace Chiro.Gap.Services
{
    public class GavChecker
    {
        private readonly IAutorisatieManager _autorisatieMgr;

        public GavChecker(IAutorisatieManager autorisatieMgr)
        {
            _autorisatieMgr = autorisatieMgr;
        }

        public void Check(Afdeling g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g.ChiroGroep))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(Uitstap g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(Deelnemer g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(Functie g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g.Groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(Gav g)
        {
            if (g == null)
            {
                throw FaultExceptionHelper.GeenGav();
            }
            if ( ! g.Persoon.SelectMany(persoon => persoon.GelieerdePersoon).Any(gelieerdePersoon => _autorisatieMgr.IsGav(gelieerdePersoon)))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(Categorie g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g.Groep))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(GelieerdePersoon g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(Lid g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g.GelieerdePersoon))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(AfdelingsJaar g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g.GroepsWerkJaar))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(GroepsWerkJaar g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void Check(Groep g)
        {
            if (g == null || !_autorisatieMgr.IsGav(g))
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }

        public void CheckSuperGav()
        {
            if (!_autorisatieMgr.IsSuperGav())
            {
                throw FaultExceptionHelper.GeenGav();
            }
        }
    }
}