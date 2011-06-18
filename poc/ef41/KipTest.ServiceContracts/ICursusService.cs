using System;
using System.ServiceModel;

namespace KipTest.ServiceContracts
{
    // Geen datacontracts, want enkel POC

    [ServiceContract]
    public interface ICursusService
    {
        [OperationContract]
        int CursusMaken(string naam, DateTime startDatum, DateTime stopDatum);

        [OperationContract]
        int DeelnemerInschrijven(int cursusID, string deelnemerNaam);

        [OperationContract]
        string[] DeelnemersOphalen(int cursusID);

        [OperationContract]
        string Hallo();

        [OperationContract]
        void DeelnemerVerwijderen(int cursusID, string naam);

        [OperationContract]
        void DeelnemerVerhuizen(int cursusVanID, int cursusTotID, string naam);
    }
}
