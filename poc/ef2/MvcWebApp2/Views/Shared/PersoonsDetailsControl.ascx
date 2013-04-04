<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<GelieerdePersoon>" %>
<%
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
%>
<%@ Import Namespace="Cg2.Orm" %>


<h3>Algemeen</h3>

<table>
<tr><td>Ad-nummer:</td><td><%=ViewData.Model.Persoon.AdNummer %></td></tr>
<tr><td>Familienaam</td><td><%=Html.Encode(ViewData.Model.Persoon.Naam) %></td></tr>
<tr><td>Voornaam</td><td><%=Html.Encode(ViewData.Model.Persoon.VoorNaam) %></td></tr>
<tr><td>Geboortedatum</td><td><%=ViewData.Model.Persoon.GeboorteDatum == null ? "?" : ((DateTime)ViewData.Model.Persoon.GeboorteDatum).ToString("d") %></td></tr>
<tr><td>Geslacht</td><td><%=ViewData.Model.Persoon.Geslacht.ToString() %></td></tr>
<tr><td>Chiroleeftijd</td><td><%=ViewData.Model.ChiroLeefTijd %></td></tr>
</table>

<h3>Adressen</h3>

<ul>
<% foreach (PersoonsAdres pa in ViewData.Model.PersoonsAdres)
   { %>
   <li>
        <%=Html.Encode(String.Format("{0} {1}", pa.Adres.Straat.Naam, pa.Adres.HuisNr)) %>,
        <%=Html.Encode(String.Format("{0} {1} {2}", pa.Adres.Straat.PostNr, pa.Adres.PostCode, pa.Adres.Subgemeente.Naam)) %>
        <%= pa.IsStandaard ? "(standaardadres)" : "" %>
    </li>
<%} %>
</ul>


<h3>Communicatie</h3>

<ul>
<% foreach (CommunicatieVorm cv in ViewData.Model.Communicatie)
   { %>
   <li>
        <%=cv.Type.ToString() %>:
        <%=Html.Encode(cv.Nummer) %>
        <%=cv.Voorkeur ? "(voorkeur)" : "" %>
    </li>
<%} %>
</ul>

