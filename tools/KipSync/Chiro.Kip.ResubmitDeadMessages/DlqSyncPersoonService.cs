/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using Chiro.Kip.ServiceContracts;
using Chiro.Kip.ServiceContracts.DataContracts;

namespace Chiro.Kip.ResubmitDeadMessages
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, AddressFilterMode=AddressFilterMode.Any)]
	class DlqSyncPersoonService: ISyncPersoonService
	{
		private ServiceReference1.ISyncPersoonService _sync;
		
		public DlqSyncPersoonService()
		{
			_sync = new ServiceReference1.SyncPersoonServiceClient("KipSyncEndPoint");
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void PersoonUpdaten(Persoon persoon)
		{
			Console.WriteLine(
				"Updaten van AD{2} {0} {1} was mislukt.",
				persoon.VoorNaam, persoon.Naam, persoon.AdNummer);
			var mqProp =
				OperationContext.Current.IncomingMessageProperties[MsmqMessageProperty.Name] as MsmqMessageProperty;
			if (mqProp.DeliveryFailure == DeliveryFailure.ReachQueueTimeout ||
				mqProp.DeliveryFailure == DeliveryFailure.ReceiveTimeout)
			{
				Console.WriteLine("Opnieuw proberen...");
				_sync.PersoonUpdaten(persoon);
				Console.WriteLine("Verzonden!");
			}

		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void StandaardAdresBewaren(Adres adres, IEnumerable<Bewoner> bewoners)
		{
			Console.WriteLine("Standaardadres bewaren was mislukt.");
			var mqProp =
				OperationContext.Current.IncomingMessageProperties[MsmqMessageProperty.Name] as MsmqMessageProperty;
			if (mqProp.DeliveryFailure == DeliveryFailure.ReachQueueTimeout ||
				mqProp.DeliveryFailure == DeliveryFailure.ReceiveTimeout)
			{
				Console.WriteLine("Opnieuw proberen...");
				_sync.StandaardAdresBewaren(adres, bewoners.ToArray());
				Console.WriteLine("Verzonden!");
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void CommunicatieToevoegen(Persoon persoon, CommunicatieMiddel communicatieMiddel)
		{
			Console.WriteLine(
				"Communicatie {3} toevoegen voor AD{2} {0} {1} was mislukt.",
				persoon.VoorNaam, persoon.Naam, persoon.AdNummer, communicatieMiddel.Waarde);
			var mqProp =
				OperationContext.Current.IncomingMessageProperties[MsmqMessageProperty.Name] as MsmqMessageProperty;
			if (mqProp.DeliveryFailure == DeliveryFailure.ReachQueueTimeout ||
				mqProp.DeliveryFailure == DeliveryFailure.ReceiveTimeout)
			{
				Console.WriteLine("Opnieuw proberen...");
				_sync.CommunicatieToevoegen(persoon, communicatieMiddel);
				Console.WriteLine("Verzonden!");
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AlleCommunicatieBewaren(Persoon persoon, IEnumerable<CommunicatieMiddel> communicatieMiddelen)
		{
			Console.WriteLine(
				"Alle communicatie bewaren voor AD{2} {0} {1} was mislukt.",
				persoon.VoorNaam, persoon.Naam, persoon.AdNummer);
			var mqProp =
				OperationContext.Current.IncomingMessageProperties[MsmqMessageProperty.Name] as MsmqMessageProperty;
			if (mqProp.DeliveryFailure == DeliveryFailure.ReachQueueTimeout ||
				mqProp.DeliveryFailure == DeliveryFailure.ReceiveTimeout)
			{
				Console.WriteLine("Opnieuw proberen...");
				_sync.AlleCommunicatieBewaren(persoon, communicatieMiddelen.ToArray());
				Console.WriteLine("Verzonden!");
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void CommunicatieVerwijderen(Persoon persoon, CommunicatieMiddel communicatieMiddel)
		{
			Console.WriteLine(
				"Communicatie {3} verwijderen voor AD{2} {0} {1} was mislukt.",
				persoon.VoorNaam, persoon.Naam, persoon.AdNummer, communicatieMiddel.Waarde);
			var mqProp =
				OperationContext.Current.IncomingMessageProperties[MsmqMessageProperty.Name] as MsmqMessageProperty;
			if (mqProp.DeliveryFailure == DeliveryFailure.ReachQueueTimeout ||
				mqProp.DeliveryFailure == DeliveryFailure.ReceiveTimeout)
			{
				Console.WriteLine("Opnieuw proberen...");
				_sync.CommunicatieVerwijderen(persoon, communicatieMiddel);
				Console.WriteLine("Verzonden!");
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void LidBewaren(int adNummer, LidGedoe gedoe)
		{
			Console.WriteLine("Lid bewaren was mislukt: AD{0}", adNummer);
			var mqProp =
				OperationContext.Current.IncomingMessageProperties[MsmqMessageProperty.Name] as MsmqMessageProperty;
                        if (mqProp.DeliveryFailure == DeliveryFailure.ReachQueueTimeout ||
				mqProp.DeliveryFailure == DeliveryFailure.ReceiveTimeout)
                        {
                        	Console.WriteLine("Opnieuw proberen...");
                        	_sync.LidBewaren(adNummer, gedoe);
                        	Console.WriteLine("Verzonden!");
                        }
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void NieuwLidBewaren(PersoonDetails details, LidGedoe lidGedoe)
		{
			Console.WriteLine("Nieuw Lid bewaren was mislukt: {0} {1}", details.Persoon.VoorNaam, details.Persoon.Naam);
			var mqProp =
				OperationContext.Current.IncomingMessageProperties[MsmqMessageProperty.Name] as MsmqMessageProperty;
			if (mqProp.DeliveryFailure == DeliveryFailure.ReachQueueTimeout ||
				mqProp.DeliveryFailure == DeliveryFailure.ReceiveTimeout)
			{
				Console.WriteLine("Opnieuw proberen...");
				_sync.NieuwLidBewaren(details, lidGedoe);
				Console.WriteLine("Verzonden!");
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void FunctiesUpdaten(Persoon persoon, string stamNummer, int werkJaar, IEnumerable<FunctieEnum> functies)
		{
			Console.WriteLine(
				"Functies updaten was mislukt: {3} AD{2} {0} {1}",
				persoon.VoorNaam, persoon.Naam, persoon.AdNummer, stamNummer);
			var mqProp =
				OperationContext.Current.IncomingMessageProperties[MsmqMessageProperty.Name] as MsmqMessageProperty;
			if (mqProp.DeliveryFailure == DeliveryFailure.ReachQueueTimeout ||
				mqProp.DeliveryFailure == DeliveryFailure.ReceiveTimeout)
			{
				Console.WriteLine("Opnieuw proberen...");
				_sync.FunctiesUpdaten(persoon, stamNummer, werkJaar, functies.ToArray());
				Console.WriteLine("Verzonden!");
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void LidTypeUpdaten(Persoon persoon, string stamNummer, int werkJaar, LidTypeEnum lidType)
		{
			Console.WriteLine(
				"Lidtype updaten was mislukt: {3} AD{2} {0} {1}", 
				persoon.VoorNaam, persoon.Naam, persoon.AdNummer, stamNummer);
			var mqProp =
				OperationContext.Current.IncomingMessageProperties[MsmqMessageProperty.Name] as MsmqMessageProperty;
			if (mqProp.DeliveryFailure == DeliveryFailure.ReachQueueTimeout ||
				mqProp.DeliveryFailure == DeliveryFailure.ReceiveTimeout)
			{
				Console.WriteLine("Opnieuw proberen...");
				_sync.LidTypeUpdaten(persoon, stamNummer, werkJaar, lidType);
				Console.WriteLine("Verzonden!");
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void AfdelingenUpdaten(Persoon persoon, string stamNummer, int werkJaar, IEnumerable<AfdelingEnum> afdelingen)
		{
			Console.WriteLine(
				"Afdelingen updaten was mislukt: {3} AD{2} {0} {1}",
				persoon.VoorNaam, persoon.Naam, persoon.AdNummer, stamNummer);
			var mqProp =
				OperationContext.Current.IncomingMessageProperties[MsmqMessageProperty.Name] as MsmqMessageProperty;
			if (mqProp.DeliveryFailure == DeliveryFailure.ReachQueueTimeout ||
				mqProp.DeliveryFailure == DeliveryFailure.ReceiveTimeout)
			{
				Console.WriteLine("Opnieuw proberen...");
				_sync.AfdelingenUpdaten(persoon, stamNummer, werkJaar, afdelingen.ToArray());
				Console.WriteLine("Verzonden!");
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DubbelpuntBestellen(int adNummer, string stamNummer, int werkJaar)
		{
			Console.WriteLine(
				"Dubbelpunt bestellen was mislukt: AD{0} {1} {2}",
				adNummer, stamNummer, werkJaar);
			var mqProp =
				OperationContext.Current.IncomingMessageProperties[MsmqMessageProperty.Name] as MsmqMessageProperty;
			if (mqProp.DeliveryFailure == DeliveryFailure.ReachQueueTimeout ||
				mqProp.DeliveryFailure == DeliveryFailure.ReceiveTimeout)
			{
				Console.WriteLine("Opnieuw proberen...");
				_sync.DubbelpuntBestellen(adNummer, stamNummer, werkJaar);
				Console.WriteLine("Verzonden!");
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void DubbelpuntBestellenNieuwePersoon(PersoonDetails details, string stamNummer, int werkJaar)
		{
			Console.WriteLine(
				"Dubbelpunt bestellen mislukt: {2} {0} {1}",
				details.Persoon.VoorNaam, details.Persoon.Naam, stamNummer);
			var mqProp =
				OperationContext.Current.IncomingMessageProperties[MsmqMessageProperty.Name] as MsmqMessageProperty;
			if (mqProp.DeliveryFailure == DeliveryFailure.ReachQueueTimeout ||
				mqProp.DeliveryFailure == DeliveryFailure.ReceiveTimeout)
			{
				Console.WriteLine("Opnieuw proberen...");
				_sync.DubbelpuntBestellenNieuwePersoon(details, stamNummer, werkJaar);
				Console.WriteLine("Verzonden!");
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void LoonVerliesVerzekeren(int adNummer, string stamNummer, int werkJaar)
		{
			Console.WriteLine(
				"Loonverlies verzekeren was mislukt: AD{0} {1} {2}",
				adNummer, stamNummer, werkJaar);
			var mqProp =
				OperationContext.Current.IncomingMessageProperties[MsmqMessageProperty.Name] as MsmqMessageProperty;
			if (mqProp.DeliveryFailure == DeliveryFailure.ReachQueueTimeout ||
				mqProp.DeliveryFailure == DeliveryFailure.ReceiveTimeout)
			{
				Console.WriteLine("Opnieuw proberen...");
				_sync.LoonVerliesVerzekeren(adNummer, stamNummer, werkJaar);
				Console.WriteLine("Verzonden!");
			}
		}

		[OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
		public void LoonVerliesVerzekerenAdOnbekend(PersoonDetails details, string stamNummer, int werkJaar)
		{
			Console.WriteLine(
				"Verzekering loonverlies mislukt: {2} {0} {1}",
				details.Persoon.VoorNaam, details.Persoon.Naam, stamNummer);
			var mqProp =
				OperationContext.Current.IncomingMessageProperties[MsmqMessageProperty.Name] as MsmqMessageProperty;
			if (mqProp.DeliveryFailure == DeliveryFailure.ReachQueueTimeout ||
				mqProp.DeliveryFailure == DeliveryFailure.ReceiveTimeout)
			{
				Console.WriteLine("Opnieuw proberen...");
				_sync.LoonVerliesVerzekerenAdOnbekend(details, stamNummer, werkJaar);
				Console.WriteLine("Verzonden!");
			}
		}
	}
}
