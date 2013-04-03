/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
﻿// Met dank aan "Pro ASP.NET MVC Framework", het Apress-handboek van Steven Sanderson

using System.Collections.Generic;

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
