<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AfdelingsJaarModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Html.BeginForm("Bewerken", "Afdeling", new { groepID = Model.GroepID }))
       {%>
       
    <ul id="acties">
        <li><input type="submit" value="Bewaren" /></li>
        <li><input type="reset" value="  Reset  " /></li>
    </ul>
    
    <fieldset>
    
    <%=Html.LabelFor(mdl=> mdl.Afdeling.Naam)%>
    <%=Html.TextBoxFor(mdl => mdl.Afdeling.Naam, new { @readonly = "readonly", title = "Nu niet wijzigbaar", disabled = "disabled" })%><br />

    <%=Html.LabelFor(mdl => mdl.Afdeling.Afkorting)%>
    <%=Html.TextBoxFor(mdl => mdl.Afdeling.Afkorting, new { @readonly = "readonly", title = "Nu niet wijzigbaar", disabled = "disabled" })%><br />

    <%=Html.LabelFor(s => s.AfdelingsJaar.Geslacht)%>
    <%= Html.RadioButton("AfdelingsJaar.Geslacht", GeslachtsType.Gemengd, Model.AfdelingsJaar.Geslacht == GeslachtsType.Gemengd)%> Gemengd
    <%= Html.RadioButton("AfdelingsJaar.Geslacht", GeslachtsType.Man, Model.AfdelingsJaar.Geslacht == GeslachtsType.Man)%> Jongens
    <%= Html.RadioButton("AfdelingsJaar.Geslacht", GeslachtsType.Vrouw, Model.AfdelingsJaar.Geslacht == GeslachtsType.Vrouw)%> Meisjes
    <%=Html.ValidationMessageFor(s => s.AfdelingsJaar.Geslacht)%><br />
   
    <%=Html.LabelFor(mdl => mdl.AfdelingsJaar.GeboorteJaarVan)%>
    <%=Html.EditorFor(mdl => mdl.AfdelingsJaar.GeboorteJaarVan)%>
    <%=Html.ValidationMessageFor(mdl => mdl.AfdelingsJaar.GeboorteJaarVan)%><br />

    <%=Html.LabelFor(mdl => mdl.AfdelingsJaar.GeboorteJaarTot)%>
    <%=Html.EditorFor(mdl => mdl.AfdelingsJaar.GeboorteJaarTot)%>
    <%=Html.ValidationMessageFor(mdl => mdl.AfdelingsJaar.GeboorteJaarTot)%><br />
       
    <% var values = from OfficieleAfdeling oa in Model.OfficieleAfdelingen
                    select new { value = oa.ID, text = oa.Naam.ToString() }; 
    %>
    
    <%=Html.LabelFor(mdl=>mdl.AfdelingsJaar.OfficieleAfdelingID) %>
    <%=Html.DropDownListFor(mdl => mdl.AfdelingsJaar.OfficieleAfdelingID, new SelectList(values, "value", "text"))%><br />
    
    <%=Html.HiddenFor(mdl=>mdl.AfdelingsJaar.AfdelingID) %>
    <%=Html.HiddenFor(mdl=>mdl.AfdelingsJaar.AfdelingsJaarID) %>
    <%=Html.HiddenFor(mdl=>mdl.AfdelingsJaar.VersieString) %>

    </fieldset>
    
    De standaard geboortejaren voor het jaar 2009-2010 zijn:<br />
	//DIT IS VERKEERD, want in januari 2010 mogen kleuters die nu in het derde kleuterklasje zitten eigenlijk geen lid worden (hoofdzaak is eerste 
	//leerjaar. Dus hier moet eigenlijk het huidige werkjaar gebruikt worden (wat van januari tot augustus het vorige echte jaar zal zijn)
    <% int jongemini = DateTime.Today.Year - 7; //6 worden in het jaar dat je naar het eerste leerjaar gaat
		int oudemini = DateTime.Today.Year - 8;
	%>
	<table>
	<tr><td>Ribbels</td>    <td><%=oudemini %>-<%=jongemini %></td></tr>
    <tr><td>Speelclub</td>  <td><%=oudemini-2 %>-<%=jongemini-2 %></td></tr>
    <tr><td>Rakkers</td>    <td><%=oudemini-4 %>-<%=jongemini-4 %></td></tr>
    <tr><td>Toppers</td>    <td><%=oudemini-6 %>-<%=jongemini-6 %></td></tr>
    <tr><td>Kerels</td>     <td><%=oudemini-8 %>-<%=jongemini-8 %></td></tr>
    <tr><td>Aspiranten</td> <td><%=oudemini-10 %>-<%=jongemini-10 %></td></tr>
	</table>
    
    <%} %>
</asp:Content>
