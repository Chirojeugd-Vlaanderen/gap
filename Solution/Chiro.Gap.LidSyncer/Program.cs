// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Chiro.Cdf.Ioc;
using Chiro.Gap.Domain;
using Chiro.Gap.Orm;
using Chiro.Gap.Sync;
using Chiro.Gap.Workers;
using Chiro.Gap.Workers.Exceptions;

using Moq;

namespace Chiro.Gap.LidSyncer
{
	class Program
	{
		/// <summary>
		/// Een dom programma dat niets anders doet dan de IoC in orde brengen, en vervolgens een LedenManager
		/// aanroept om de leden met voorbije probeerperiode te syncen.
		/// </summary>
		static void Main(string[] args)
		{
			Factory.ContainerInit();
			MappingHelper.MappingsDefinieren();

			// Leden overzetten vereist super-gav-rechten
			// We regelen dit via mocking.

			var auMgrMock = new Mock<IAutorisatieManager>();
			auMgrMock.Setup(mgr => mgr.IsSuperGav()).Returns(true);
			auMgrMock.Setup(mgr => mgr.IsGavGelieerdePersoon(It.IsAny<int>())).Returns(true);
			auMgrMock.Setup(mgr => mgr.EnkelMijnGelieerdePersonen(It.IsAny<IEnumerable<int>>())).Returns<IEnumerable<int>>(bla => bla.ToList());
			// nifty ;-P

			Factory.InstantieRegistreren(auMgrMock.Object);


			var gpMgr = Factory.Maak<GelieerdePersonenManager>();

			//// Fixen dubbelpuntabonnementen die niet goed overgezet zijn
			// gpMgr.FixDubbelPunt();

			// Manueel verloren dubbelpunt rechttrekken

			HardCodedDubbelpunt();

			//// Overzetten leden na probeerperiode

			//var ledenMgr = Factory.Maak<LedenManager>();
			//ledenMgr.OverZettenNaProbeerPeriode();
		}

