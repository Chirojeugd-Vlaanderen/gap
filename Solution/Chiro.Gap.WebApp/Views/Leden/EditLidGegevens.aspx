<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.LedenModel>" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <% using (Html.BeginForm()){%>
    
    <fieldset>
    
    <legend>Lidinfo</legend>

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
       
	<%= Html.Hidden("HuidigLid.LidID")%>
	<%= Html.Hidden("HuidigLid.PersoonDetail.GelieerdePersoonID")%>
	<%= Html.Hidden("HuidigLid.Type")%>
       
	</fieldset>

    <ul id="acties">
        <li><input type="submit" value="Gegevens wijzigen" /></li>        
    </ul>
    <br />
    
    <%} %>
    
    <%=Html.ActionLink("Terug naar persoonsgegevens", "EditRest", new { Controller = "Personen", id = Model.HuidigLid.PersoonDetail.GelieerdePersoonID})%>
</asp:Content>
