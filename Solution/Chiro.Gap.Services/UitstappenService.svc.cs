using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using Chiro.Gap.ServiceContracts;
using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.Services
{
	public class UitstappenService : IUitstappenService
	{
		/// <summary>
		/// Maakt een nieuwe uitstap aan voor de groep met gegeven <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">ID van de groep waarvoor een uitstap moet worden aangemaakt</param>
		/// <param name="uitstap">Details over de nieuwe uitstap</param>
		/// <returns>ID van de nieuw gemaakte uitstap</returns>
		public int Nieuw(int groepID, UitstapDetail uitstap)
		{
			throw new NotImplementedException();
		}
	}
}
