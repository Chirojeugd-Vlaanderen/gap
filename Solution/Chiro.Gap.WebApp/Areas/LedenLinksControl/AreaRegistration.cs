using System.Web.Mvc;

namespace Chiro.Gap.WebApp.Areas.LedenLinksControl
{
	public class LedenLinksControlAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "LedenLinksControl";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"LedenLinksControl_default",
				"LedenLinksControl/{controller}/{action}/{id}",
				new { action = "Index", id = "" }
			);
		}
	}
}
