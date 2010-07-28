// Met dank aan "Pro ASP.NET MVC Framework", het Apress-handboek van Steven Sanderson

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Collections.Specialized;

namespace MvcWebApp2.Tests
{
    /// <summary>
    /// Hulpklasse die een HttpContext mockt, om controllers mee te testen.
    /// Het project bevat een reference naar Moq.dll (http://code.google.com/p/moq/).
    /// </summary>
    public class MockContext
    {
        public Mock<HttpContextBase> HttpContext { get; private set; }
        public Mock<HttpRequestBase> Request { get; private set; }
        public Mock<HttpResponseBase> Response { get; private set; }
        public RouteData RouteData { get; private set; }

        public MockContext(Controller controller)
        {
            // Definieer alle 'gewone' contextobjecten en de relaties ertussen
            HttpContext = new Mock<HttpContextBase>();
            Request = new Mock<HttpRequestBase>();
            Response = new Mock<HttpResponseBase>();
            HttpContext.Setup(x => x.Request).Returns(Request.Object);
            HttpContext.Setup(x => x.Response).Returns(Response.Object);
            HttpContext.Setup(x => x.Session).Returns(new FakeSessionState());
            Request.Setup(x => x.Cookies).Returns(new HttpCookieCollection());
            Response.Setup(x => x.Cookies).Returns(new HttpCookieCollection());
            Request.Setup(x => x.QueryString).Returns(new NameValueCollection());
            Request.Setup(x => x.Form).Returns(new NameValueCollection());

            // Geef de mockcontext door aan de controller
            var rc = new RequestContext(HttpContext.Object, new RouteData());
            controller.ControllerContext = new ControllerContext(rc, controller);
        }

        // Gebruik een gesimuleerde HttpSessionBase (is moeilijk te mocken met Moq)
        private class FakeSessionState : HttpSessionStateBase
        {
        	readonly Dictionary<string, object> _items = new Dictionary<string, object>();
            public override object this[string name]
            {
                get
                {
                    return _items.ContainsKey(name) ? _items[name] : null;
                }
                set
                {
                    _items[name] = value;
                }
            }
        }
    }
}
