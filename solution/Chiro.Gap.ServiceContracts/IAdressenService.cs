// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ServiceModel;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.Services
{
    [ServiceContract]
    public interface IAdressenService
    {
        [OperationContract]
        Adres Ophalen(AdresInfo adresInfo);
    }
}
