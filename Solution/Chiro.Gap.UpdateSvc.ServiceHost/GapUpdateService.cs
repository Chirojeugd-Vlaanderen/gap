// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Diagnostics;
using System.ServiceProcess;

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
