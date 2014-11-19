using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nancy.ModelBinding;

using Chiro.Gap.UpdateApi.Workers;
using Nancy;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.Domain;

namespace Chiro.Gap.UpdateApi
{
    public class PersoonModule: Nancy.NancyModule, IDisposable
    {
        private IPersoonUpdater _persoonUpdater;

        public PersoonModule(IPersoonUpdater persoonUpdater)
        {
            // persoonUpdater, aangeleverd door de dependency injection container, is disposable,
            // en moet achteraf vrijgegeven worden. Dat doen we in Dispose() van deze module.

            _persoonUpdater = persoonUpdater;

            Get["/"] = _ => "Hello World!";
            Get["/persoon/{id}"] = parameters =>
                {
                    int id = parameters.id;
                    return String.Format("You requested {0}", id);
                };
            // You can test this with curl:
            // curl -X PUT -d PersoonId=2 -d CiviId=3 localhost:50673/persoon
            Put["/persoon"] = _ => {
                Models.Persoon model = this.Bind();
                if (model.CiviId > 0)
                {
                    try
                    {
                        _persoonUpdater.CiviIdToekennen(model.PersoonId, model.CiviId, true);
                    }
                    catch (FoutNummerException ex)
                    {
                        if (ex.FoutNummer == FoutNummer.PersoonNietGevonden)
                        {
                            return HttpStatusCode.NotFound;
                        }
                        else
                        {
                            throw;
                        }
                    };
                    return HttpStatusCode.OK;
                }
                else
                {
                    return HttpStatusCode.NotImplemented;
                }
            };
        }

        #region Disposable etc

        // Ik heb die constructie met 'disposed' en 'disposing' nooit begrepen.
        // Maar ze zeggen dat dat zo moet :-)

        private bool disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _persoonUpdater.Dispose();
                }
                disposed = true;
            }
        }

        ~PersoonModule()
        {
            Dispose(false);
        }

        #endregion
    }
}