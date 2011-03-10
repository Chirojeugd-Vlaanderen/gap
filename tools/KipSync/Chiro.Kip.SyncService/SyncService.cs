using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Chiro.Cdf.Ioc;
using Chiro.Cdf.ServiceModel;
using Chiro.Kip.Services;

namespace Chiro.Kip.SyncService
{
	public partial class SyncService : ServiceBase
	{
		private ServiceHost<SyncPersoonService> _host = null;

		public SyncService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
		    Factory.ContainerInit();
			Trace.WriteLine("KipSync v. 2010-12-06 wordt gestart");

			_host = new ServiceHost<SyncPersoonService>();

			_host.Open();
		}

		protected override void OnStop()
		{
			if (_host != null)
			{
				Trace.WriteLine("KipSync v. 2010-12-06 wordt gestopt");

				_host.Close();
				_host = null;
			}
		}
	}
}
