<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.IAdresBewerkenModel>" %>
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
<p>
    Voor een buitenlands adres kun je behalve een postnummer ook een postcode invullen:
    dat is bijvoorbeeld de lettercode die in Nederlandse adressen na het postnummer
    komt (bv. 1216 RA Hilversum).</p>
<p>
    <%=Html.LabelFor(mdl => mdl.Land) %>
    <%=Html.DropDownListFor(mdl => mdl.Land, new SelectList(Model.AlleLanden, "Naam", "Naam")) %>
</p>
<p>
    <%=Html.LabelFor(mdl => mdl.PostNr) %>
    <%=Html.EditorFor(mdl => mdl.PostNr)%>
    <%=Html.ValidationMessageFor(mdl => mdl.PostNr)%>
</p>
<p class="buitenland">
    <%=Html.LabelFor(mdl => mdl.PostCode) %>
    <%=Html.EditorFor(mdl => mdl.PostCode)%>
    <%=Html.ValidationMessageFor(mdl => mdl.PostCode)%>
</p>
<noscript>
    <input type="submit" name="action" value="Woonplaatsen ophalen" />
</noscript>
<p>
    <div class="ui-widget">
    <%=Html.LabelFor(mdl => mdl.Straat)%>
    <%=Html.EditorFor(mdl => mdl.Straat)%>
    <%=Html.ValidationMessageFor(mdl => mdl.Straat)%>
    </div>
</p>
<p>
    <%=Html.LabelFor(mdl => mdl.HuisNr)%>
    <%=Html.EditorFor(mdl => mdl.HuisNr)%>
    <%=Html.ValidationMessageFor(mdl => mdl.HuisNr)%>
</p>
<p>
    <%=Html.LabelFor(mdl => mdl.Bus)%>
    <%=Html.EditorFor(mdl => mdl.Bus)%>
    <%=Html.ValidationMessageFor(mdl => mdl.Bus)%>
</p>
<p class="binnenland">
    <%=Html.LabelFor(mdl => mdl.WoonPlaats)%>
    <%=Html.DropDownListFor(mdl => mdl.WoonPlaats, new SelectList(Model.BeschikbareWoonPlaatsen, "Naam", "Naam"))%>
    <%=Html.ValidationMessageFor(mdl => mdl.WoonPlaats)%>
</p>
<p class="buitenland">
    <%=Html.LabelFor(mdl => mdl.WoonPlaatsBuitenLand)%>
    <%=Html.EditorFor(mdl => mdl.WoonPlaatsBuitenLand)%>
    <%=Html.ValidationMessageFor(mdl => mdl.WoonPlaatsBuitenLand)%>
</p>
