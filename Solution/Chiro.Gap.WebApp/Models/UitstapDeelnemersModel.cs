using System.Collections.Generic;

using Chiro.Gap.ServiceContracts.DataContracts;

namespace Chiro.Gap.WebApp.Models
{
	public class UitstapDeelnemersModel: UitstapModel
	{
		public IEnumerable<DeelnemerDetail> Deelnemers { get; set; }
	}
}