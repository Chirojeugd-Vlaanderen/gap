﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.IAdresBewerkenModel>" %>
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
    <tr id="postCode" hidden>
        <td><%=Html.LabelFor(mdl => mdl.PostCode) %></td>
        <td><%=Html.EditorFor(mdl => mdl.PostCode)%>
        <%=Html.ValidationMessageFor(mdl => mdl.PostCode)%></td>
    </tr>
    <tr>
        <td><%=Html.LabelFor(mdl => mdl.PostNr) %></td>
        <td><%=Html.EditorFor(mdl => mdl.PostNr)%> <!--<input id="PostNr" class="text-box single-line" type="text" required></input> -->
        <%=Html.ValidationMessageFor(mdl => mdl.PostNr)%></td>
    </tr>
    <tr>
        <td><%=Html.LabelFor(mdl => mdl.Straat)%></td>
        <td><%=Html.EditorFor(mdl => mdl.Straat)%><!--<input id="Straat" class="text-box single-line" type="text" required></input>-->
        <%=Html.ValidationMessageFor(mdl => mdl.Straat)%></td>
    </tr>   
    
    <tr>
        <td><%=Html.LabelFor(mdl => mdl.HuisNr)%></td>
        <td><%=Html.EditorFor(mdl => mdl.HuisNr)%><!--<input id="HuisNr" class="text-box single-line" type="text" required></input>-->
        <%=Html.ValidationMessageFor(mdl => mdl.HuisNr)%></td>
    </tr>
    <tr>
        <td><%=Html.LabelFor(mdl => mdl.Bus)%></td>
        <td><%=Html.EditorFor(mdl => mdl.Bus)%>
        <%=Html.ValidationMessageFor(mdl => mdl.Bus)%></td>
    </tr>

    
        <!--<input type="submit" name="action" value="Woonplaatsen ophalen" />-->
 
    <tr id="woonplaatsBinnenland">
        <td><%=Html.LabelFor(mdl => mdl.WoonPlaats)%></td>
        <td><%=Html.DropDownListFor(mdl => mdl.WoonPlaats, new SelectList(Model.BeschikbareWoonPlaatsen, "Naam", "Naam"))%>
        <%=Html.ValidationMessageFor(mdl => mdl.WoonPlaats)%></td>
    </tr>
    <tr id="woonplaatsBuitenland" hidden>
        <td><%=Html.LabelFor(mdl => mdl.WoonPlaatsBuitenLand)%></td>
        <td><%=Html.EditorFor(mdl => mdl.WoonPlaatsBuitenLand)%>
        <%=Html.ValidationMessageFor(mdl => mdl.WoonPlaatsBuitenLand)%></td>    
    </tr>