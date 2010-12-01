using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;


namespace Chiro.Kip.SyncService
{
	[RunInstaller(true)]
	public partial class Installer1 : Installer
	{
		public Installer1()
		{
			InitializeComponent();

			var processInstaller = new ServiceProcessInstaller();
			var serviceInstaller = new ServiceInstaller();

			processInstaller.Account = ServiceAccount.User;
			processInstaller.Username = @"KIPDORP\Kipsync";
			serviceInstaller.StartType = ServiceStartMode.Automatic;
			serviceInstaller.ServiceName = "Kipsync";

			Installers.Add(serviceInstaller);
			Installers.Add(processInstaller);
		}
	}
}
