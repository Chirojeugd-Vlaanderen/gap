// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.ComponentModel;
using System.ServiceProcess;

namespace Chiro.Gap.UpdateSvc.ServiceHost
{
    /// <summary>
    /// Class die Updateservice helpt installeren
    /// </summary>
    [RunInstaller(true)]
	public partial class Installer1 : System.Configuration.Install.Installer
	{
        /// <summary>
        /// Constructor. Zorgt voor de nodige configuratie.
        /// </summary>
        public Installer1()
		{
			InitializeComponent();

			var processInstaller = new ServiceProcessInstaller();
			var serviceInstaller = new ServiceInstaller();

			processInstaller.Account = ServiceAccount.User;
			processInstaller.Username = @"KIPDORP\GapService";
			serviceInstaller.StartType = ServiceStartMode.Automatic;
			serviceInstaller.ServiceName = "GapUpdate";

			Installers.Add(serviceInstaller);
			Installers.Add(processInstaller);
		}
	}
}
