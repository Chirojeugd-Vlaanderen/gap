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

namespace Chiro.Gap.Domain
{
	/// <summary>
	/// Interface voor communicatie, die zowel aan UI-kant als aan businesskant gebruikt kan worden.
	/// </summary>
	public interface ICommunicatie
	{
		/// <summary>
		/// Het telefoonnummer, e-mailadres,...
		/// </summary>
		string Nummer { get; set; }

		/// <summary>
		/// Regular expression waaraan het nummer moet voldoen
		/// </summary>
		string CommunicatieTypeValidatie { get; set; }
	}
}
