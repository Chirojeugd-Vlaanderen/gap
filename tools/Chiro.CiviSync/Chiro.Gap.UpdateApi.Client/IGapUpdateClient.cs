/*
   Copyright 2015 Chirojeugd-Vlaanderen vzw

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
using System.Threading.Tasks;

namespace Chiro.Gap.UpdateApi.Client
{
    public interface IGapUpdateClient
    {
        /// <summary>
        /// Configureert GapUpdate.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="path"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        void Configureren(string server, string path, string username, string password);

        /// <summary>
        /// Rapporteer het gegeven <paramref name="adNummer"/> als ongeldig bij GAP.
        /// </summary>
        /// <param name="adNummer">Als ongeldig te rapporteren AD-nummer</param>
        void OngeldigAdNaarGap(int adNummer);
    }
}