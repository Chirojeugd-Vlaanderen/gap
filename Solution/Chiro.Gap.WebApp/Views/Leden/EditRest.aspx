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
    <p>
    <%=Html.LabelFor(s => s.HuidigLid.EindeInstapperiode)%>
    <%if (Model.HuidigLid.EindeInstapperiode.Value.CompareTo(DateTime.Today) <= 0)
	  {
		  Response.Write(" is verlopen");
	  }else{
		  %>
		 <%Response.Write(" tot" + Html.DisplayFor(s => s.HuidigLid.EindeInstapperiode));%>
    <%} %>
    </p>
	<%}else{ %>
    <p>
    <%=Html.LabelFor(s => s.HuidigLid.DubbelPunt)%>
    <%=Html.EditorFor(s => s.HuidigLid.DubbelPunt)%>
    </p>
	<%} %>
	
	<p>
    <%=Html.LabelFor(s => s.HuidigLid.NonActief)%>
    <%=Html.EditorFor(s => s.HuidigLid.NonActief)%>
    </p>

	<h3>Afdelingen</h3>  
           
	<%if (Model.HuidigLid.Type == LidType.Leiding)
   {
	   if (Model.HuidigLid.AfdelingIdLijst.Count == 0)
	   {
		   Response.Write(Model.HuidigLid.PersoonInfo.VolledigeNaam + " heeft geen afdelingen.");
	   }
	   else
	   {
		   Response.Write(Model.HuidigLid.PersoonInfo.VolledigeNaam + " is leiding van:<table>");
		   foreach (var ai in Model.AlleAfdelingen)
		   {
			   if (Model.HuidigLid.AfdelingIdLijst.ElementAt(0) == ai.AfdelingID)
			   {
				   Response.Write("<tr><td>" + ai.Naam + "</td></tr>");
			   }
		   }
		   Response.Write("</table>");
	   }
   }else{
	   Response.Write(Model.HuidigLid.PersoonInfo.VolledigeNaam + " zit in de " +
		   Model.AlleAfdelingen.FirstOrDefault(s => s.AfdelingID == Model.HuidigLid.AfdelingIdLijst.ElementAt(0)).Naam + ".");
   }%>
   </br></br>
   <%= Html.ActionLink("Afdelingen aanpassen", "AfdelingBewerken", new { groepsWerkJaarID = Model.HuidigLid.GroepsWerkJaarID, id = Model.HuidigLid.LidID })%>
       
	<%= Html.Hidden("HuidigLid.LidID")%>
	<%= Html.Hidden("HuidigLid.Type")%>
	<%= Html.Hidden("AfdelingsInfoDictionary")%>
       
	</fieldset>

    <ul id="acties">
        <li><input type="submit" value="Gegevens wijzigen" /></li>        
    </ul>
    <br />
    
    <%} %>
    
    <% Html.RenderPartial("TerugNaarLijstLinkControl"); %>
</asp:Content>
