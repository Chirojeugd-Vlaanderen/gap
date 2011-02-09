using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Unity;

namespace Chiro.Kip.Services.UpdateService
{
	/// <summary>
	/// We gebruiken Unity voor IoC.  Maar unity weet niet welke constructor te kiezen voor
	/// de mapping IUpdateService -> UpdateServiceClient.  Daarom definieren we hier een
	/// constructor bij, stellen deze in als 'te gebruiken door unity', en roepen
	/// vervolgens de juiste originele (automatisch gegenereerde) constructor aan.
	/// </summary>
	public partial class UpdateServiceClient
	{
		[InjectionConstructor]
		public UpdateServiceClient(object dummy): this()
		{
		}
	}
}
