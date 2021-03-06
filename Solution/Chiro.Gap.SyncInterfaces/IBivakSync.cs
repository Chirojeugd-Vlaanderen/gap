/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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

using Chiro.Gap.Poco.Model;

namespace Chiro.Gap.SyncInterfaces
{
	/// <summary>
	/// Interface voor synchronisatie van bivakaangifte naar Kipadmin
	/// </summary>
	public interface IBivakSync
	{
		/// <summary>
		/// Geeft de <paramref name="uitstap"/> door aan Kipadmin als bivakaangifte.
		/// </summary>
		/// <param name="uitstap">Te bewaren uitstap</param>
		void Bewaren(Uitstap uitstap);

		/// <summary>
		/// Verwijdert uitstap met gegeven <paramref name="uitstapID"/> uit Kipadmin.
		/// </summary>
		/// <param name="uitstapID">Te verwijderen</param>
		void Verwijderen(int uitstapID);
	}
}
