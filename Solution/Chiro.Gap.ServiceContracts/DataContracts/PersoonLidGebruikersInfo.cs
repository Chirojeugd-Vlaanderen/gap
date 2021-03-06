/*
 * Copyright 2014 Johan Vervloet. See the NOTICE file at the 
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

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Chiro.Gap.ServiceContracts.DataContracts
{
	/// <summary>
	/// DataContract waarin een gelieerde persoon, zijn/haar LidInfo, zijn/haar adressen, 
	/// zijn/haar communicatievormen en gebruikersinfo aan elkaar koppeld zijn
	/// </summary>
	[DataContract]
	public class PersoonLidGebruikersInfo: PersoonLidInfo
	{
	    /// <summary>
	    /// Info over eventueel gebruikersrecht van deze gelieerde persoon op zijn eigen groep.
	    /// (null als er geen gebruikersrecht is)
	    /// </summary>
        [DataMember]
        public GebruikersInfo GebruikersInfo { get; set; }
	}
}
