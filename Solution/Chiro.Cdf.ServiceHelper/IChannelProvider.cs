/*
 * Copyright 2008 Capgemini - Accelerated Delivery Framework - http://www.be.capgemini.com/
 * Copyright 2008-2014 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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

namespace Chiro.Cdf.ServiceHelper
{
	/// <summary>
	/// Defines methods to obtain configuration-based instances of interface implementations.
	/// </summary>
	public interface IChannelProvider
	{
		/// <summary>
		/// Lever een channel op voor het servicecontract <typeparamref name="I"/>.
		/// </summary>
		/// <typeparam name="I">Servicecontract</typeparam>
		/// <returns>Channel voor het gevraagde service-contract, of <c>null</c> als er geen implementatie
		/// beschikbaar is.</returns>
		I GetChannel<I>() where I : class;

        /// <summary>
        /// Lever een channel op voor het servicecontract <typeparamref name="I"/>.
        /// </summary>
        /// <typeparam name="I">Servicecontract</typeparam>
        /// <param name="instanceName">Instance name, wat van belang is als er meerdere endpoints gedefinieerd zijn
        /// in de config file.</param>
        /// <returns>Channel voor het gevraagde service-contract, of <c>null</c> als er geen implementatie
        /// beschikbaar is.</returns>
        I GetChannel<I>(string instanceName) where I : class;

        /// <summary>
        /// Lever een channel op voor het servicecontract <typeparamref name="I"/>.
        /// </summary>
        /// <typeparam name="I">Servicecontract</typeparam>
        /// <param name="channel">Hier wordt het channel bewaard.</param>
        /// <returns><c>true</c> als een channel gevonden is, anders <c>false</c>.</returns>
        bool TryGetChannel<I>(out I channel) where I : class;
	}
}
