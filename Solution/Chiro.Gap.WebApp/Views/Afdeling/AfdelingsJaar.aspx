<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AfdelingInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <% using (Html.BeginForm())
       {%>
       
    <ul id="acties">
        <li><input type="submit" value="Bewaren"/></li>
        <li><input type="reset" value="  Reset  " /></li>
    </ul>
    
    <fieldset>
    
    <%=Html.LabelFor(mdl=> mdl.HuidigeAfdeling.Naam)%>
    <%=Html.TextBoxFor(mdl => mdl.HuidigeAfdeling.Naam, new { @readonly = "readonly", title = "Nu niet wijzigbaar", disabled = "disabled" } )%><br />

    <%=Html.LabelFor(mdl=> mdl.HuidigeAfdeling.Afkorting)%>
    <%=Html.TextBoxFor(mdl => mdl.HuidigeAfdeling.Afkorting, new { @readonly = "readonly", title = "Nu niet wijzigbaar", disabled = "disabled" } )%><br />

    <%=Html.LabelFor(s => s.HuidigAfdelingsJaar.Geslacht)%>
    <%= Html.RadioButton("HuidigAfdelingsJaar.Geslacht", GeslachtsType.Gemengd, Model.HuidigAfdelingsJaar.Geslacht == GeslachtsType.Gemengd)%> Gemengd
    <%= Html.RadioButton("HuidigAfdelingsJaar.Geslacht", GeslachtsType.Man, Model.HuidigAfdelingsJaar.Geslacht == GeslachtsType.Man)%> Jongens
    <%= Html.RadioButton("HuidigAfdelingsJaar.Geslacht", GeslachtsType.Vrouw, Model.HuidigAfdelingsJaar.Geslacht == GeslachtsType.Vrouw)%> Meisjes
    <%=Html.ValidationMessageFor(s => s.HuidigAfdelingsJaar.Geslacht)%><br />
   
    <%=Html.LabelFor(mdl => mdl.HuidigAfdelingsJaar.GeboorteJaarVan) %>
    <%=Html.EditorFor(mdl => mdl.HuidigAfdelingsJaar.GeboorteJaarVan) %>
    <%=Html.ValidationMessageFor(mdl => mdl.HuidigAfdelingsJaar.GeboorteJaarVan) %><br />

    <%=Html.LabelFor(mdl => mdl.HuidigAfdelingsJaar.GeboorteJaarTot) %>
    <%=Html.EditorFor(mdl => mdl.HuidigAfdelingsJaar.GeboorteJaarTot) %>
    <%=Html.ValidationMessageFor(mdl => mdl.HuidigAfdelingsJaar.GeboorteJaarTot) %><br />
    
    <label for="Afdeling_Afkorting">Afkorting</label> 
    <%=Html.TextBox("HuidigeAfdeling.Afkorting", Model.HuidigeAfdeling.Afkorting, new { @readonly = "readonly", title = "Nu niet wijzigbaar", disabled = "disabled" })%><br />
        
    <label for="AfdelingsJaar_GeboorteJaarVan">Geboren van </label> 
    <%=Html.TextBox("HuidigAfdelingsJaar.GeboorteJaarVan")%><br />
    
    <label for="AfdelingsJaar_GeboorteJaarTot"> tot </label> 
    <%=Html.TextBox("HuidigAfdelingsJaar.GeboorteJaarTot")%><br />
    
    <% var values = from OfficieleAfdeling oa in Model.OfficieleAfdelingenLijst
                    select new { value = oa.ID, text = oa.Naam.ToString() }; 
    %>
    
    <%=Html.LabelFor(mdl=>mdl.OfficieleAfdelingID) %>
    <%=Html.DropDownListFor(mdl => mdl.OfficieleAfdelingID, new SelectList(values, "value", "text"))%><br />

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