		private static void HardCodedDubbelpunt()
		{
			int[] gpIdLijst = { 4350, 4352, 4353, 4354, 4356, 4357, 6760, 6762, 6766, 6787, 6791, 6797,
						7552, 7556, 7580, 7586, 7595, 7634, 7644, 7651, 7658, 7663, 7691, 7703,
						7727, 7732, 7763, 7782, 7800, 7821, 18677, 18706, 18707, 18716, 18722,
						18723, 18732, 18736, 18784, 18826, 18845, 27987, 33613, 33620, 33621,
						33626, 33631, 33635, 33637, 33653, 33663, 33664, 33671, 33674, 33677,
						33685, 36532, 36540, 36550, 36560, 36563, 36568, 36617, 36632, 36646,
						36661, 36662, 36691, 36695, 36728, 36730, 36771, 36797, 36828, 36861,
						49857, 52848, 52929, 52955, 52962, 52967, 62619, 62624, 62630, 62633,
						62636, 62640, 62645, 62665, 62667, 62675, 62681, 62689, 62695, 62704,
						62712, 63440, 63443, 63457, 63462, 63480, 63482, 63484, 63486, 63494,
						63497, 63508, 63511, 63608, 63621, 63680, 63700, 63764, 63773, 63791,
						63871, 64035, 64071, 64072, 64107, 64118, 64132, 64188, 64215, 64218,
						69346, 69388, 70614, 70624, 70659, 70661, 70665, 70667, 70676, 70698,
						70748, 70797, 70817, 71862, 73250, 73251, 73252, 73255, 73277, 73305,
						73311, 73327, 73330, 73351, 97670, 97671, 97677, 97689, 97733, 97745,
						97809, 97832, 97873, 97885, 97889, 97899, 97900, 106623, 106646, 106649,
						106675, 106695, 106707, 106721, 106729, 106747, 106765, 106786, 106807,
						106816, 106847, 106907, 116510, 116512, 116524, 116527, 116529, 116569,
						116573, 116574, 116577, 117054, 117082, 130592, 130603, 130605, 130617,
						130631, 130646, 130650, 132412, 132413, 132429, 132433, 132435, 132442,
						132465, 132477, 132497, 132498, 132510, 132517, 132521, 132523, 133469,
						133487, 133499, 133505, 133546, 133598, 133604, 133628, 134005, 134017,
						134043, 134049, 134081, 138832, 138833, 138835, 138836, 138869, 138882,
						138890, 138901, 138910, 138918, 138942, 138966, 138973, 138980, 138992,
						138995, 139026, 139055, 139112, 139181, 150701, 150744, 150755, 153468,
						153496, 153501, 153519, 153530, 153550, 153579, 153587, 153594, 153607,
						153631, 153758, 153767, 153776, 153826, 153832, 153845, 153851, 153855,
						153856, 158982, 159371, 159387, 159389, 159423, 159470, 159557, 159558,
						159588, 159594, 175181, 175199, 175202, 175214, 175230, 175233, 175241,
						175253, 175254, 175275, 175276, 175283, 175294, 175328, 175331, 607,
						12948, 12985, 13308, 13312, 13314, 13315, 13316, 13318, 13771, 13776,
						13796, 15808, 15880, 29648, 29666, 30225, 30227, 30241, 30249, 30256,
						30263, 30275, 30284, 46413, 46416, 46420, 46456, 46461, 46539, 46553,
						46554, 46555, 47517, 47519, 47523, 47528, 47538, 47545, 47559, 47566,
						47570, 47587, 47592, 58575, 58587, 58595, 58627, 58629, 58636, 58641,
						58656, 58698, 58800, 58808, 58833, 58846, 58919, 58921, 58926, 59012,
						59014, 59015, 59016, 66228, 66327, 66465, 66474, 66491, 66579, 66956,
						66994, 67012, 67013, 69108, 69153, 79218, 79296, 81169, 81170, 104576,
						92470, 92501, 92516, 92530, 92539, 92547, 92554, 92575, 92580, 92605,
						92620, 92636, 92638, 92646, 100343, 100347, 100352, 100356, 100366,
						100371, 100377, 100406, 102434, 102442, 102510, 102525, 102556, 102578,
						102586, 104516, 104531, 104544, 104553, 104562, 104567, 104571, 104602,
						104607, 104644, 104693, 104707, 104710, 104742, 104770, 104779, 105521,
						105525, 105541, 105551, 105570, 105607, 105618, 106562, 106581, 106605,
						106617, 119949, 119991, 120003, 120022, 126568, 126601, 126628, 134604,
						134638, 134766, 134789, 134795, 134808, 134857, 134889, 134897, 134938,
						134982, 135041, 135042, 135043, 135485, 135582, 135620, 135631, 135773,
						135827, 136161, 136210, 136217, 137393, 137396, 137444, 137453, 137713,
						137722, 137723, 137735, 137749, 137750, 137755, 137756, 137759, 137762,
						137771, 137775, 137776, 137788, 137792, 137809, 137820, 137851, 137852,
						137853, 137901, 137915, 137922, 137924, 137926, 137956, 137980, 137989,
						148682, 150597, 150616, 150618, 150626, 150640, 158173, 158197, 158264,
						158293, 164382, 164477, 175340, 168430, 168602, 168637, 168646, 176918,
						176922, 176957, 176999, 177000, 177001, 177027, 177028, 177094, 177153,
						177154, 177221, 177277, 177279, 177280, 177439, 177459, 177585, 177617,
						177622
						 };

			var gpMgr = Factory.Maak<GelieerdePersonenManager>();

			foreach (int gpid in gpIdLijst)
			{
				var gp = gpMgr.Ophalen(gpid, PersoonsExtras.Adressen);

				// Eerst abonnement opheffen

				//Debug.Assert(gp.Persoon.DubbelPuntAbonnement);
				gp.Persoon.DubbelPuntAbonnement = false;
				gpMgr.Bewaren(gp, PersoonsExtras.Geen);

				// Opnieuw bestellen

				try
				{
					gpMgr.DubbelpuntBestellen(gp);
				}
				catch (FoutNummerException ex)
				{
					if (ex.FoutNummer == FoutNummer.AdresOntbreekt)
					{
						Console.WriteLine("Geen adres voor gelieerdepersoon {0}", gp.ID);
					}
					else throw;
				}

				gpMgr.Bewaren(gp, PersoonsExtras.Geen);
			}
		}
	}
}
