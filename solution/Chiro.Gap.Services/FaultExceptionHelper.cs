﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using Chiro.Gap.Domain;
using Chiro.Gap.Poco.Model.Exceptions;
using Chiro.Gap.ServiceContracts.DataContracts;
using Chiro.Gap.ServiceContracts.FaultContracts;

namespace Chiro.Gap.Services
{
    /// <summary>
    /// Klasse die op te werpen FaultExceptions aflevert. Dit om de code leesbaarder te houden.
    /// </summary>
    public static class FaultExceptionHelper
    {
        /// <summary>
        /// Levert een GeenGav-fout op die de service kan throwen.
        /// </summary>
        /// <returns>Een GeenGav-fout</returns>
        public static FaultException<FoutNummerFault> GeenGav()
        {
            return new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = Domain.FoutNummer.GeenGav },
                                                       new FaultReason(Properties.Resources.GeenGav));
        }

        /// <summary>
        /// Levert een 'BestaatAl-fout' op, die de service can throwen
        /// </summary>
        /// <typeparam name="TInfo">Een datacontract van de servie</typeparam>
        /// <param name="bestaande">info over het ding dat al bestaat</param>
        /// <returns>Een 'BestaatAl-fout'</returns>
        public static FaultException<BestaatAlFault<TInfo>> BestaatAl<TInfo>(TInfo bestaande)
        {
            // Hier kan misschien best een overload van gemaakt worden, zodat je een reason kunt
            // meegeven.
            return new FaultException<BestaatAlFault<TInfo>>(new BestaatAlFault<TInfo> { Bestaande = bestaande },
                                                             new FaultReason(Properties.Resources.EntiteitBestondAl));
        }

        public static FaultException<FoutNummerFault> FoutNummer(FoutNummer nummer, string reason)
        {
            return new FaultException<FoutNummerFault>(new FoutNummerFault { FoutNummer = nummer }, new FaultReason(reason));
        }

        /// <summary>
        /// Levert een 'BlokkerendeObjecten'-faultexception op, die de service kan throwen
        /// </summary>
        /// <typeparam name="TInfo">Een datacontract om de blokkerende objecten meet te beschrijven</typeparam>
        /// <param name="infoBlokkerendeObjecten">De blokkerende objecten, als <typeparamref name="TInfo"/></param>
        /// <param name="reden">Wat meer uitleg over de fout</param>
        /// <returns>een 'BlokkerendeObjecten'-faultexception, die de service kan throwen</returns>
        public static FaultException<BlokkerendeObjectenFault<TInfo>>  Blokkerend<TInfo>(List<TInfo> infoBlokkerendeObjecten, string reden)
        {
            return
                new FaultException<BlokkerendeObjectenFault<TInfo>>(
                    new BlokkerendeObjectenFault<TInfo>
                        {
                            Aantal = infoBlokkerendeObjecten.Count,
                            Objecten = infoBlokkerendeObjecten
                        }, new FaultReason(reden));
        }

        /// <summary>
        /// Levert een faultexception i.v.m. 'ongeldige objecten'. Louche.
        /// </summary>
        /// <param name="berichten">Lijstje met foutberichten</param>
        /// <returns>faultexception i.v.m. 'ongeldige objecten'. Louche.</returns>
        public static FaultException<OngeldigObjectFault> Ongeldig(Dictionary<string, FoutBericht> berichten)
        {
            // TODO: Dit is een rare constructie. Waarschijnlijk kan ze vervangen worden door iets beters.
            // (AdresException iemand?)
            throw new FaultException<OngeldigObjectFault>(new OngeldigObjectFault { Berichten = berichten });
        }
    }
}