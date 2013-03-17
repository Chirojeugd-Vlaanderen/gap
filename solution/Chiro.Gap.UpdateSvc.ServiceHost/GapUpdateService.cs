// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2007-2012
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.ServiceProcess;

using Chiro.Cdf.Ioc;
using Chiro.Cdf.ServiceModel;
using Chiro.Gap.UpdateSvc.Service;

namespace Chiro.Gap.UpdateSvc.ServiceHost
{
    /// <summary>
    /// De service die updates doorgeeft tussen GAP en KipAdmin
    /// </summary>
    public partial class GapUpdateService : ServiceBase
    {
        private ServiceHost<UpdateService> _host = null;

        /// <summary>
        /// De standaardconstructor
        /// </summary>
        public GapUpdateService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Regelt wat er allemaal moet gebeuren wanneer de service gestart wordt
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1611:ElementParametersMustBeDocumented",
            Justification = "Moet niet gemarkeerd worden door ReSharper/StyleCop")]
        protected override void OnStart(string[] args)
        {
            Trace.WriteLine("GapUpdate service wordt gestart.");
            Factory.ContainerInit();

            _host = new ServiceHost<UpdateService>();
            _host.Open();
        }

        /// <summary>
        /// Regelt wat er allemaal moet gebeuren wanneer de service gestopt wordt
        /// </summary>
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