﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
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
			Trace.WriteLine("KipSync wordt gestart");

			_host = new ServiceHost<SyncPersoonService>();

			_host.Open();
		}

		protected override void OnStop()
		{
			if (_host != null)
			{
				Trace.WriteLine("KipSync wordt gestopt");

				_host.Close();
				_host = null;
			}
		}
	}
}