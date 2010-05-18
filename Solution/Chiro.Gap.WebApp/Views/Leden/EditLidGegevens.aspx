<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.LedenModel>" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <% using (Html.BeginForm()){%>
    
    <fieldset>
    
    <legend>Lidinfo</legend>

	<%if (Model.HuidigLid.LidInfo.Type == LidType.Kind)
   { %>
	<p>
    <%=Html.LabelFor(s => s.HuidigLid.LidInfo.LidgeldBetaald)%>
    <%=Html.EditorFor(s => s.HuidigLid.LidInfo.LidgeldBetaald)%>
    </p>
	<%}else{ %>
    <p>
    <%=Html.LabelFor(s => s.HuidigLid.LidInfo.DubbelPunt)%>
    <%=Html.EditorFor(s => s.HuidigLid.LidInfo.DubbelPunt)%>
    </p>
    
	<%} %>

	<p>
    <%=Html.LabelFor(s => s.HuidigLid.LidInfo.EindeInstapperiode)%>
    <%=(Model.HuidigLid.LidInfo.EindeInstapperiode < DateTime.Today) ? "verlopen" : "tot " + Html.DisplayFor(s => s.HuidigLid.LidInfo.EindeInstapperiode)%>
    </p>
	
	<p>
    <%=Html.LabelFor(s => s.HuidigLid.LidInfo.NonActief)%>
    <%=Html.EditorFor(s => s.HuidigLid.LidInfo.NonActief)%>
    </p>
    
    <%
        if (Model.AlleFuncties != null && Model.AlleFuncties.FirstOrDefault() != null)
        {
    %>
    
        <p><strong>Functies:</strong></p>
        <%
            List<CheckBoxListInfo> info = (from f in Model.AlleFuncties
                                           select new CheckBoxListInfo(
								    f.ID.ToString(),
								    String.Format("{0} ({1})", f.Naam, f.Code),
								    Model.FunctieIDs.Contains(f.ID))).ToList();
        %>
        
        <p><%= Html.CheckBoxList("FunctieIDs", info)%></p>
    
    <%
        }
    %>
       
	<%= Html.Hidden("HuidigLid.LidInfo.LidID")%>
	<%= Html.Hidden("HuidigLid.LidInfo.PersoonDetail.GelieerdePersoonID")%>
	<%= Html.Hidden("HuidigLid.LidInfo.Type")%>
       
	</fieldset>

    <ul id="acties">
        <li><input type="submit" value="Gegevens wijzigen" /></li>        
    </ul>
    <br />
    
    <%} %>
    
    <%=Html.ActionLink("Terug naar persoonsgegevens", "EditRest", new { Controller = "Personen", id = Model.HuidigLid.PersoonDetail.GelieerdePersoonID})%>
</asp:Content>
