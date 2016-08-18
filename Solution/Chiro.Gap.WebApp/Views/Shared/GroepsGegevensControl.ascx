<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MasterViewModel>" %>
<%
/*
 * Copyright 2008-2013, 2016 the GAP developers. See the NOTICE file at the 
 * top-level directory of this distribution, and at
 * https://gapwiki.chiro.be/copyright
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
%>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<h1>
	<%=Html.Encode(ViewData.Model.GroepsNaam) %>  
<% if (ViewData.Model.HuidigWerkJaar > 0)
   {%>
	<%=ViewData.Model.HuidigWerkJaar%>-<%=ViewData.Model.HuidigWerkJaar + 1%>
<%
   }
%>
</h1>
<%  if (ViewData.Model.Plaats != "geen" && ViewData.Model.Plaats != "nvt")
	{ %>
<p>
	<%=Html.Encode(ViewData.Model.Plaats)%>
	<br />
	Stamnummer:
	<%=Html.Encode(ViewData.Model.StamNummer)%>
</p>
<%  }
	else
	{ %>
<p>
	Stamnummer:
	<%=Html.Encode(ViewData.Model.StamNummer)%></p>
<%  } %>