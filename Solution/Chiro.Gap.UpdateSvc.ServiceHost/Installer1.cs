﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;

namespace Chiro.Gap.UpdateSvc.ServiceHost
{
	[RunInstaller(true)]
	public partial class Installer1 : System.Configuration.Install.Installer
	{
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