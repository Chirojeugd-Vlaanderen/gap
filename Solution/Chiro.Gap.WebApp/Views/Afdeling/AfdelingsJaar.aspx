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
    
    <label for="Afdeling_Naam">Naam</label> 
    <%=Html.TextBox("HuidigeAfdeling.Naam", Model.HuidigeAfdeling.Naam, new { @readonly = "readonly", title = "Nu niet wijzigbaar", disabled = "disabled" } )%><br />
    
    <label for="Afdeling_Afkorting">Afkorting</label> 
    <%=Html.TextBox("HuidigeAfdeling.Afkorting", Model.HuidigeAfdeling.Afkorting, new { @readonly = "readonly", title = "Nu niet wijzigbaar", disabled = "disabled" })%><br />
        
    <label for="AfdelingsJaar_GeboorteJaarVan">Van</label> 
    <%=Html.TextBox("HuidigAfdelingsJaar.GeboorteJaarVan")%><br />
    
    <label for="AfdelingsJaar_GeboorteJaarTot">Tot</label> 
    <%=Html.TextBox("HuidigAfdelingsJaar.GeboorteJaarTot")%><br />
    
    <% var values = from OfficieleAfdeling oa in Model.OfficieleAfdelingenLijst
                    select new { value = oa.ID, text = oa.Naam.ToString() }; 
    %>
    <label for="AfdelingsJaar_OfficieleAfdeling">Offici&euml;le afdeling</label>
    <%=Html.DropDownList("OfficieleAfdelingID", new SelectList(values, "value", "text"))%>

    </fieldset>
    
    <%} %>
</asp:Content>
