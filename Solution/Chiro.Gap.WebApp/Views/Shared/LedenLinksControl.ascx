<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.LedenLinksModel>" %>
<%@ Import Namespace="System.Linq" %>
<%
/*
 * Copyright 2008-2013 the GAP developers. See the NOTICE file at the 
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
<ul>
    <% foreach (var info in Model.Leden)
       { %>
    <li>
        <%=Html.PersoonsLink(info.PersoonDetail.GelieerdePersoonID, info.PersoonDetail.VoorNaam, info.PersoonDetail.Naam)%>
    </li>
    <%
        }
       if (Model.Leden.Count() < Model.TotaalAantal)
       {
    %>
    <li><a href="<%=Model.VolledigeLijstUrl%>">(alle
        <%=Model.TotaalAantal %>
        personen)</a></li>
    <%
        }
    %>
</ul>
