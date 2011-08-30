using System;
using Chiro.Poc.WcfService.ServiceContracts;

namespace Chiro.Poc.WcfService
{
    /// <summary>
    /// Domme service ter illustratie
    /// </summary>
    public class Service1 : IService1
    {
        /// <summary>
        /// Deze method levert gewoon een hello world-achtige string op.
        /// </summary>
        /// <returns>Een hello-world string</returns>
        public string Hallo()
        {
            return "Antwoord van de service.";
        }
    }
}
