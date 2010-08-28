// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2010
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System;

using Chiro.Cdf.Ioc;
using Chiro.Gap.ServiceContracts.Mappers;

namespace Chiro.Gap.Services
{
	/// <summary>
	/// Klasse die op de scope van de service een aantal zaken regelt
	/// </summary>
	public class Global : System.Web.HttpApplication
	{
		/// <summary>
		/// Acties die uitgevoerd moeten worden wanneer de applicatie start
		/// </summary>
		protected void Application_Start(object sender, EventArgs e)
		{
			Factory.ContainerInit();
			MappingHelper.MappingsDefinieren();		// mappings voor servicecontracts
			Sync.MappingHelper.MappingsDefinieren();	// mappings voor sync
		}

		/// <summary>
		/// Acties die uitgevoerd moeten worden wanneer een gebruiker de applicatie opent
		/// </summary>
		protected void Session_Start(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Acties die uitgevoerd moeten worden VOOR elke gebruikersrequest 
		/// </summary>
		protected void Application_BeginRequest(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Acties die uitgevoerd moeten worden op het moment dat de rechten van de gebruiker
		/// nagekeken worden
		/// </summary>
		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Acties die uitgevoerd moeten worden op het moment dat er een niet-opgevangen fout optreedt
		/// </summary>
		protected void Application_Error(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Acties die uitgevoerd moeten worden wanneer een gebruiker de applicatie afsluit of lang genoeg niet gebruikt
		/// </summary>
		protected void Session_End(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Acties die uitgevoerd moeten worden wanneer de applicatie gestopt wordt
		/// </summary>
		protected void Application_End(object sender, EventArgs e)
		{
			Factory.Dispose();
		}
	}
}