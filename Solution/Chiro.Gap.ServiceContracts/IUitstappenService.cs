using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.ServiceContracts
{
	/// <summary>
	/// Service voor beheer van uitstappen en bivakken
	/// </summary>
	[ServiceContract]
	public interface IUitstappenService
	{
		/// <summary>
		/// Bewaart een uitstap aan voor de groep met gegeven <paramref name="groepID"/>
		/// </summary>
		/// <param name="groepID">ID van de groep horende bij de uitstap.
		/// Is eigenlijk enkel relevant als het om een nieuwe uitstap gaat.</param>
		/// <param name="detail">Details over de uitstap.  Als <c>uitstap.ID</c> <c>0</c> is,
		/// dan wordt een nieuwe uitstap gemaakt.  Anders wordt de bestaande overschreven.</param>
		/// <returns>ID van de uitstap</returns>
		[OperationContract]
		int Bewaren(int groepID, UitstapDetail detail);
	}
}
