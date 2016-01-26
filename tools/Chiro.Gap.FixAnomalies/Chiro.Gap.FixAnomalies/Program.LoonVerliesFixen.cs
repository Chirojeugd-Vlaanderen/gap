/*
   Copyright 2015,2016 Chirojeugd-Vlaanderen vzw

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
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chiro.Cdf.Poco;
using Chiro.Cdf.ServiceHelper;
using Chiro.CiviCrm.Api;
using Chiro.CiviCrm.Api.DataContracts;
using Chiro.Gap.Domain;
using Chiro.Gap.FixAnomalies.Properties;
using Chiro.Gap.Poco.Context;
using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.FixAnomalies
{
    partial class Program
    {
        private static void LoonVerliesFixen(ServiceHelper serviceHelper, string apiKey, string siteKey)
        {
            Console.WriteLine(Resources.Program_LoonVerliesFixen_Opvragen_actieve_memberships_met_loonverlies_uit_CiviCRM_);

            // Lijst ad-nummers uit civi
            var civiResult =
                serviceHelper.CallService<ICiviCrmApi, ApiResultStrings>(
                    svc => svc.ChiroDiagnosticsMembersVerzekerdLoonVerlies(apiKey, siteKey));
            if (civiResult.IsError != 0)
            {
                throw new ApplicationException(civiResult.ErrorMessage);
            }
            var civiAdNrs = (from v in civiResult.Values
                             select int.Parse(v.First())).ToArray();
            Console.WriteLine(Resources.Program_Main_Dat_zijn_er__0__, civiResult.Count);

            // Lijst ad-nummers uit GAP
            int werkjaar = HuidigWerkJaar();
            Console.WriteLine(Resources.Program_LoonVerliesFixen_Verzekeringen_loonverlies_opvragen_uit_GAP_);
            int[] gapAdNrs = AdNrsLoonverlies(werkjaar);
            Console.WriteLine(Resources.Program_Main_Dat_zijn_er__0__, gapAdNrs.Count());

            // Voor 2015: lijst ad-nummers uit Kipdorp
            if (werkjaar == 2015)
            {
                Console.WriteLine(Resources.Program_LoonVerliesFixen_Personen_waarvoor_Kipadmin_loonverlies_factureerde__maar_die_niet_aangesloten_zijn_);
                int[] kipAdNrs = LoonVerliesPreMigratie2015();
                AEnNietB(kipAdNrs, civiAdNrs);
                Console.WriteLine(Resources.Program_LoonVerliesFixen_Ofwel_zijn_er_zo_geen__ofwel_is_Zulma_op_de_hoogte_);
            }

            var teBewaren = AEnNietB(gapAdNrs, civiAdNrs);
            Console.WriteLine(Resources.Program_LoonVerliesFixen__0__verzekeringen_te_kort_in_CiviCRM_, teBewaren.Count);

            // TODO: command line switch om deze vraag te vermijden.
            Console.Write(Resources.Program_Main_Meteen_syncen__);
            string input = Console.ReadLine();

            if (input.ToUpper() == "J" || input.ToUpper() == "Y")
            {
                // Uiteindelijk hebben we dit nog niet nodig.
                // Zie #4586.
                throw new NotImplementedException();
                //VerzekeringenNaarCivi(teBewarenLeden, serviceHelper);
            }
        }

        /// <summary>
        /// Levert een lijst van AD-nummers van leden waarvan de probeerperiode voorbij is,
        /// met een verzekering loonverlies in het gegeven <paramref name="werkJaar"/>.
        /// </summary>
        /// <param name="werkJaar"></param>
        /// <returns>
        /// Lijst van AD-nummers van leden waarvan de probeerperiode voorbij is,
        /// met een verzekering loonverlies in het gegeven <paramref name="werkJaar"/>.
        /// </returns>
        private static int[] AdNrsLoonverlies(int werkJaar)
        {
            // Dit zou beter gebeuren met dependency injection. Maar het is en blijft een hack.
            using (var context = new ChiroGroepEntities())
            {
                // Verzekeringen loonverlies eindigen op 31 augustus. Deze informatie
                // zou beter uit een worker komen.
                var eindePeriode = new DateTime(werkJaar + 1, 8, 31);

                var repositoryProvider = new RepositoryProvider(context);
                var ledenRepo = repositoryProvider.RepositoryGet<Lid>();

                var alles = from ld in ledenRepo.Select()
                    where ld.GroepsWerkJaar.WerkJaar == werkJaar && (ld.EindeInstapPeriode < DateTime.Today && (!ld.NonActief || ld.UitschrijfDatum > ld.EindeInstapPeriode))
                        // Enkel personen waarvan AD-nummers al gekend zijn. In het andere geval
                        // zal gapmaintenance wel syncen.
                          && ld.GelieerdePersoon.Persoon.AdNummer.HasValue
                        // Enkel leden van actieve groepen
                          && ld.GroepsWerkJaar.Groep.StopDatum == null
                        // Met verzekering loonverlies
                          &&
                          ld.GelieerdePersoon.Persoon.PersoonsVerzekering.Any(
                              pv => pv.VerzekeringsType.ID == (int) Verzekering.LoonVerlies && pv.Tot == eindePeriode)
                    select ld.GelieerdePersoon.Persoon.AdNummer ?? 0;
                return alles.OrderBy(ad => ad).ToArray();
            }
        }

        /// <summary>
        /// Dit zijn de AD-nummers van iedereen die een verzekering loonverlies had voor 2015-2016 op het
        /// moment van migratie, waarbij de factuur doorgeboekt was naar Winbooks.
        /// </summary>
        /// <returns></returns>
        private static int[] LoonVerliesPreMigratie2015()
        {
            return new[]
            {
                1599, 5732, 5733, 5761, 6400, 6610, 6905, 9356, 10664, 10968, 12532, 12711, 12938, 13619, 13657, 14526,
                16145, 16185, 17008, 18540, 18541, 22589, 24208, 25324, 25852, 26668, 27849, 28500, 31979, 33079, 36969,
                39314, 39796, 40017, 40498, 40967, 41021, 41821, 45701, 46598, 47565, 50180, 51465, 51629, 51699, 52766,
                56759, 56766, 59836, 61559, 61806, 62303, 65688, 65692, 65872, 65968, 66297, 66455, 67326, 68772, 70214,
                70428, 72730, 80558, 81048, 84965, 85302, 86412, 88517, 93307, 94435, 94865, 97968, 98017, 99373, 101660,
                102376, 102502, 102931, 102951, 103758, 103759, 108388, 108389, 108890, 109573, 110079, 113836, 114413,
                115575, 116894, 120706, 121628, 122330, 122689, 123543, 123677, 124788, 124958, 125686, 126458, 127668,
                127669, 127670, 128675, 128744, 129498, 129666, 131058, 132623, 133038, 134001, 136429, 136525, 137628,
                137642, 138060, 139053, 139056, 139211, 139795, 140047, 140049, 140503, 140940, 142687, 143376, 143913,
                144038, 145042, 145404, 145850, 145888, 146490, 147377, 147886, 147889, 148802, 151041, 151043, 151327,
                152483, 152704, 152719, 152978, 153014, 153019, 153365, 153390, 153457, 153722, 153725, 153770, 154367,
                154705, 154724, 154929, 155004, 155111, 155171, 155308, 155333, 155868, 155870, 155950, 156184, 156558,
                157014, 157061, 157147, 157275, 157292, 157585, 158090, 158469, 158472, 159192, 159305, 159468, 160017,
                161038, 161649, 161703, 162046, 162066, 162071, 162459, 162494, 162496, 162498, 162501, 162735, 162782,
                162940, 162996, 163097, 163099, 163104, 163153, 163562, 163716, 164251, 164392, 164393, 164470, 164498,
                165027, 165173, 165334, 165612, 165803, 166062, 166182, 166260, 166348, 166425, 166456, 166714, 166715,
                167407, 167776, 167777, 167900, 167904, 168079, 168436, 170075, 170151, 170285, 170437, 171284, 171287,
                171592, 171625, 171717, 171836, 172162, 172932, 173039, 173471, 173476, 173513, 173517, 173792, 173999,
                174001, 174100, 174103, 174575, 174578, 174580, 175086, 175094, 175248, 175250, 175251, 175793, 175891,
                176313, 176470, 176758, 176799, 177371, 177407, 177412, 177952, 178130, 178142, 178294, 178296, 178553,
                178602, 179088, 179116, 179696, 179824, 180011, 180013, 180359, 180609, 181583, 182311, 183496, 183694,
                183766, 184517, 184673, 184692, 184706, 190435, 191455, 192268, 195025, 199135, 202363, 202404, 202405,
                205459, 208553, 214106, 217543, 217565, 218174, 218175, 218897, 219877, 219938, 220919, 222327, 224307,
                225203, 227690, 227697, 227890, 229388, 229389, 229629, 233991, 236081, 236364, 236585, 237258, 240574,
                240736, 241522, 241549, 242815, 251183, 252407, 253101, 254306, 259643, 260417, 260835, 265875, 272611,
                273208, 273211, 273450, 275609, 276105, 279357, 279462, 280321, 281670, 281857, 283908, 286673, 295598,
                299568, 299586, 299587, 299588, 300314, 300890, 300902, 301237, 301258, 303876, 304118, 304145, 304146,
                305123, 306163, 306238, 306239, 306241, 312422, 312731, 313575, 313577, 315726, 316084, 317562, 317677,
                318928, 319003, 319005, 320430, 320431, 322373, 323056, 323450, 323451, 323568, 323572, 324632, 326838,
                327457, 328911, 333105, 333665, 334229, 334690, 338756, 340999, 341022, 342325, 342327, 342328, 342470,
                346544, 350674, 350675, 350740, 352097, 355152, 355153, 355154, 355155, 355156, 355185, 355558, 355851,
                355852, 355853, 355859, 355861, 355896, 356028, 356136, 361290, 361292
            };
        }
    }
}
