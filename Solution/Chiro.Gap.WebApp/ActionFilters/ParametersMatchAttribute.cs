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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Diagnostics;

// Code overgenomen en verbeterd van http://blog.abodit.com/2010/02/asp-net-mvc-ambiguous-match/
// Opgelet: in onze coding style gebruiken we automatische model binding, met als gevolg dat
// dit niet direct gebruikt kan worden voor 'POST'-methods.
//
// Een workaround zou zijn: het model decoreren met een FormValueAttribute, en de daarmee gedecoreerde
// variabelen gewoon negeren.
//
// Voor overloads van post-methods, kan voorlopig het ParameterAccepterenAttribute gebruikt worden
// TODO (#1055): nog wat opkuis in de attributen ParameterAccepteren en ParametersMatch

namespace Chiro.Gap.WebApp.ActionFilters
{
	/// <summary>
	/// This attribute can be placed on a parameter of an action method that should be present on the URL in route data
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class RouteValueAttribute : Attribute
	{
		public RouteValueAttribute()
		{
		}
	}

	/// <summary>
	/// This attribute can be placed on a parameter of an action method that should be present in FormData
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class FormValueAttribute : Attribute
	{
		public FormValueAttribute()
		{
		}
	}

	/// <summary>
	/// This attribute can be placed on a parameter of an action method that should be present in a query string
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public sealed class QueryStringValueAttribute : Attribute
	{
	}

	/// <summary>
	/// Parameters Match Attribute allows you to specify that an action is only valid
	/// if it has the right number of parameters marked [RouteValue] or [FormValue] that match with the form data or route data
	/// </summary>
	/// <remarks>
	/// This attribute allows you to have two actions with the SAME name distinguished by the values they accept according to the
	/// name of those values.  Does NOT handle complex types and bindings yet but could be easily adapted to do so.
	/// </remarks>
	[AttributeUsage
	 (AttributeTargets.Method, AllowMultiple = false, Inherited =
	  true)]
	public sealed class ParametersMatchAttribute : ActionMethodSelectorAttribute
	{
	    public override bool IsValidForRequest(ControllerContext
							controllerContext,
							MethodInfo methodInfo)
		{
			// The Route values
			List<string> requestRouteValuesKeys =
			  controllerContext.RouteData.Values.Where(v => !(v.Key == "controller"
								       || v.Key == "action"
								       || v.Key ==
								       "area")).Select(rv => rv.Key).ToList();

			// The Form values
			var form = controllerContext.HttpContext.Request.Form;
			List<string> requestFormValuesKeys = form.AllKeys.ToList();

			// The query string values
			List<string> requestQueryStringValueKeys = controllerContext.HttpContext.Request.QueryString.AllKeys.ToList();

			// The parameters this method expects
			var parameters = methodInfo.GetParameters();

			// Parameters from the method that we haven’t matched up against yet
			var parametersNotMatched = parameters.ToList();

			// each parameter of the method can be marked as a [RouteValue] or [FormValue] or both or nothing
			foreach (var param in parameters)
			{
				string name = param.Name;

				bool isRouteParam = param.GetCustomAttributes(true).Any(a => a is
											  RouteValueAttribute);
				bool isFormParam = param.GetCustomAttributes(true).Any(a => a is
											 FormValueAttribute);
				bool isQueryStringParam = param.GetCustomAttributes(true).Any(a => a is
							 QueryStringValueAttribute);

				if (isRouteParam && requestRouteValuesKeys.Contains(name))
				{
					// Route value matches parameter
					requestRouteValuesKeys.Remove(name);
					parametersNotMatched.Remove(param);
				}
				else if (isFormParam && requestFormValuesKeys.Contains(name))
				{
					// Form value matches method parameter
					requestFormValuesKeys.Remove(name);
					parametersNotMatched.Remove(param);
				}
				else if (isQueryStringParam && requestQueryStringValueKeys.Contains(name))
				{
					// Query string value matches method parameter
					requestQueryStringValueKeys.Remove(name);
					parametersNotMatched.Remove(param);
				}

				else
				{
					// methodInfo parameter does not match a route value or a form value
					Debug.WriteLine(methodInfo + " failed to match " + param +
							 " against either a RouteValue or a FormValue");
					return false;
				}
			}

			// Having removed all the parameters of the method that are matched by either a route value or a form value
			// we are now left with all the parameters that do not match and all the route and form values that were not used

			if (parametersNotMatched.Count > 0)
			{
				Debug.WriteLine(methodInfo +
						 " – FAIL: has parameters left over not matched by route or form values");
				return false;
			}

			if (requestRouteValuesKeys.Count > 0)
			{
				Debug.WriteLine(methodInfo +
						 " – FAIL: Request has route values left that aren’t consumed");
				return false;
			}

			if (requestFormValuesKeys.Count > 1)
			{
				Debug.WriteLine(methodInfo + " – FAIL : unmatched form values " +
						 string.Join(", ",
							      requestFormValuesKeys.ToArray()));
				return false;
			}

			if (requestQueryStringValueKeys.Count > 0)
			{
				Debug.WriteLine(methodInfo + " – FAIL : unmatched query string form values " +
						 string.Join(", ",
							      requestQueryStringValueKeys.ToArray()));
				return false;
			}

			Debug.WriteLine(methodInfo + " – PASS ");
			return true;
		}
	}
}