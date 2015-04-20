<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.UitstapInschrijfModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<asp:Content runat="server" ID="Head" ContentPlaceHolderID="head"></asp:Content>
<asp:Content runat="server" ID="Main" ContentPlaceHolderID="MainContent">
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
<%using (Html.BeginForm())
  {%>

  <ul id="acties">
    <li><input type="submit" value="Bewaren" /></li>
  </ul>

  <%=Html.LabelFor(mdl => mdl.GeselecteerdeUitstapID) %>:
  <%=Html.DropDownListFor(mdl => mdl.GeselecteerdeUitstapID, new SelectList(Model.Uitstappen, "ID", "Naam")) %> <br />

  <%=Html.CheckBoxFor(mdl => mdl.LogistiekDeelnemer) %> 
  <%=Html.LabelFor(mdl => mdl.LogistiekDeelnemer) %>

  <ul>
  <%foreach (var p in Model.GelieerdePersonen)
    { %>
    <li>
        <%=Html.Hidden("GelieerdePersoonIDs", p.GelieerdePersoonID) %>
        <%=Html.PersoonsLink(p.GelieerdePersoonID, p.VoorNaam, p.Naam)%>
    </li>
  <%
    }%>
  </ul>
<%
  }%>

</asp:Content>
