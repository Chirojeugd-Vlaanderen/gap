<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<LedenModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <% using (Html.BeginForm()) {%>
    
    <h3>Lid-info</h3>

	<%if(Model.HuidigLid.Type == LidType.Kind){ %>
	<p>
    <%=Html.LabelFor(s=>s.HuidigLid.LidgeldBetaald) %>
    <%=Html.DisplayFor(s => s.HuidigLid.LidgeldBetaald)%>
    <%=Html.HiddenFor(s=>s.HuidigLid.LidgeldBetaald)%>
    </p>
    <p>
    <%=Html.LabelFor(s => s.HuidigLid.NonActief)%>
    <%=Html.DisplayFor(s => s.HuidigLid.NonActief)%>
    <%=Html.HiddenFor(s => s.HuidigLid.NonActief)%>
    </p>
    <p>
    <%=Html.LabelFor(s => s.HuidigLid.EindeInstapperiode)%>
    <%=Html.DisplayFor(s => s.HuidigLid.EindeInstapperiode)%>
    <%=Html.HiddenFor(s => s.HuidigLid.EindeInstapperiode)%>
    </p>
	<%}else{ %>
    <p>
    <%=Html.LabelFor(s => s.HuidigLid.DubbelPunt)%>
    <%=Html.DisplayFor(s => s.HuidigLid.DubbelPunt)%>
    <%=Html.HiddenFor(s => s.HuidigLid.DubbelPunt)%>
    </p>
    <p>
    <%=Html.LabelFor(s => s.HuidigLid.NonActief)%>
    <%=Html.DisplayFor(s => s.HuidigLid.NonActief)%>
    <%=Html.HiddenFor(s => s.HuidigLid.NonActief)%>
    </p>
	<%} %>

	<h3>Afdelingen</h3>

<%--	<ul>
	<% foreach (int afd in Model.HuidigLid.AfdelingIdLijst)
	   { %>
	   <li>
			<%	Chiro.Gap.ServiceContracts.AfdelingInfo value;
				Model.AfdelingsInfoDictionary.TryGetValue(afd, out value); %>
			<%= value %>
		</li>
	<%} %>
	</ul>--%>
    
    <ul id="acties">
        <li><input type="submit" value="Gegevens wijzigen" /></li>        
    </ul>
    <br />

    <fieldset>
        <legend>Persoonlijke gegevens</legend> 
        
        extra gegevens...         
    </fieldset>
 
    <%} %>
    
    <% Html.RenderPartial("TerugNaarLijstLinkControl"); %>
</asp:Content>
