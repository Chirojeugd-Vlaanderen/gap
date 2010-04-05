<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<LedenModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <% using (Html.BeginForm()){%>
    
    <fieldset>
    
    <h3>Lid-info</h3>

	<%if (Model.HuidigLid.Type == LidType.Kind){ %>
	<p>
    <%=Html.LabelFor(s => s.HuidigLid.LidgeldBetaald)%>
    <%=Html.EditorFor(s => s.HuidigLid.LidgeldBetaald)%>
    </p>
	<%}else{ %>
    <p>
    <%=Html.LabelFor(s => s.HuidigLid.DubbelPunt)%>
    <%=Html.EditorFor(s => s.HuidigLid.DubbelPunt)%>
    </p>
    
	<%} %>

	<p>
    <%=Html.LabelFor(s => s.HuidigLid.EindeInstapperiode) %>
    <%=(Model.HuidigLid.EindeInstapperiode < DateTime.Today) ? "verlopen" : "tot " + Html.DisplayFor(s => s.HuidigLid.EindeInstapperiode)%>
    </p>
	
	<p>
    <%=Html.LabelFor(s => s.HuidigLid.NonActief)%>
    <%=Html.EditorFor(s => s.HuidigLid.NonActief)%>
    </p>
       
	<%= Html.Hidden("HuidigLid.LidID")%>
	<%= Html.Hidden("HuidigLid.PersoonInfo.GelieerdePersoonID")%>
	<%= Html.Hidden("HuidigLid.Type")%>
	<%= Html.Hidden("AfdelingsInfoDictionary")%>
       
	</fieldset>

    <ul id="acties">
        <li><input type="submit" value="Gegevens wijzigen" /></li>        
    </ul>
    <br />
    
    <%} %>
    
    <%=Html.ActionLink("Terug naar persoonsgegevens", "EditRest", new { Controller = "Personen", id = Model.HuidigLid.PersoonInfo.GelieerdePersoonID})%>
</asp:Content>
