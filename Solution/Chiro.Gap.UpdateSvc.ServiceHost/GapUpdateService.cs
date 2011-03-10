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
using Chiro.Gap.UpdateSvc.Service;

namespace Chiro.Gap.UpdateSvc.ServiceHost
{
	public partial class GapUpdateService : ServiceBase
	{
		private ServiceHost<UpdateService> _host = null;

		public GapUpdateService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			Trace.WriteLine("GapUpdate service wordt gestart.");
            Factory.ContainerInit();

			_host = new ServiceHost<UpdateService>();
			_host.Open();
		}

		protected override void OnStop()
		{
			if (_host != null)
			{
				Trace.WriteLine("GapUpdate service wordt gestopt.");

				_host.Close();
				_host = null;
			}
		}
	}
}
