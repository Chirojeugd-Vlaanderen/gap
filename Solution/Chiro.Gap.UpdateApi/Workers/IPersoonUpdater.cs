﻿using System;
namespace Chiro.Gap.UpdateApi.Workers
{
    public interface IPersoonUpdater: IDisposable
    {
        void AdNummerToekennen(int persoonId, int adNummer);
        void AdNummerVervangen(int oudAd, int nieuwAd);
        void CiviIdToekennen(int persoonID, int civiID, bool dupesMergen);
        void GroepDesactiveren(string stamNr, DateTime? stopDatum);
    }
}