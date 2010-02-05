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
   
    <%=Html.LabelFor(mdl => mdl.HuidigAfdelingsJaar.GeboorteJaarVan) %>
    <%=Html.EditorFor(mdl => mdl.HuidigAfdelingsJaar.GeboorteJaarVan) %>
    <%=Html.ValidationMessageFor(mdl => mdl.HuidigAfdelingsJaar.GeboorteJaarVan) %><br />

    <%=Html.LabelFor(mdl => mdl.HuidigAfdelingsJaar.GeboorteJaarTot) %>
    <%=Html.EditorFor(mdl => mdl.HuidigAfdelingsJaar.GeboorteJaarTot) %>
    <%=Html.ValidationMessageFor(mdl => mdl.HuidigAfdelingsJaar.GeboorteJaarTot) %><br />
    
    <% var values = from OfficieleAfdeling oa in Model.OfficieleAfdelingenLijst
                    select new { value = oa.ID, text = oa.Naam.ToString() }; 
    %>
    
    <%=Html.LabelFor(mdl=>mdl.OfficieleAfdelingID) %>
    <%=Html.DropDownListFor(mdl => mdl.OfficieleAfdelingID, new SelectList(values, "value", "text"))%><br />

    </fieldset>
    
    <%} %>
</asp:Content>
