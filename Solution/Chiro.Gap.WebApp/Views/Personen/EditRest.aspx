<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PersonenLedenModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <% using (Html.BeginForm()) {%>

    <fieldset>
    <legend>Persoonlijke gegevens <%=Html.ActionLink("Aanpassen", "EditGegevens", new {id=Model.PersoonLidInfo.PersoonInfo.GelieerdePersoonID}) %></legend>    
    
        <p>
        <%=Html.LabelFor(s => s.PersoonLidInfo.LidInfo.PersoonInfo.AdNummer)%>
        <%=Html.TextBox("AdNummer", Model.PersoonLidInfo.PersoonInfo.AdNummer, 
                new Dictionary<string, object> { 
                    {"readonly", "readonly"}, 
                    {"title", "Stamnummer kan niet ingegeven of gewijzigd worden." } })%>
        </p>
        
        <p>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonInfo.VoorNaam)%>
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonInfo.VoorNaam)%>
        <%=Html.HiddenFor(s => s.PersoonLidInfo.PersoonInfo.VoorNaam)%>
        </p>
        
        <p>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonInfo.Naam)%>
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonInfo.Naam)%>
        <%=Html.HiddenFor(s => s.PersoonLidInfo.PersoonInfo.Naam)%>
        </p>
        
        <p>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonInfo.GeboorteDatum)%>
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonInfo.GeboorteDatum)%>
        <%=Html.HiddenFor(s => s.PersoonLidInfo.PersoonInfo.GeboorteDatum)%>
        </p>
        
        <p>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonInfo.Geslacht)%>
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonInfo.Geslacht)%>
        <%=Html.HiddenFor(s => s.PersoonLidInfo.PersoonInfo.Geslacht)%>
        </p>
        
        <p>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonInfo.ChiroLeeftijd)%>
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonInfo.ChiroLeeftijd)%>
        <%=Html.HiddenFor(s => s.PersoonLidInfo.PersoonInfo.ChiroLeeftijd)%>
        </p>
        
        <%=Html.HiddenFor(s => s.PersoonLidInfo.PersoonInfo.PersoonID)%>
        <%=Html.HiddenFor(s => s.PersoonLidInfo.PersoonInfo.GelieerdePersoonID)%>

	</fieldset>
	
	<%if (Model.PersoonLidInfo.LidInfo != null)
   { %>
	<fieldset>
   
   	<legend>Lidgegevens en functies <%= Html.ActionLink("Aanpassen", "EditLidGegevens", new{ Controller = "Leden", id = Model.PersoonLidInfo.LidInfo.LidID}) %></legend>

		<%if (Model.PersoonLidInfo.LidInfo.Type == LidType.Kind)
	   { %>
		<p>
		<%=Html.LabelFor(s => s.PersoonLidInfo.LidInfo.LidgeldBetaald)%>
		<%=Html.DisplayFor(s => s.PersoonLidInfo.LidInfo.LidgeldBetaald)%>
		<%=Html.HiddenFor(s => s.PersoonLidInfo.LidInfo.LidgeldBetaald)%>
		</p>
		<p>
		<%=Html.LabelFor(s => s.PersoonLidInfo.LidInfo.EindeInstapperiode)%>
		<%if (Model.PersoonLidInfo.LidInfo.EindeInstapperiode.Value.CompareTo(DateTime.Today) <= 0)
		  {
			  Response.Write(" is verlopen");
		  }else{
			  %>
			 <%Response.Write(" tot " + Html.DisplayFor(s => s.PersoonLidInfo.LidInfo.EindeInstapperiode));%>
		<%} %>
		</p>
		<%}else{ %>
		<p>
		<%=Html.LabelFor(s => s.PersoonLidInfo.LidInfo.DubbelPunt)%>
		<%=Html.DisplayFor(s => s.PersoonLidInfo.LidInfo.DubbelPunt)%>
		<%=Html.HiddenFor(s => s.PersoonLidInfo.LidInfo.DubbelPunt)%>
		</p>
		<%} %>
		
		<p>
		<%=Html.LabelFor(s => s.PersoonLidInfo.LidInfo.NonActief)%>
		<%=Html.DisplayFor(s => s.PersoonLidInfo.LidInfo.NonActief)%>
		<%=Html.HiddenFor(s => s.PersoonLidInfo.LidInfo.NonActief)%>
		</p>

		
		<%
                    if (Model.PersoonLidInfo.LidInfo.Functies.Count() > 0)
                    {
        %>
        <p>Functies:</p>
        <ul>
        <%
                        foreach (var f in Model.PersoonLidInfo.LidInfo.Functies)
                        {
        %>
            <li><%=f.Naam%> (<%=f.Code%>)</li>
        <%
                        }
        %>
        </ul>
        <%
                    }
		%>
		
	       
		<%= Html.Hidden("PersoonLidInfo.LidID")%>
		<%= Html.Hidden("PersoonLidInfo.Type")%>
		<%= Html.Hidden("AfdelingsInfoDictionary")%>

    </fieldset>
    
    <h3>Afdelingen <%= Html.ActionLink("Aanpassen", "AfdelingBewerken", new { Controller="Leden", groepsWerkJaarID = Model.PersoonLidInfo.LidInfo.GroepsWerkJaarID, id = Model.PersoonLidInfo.LidInfo.LidID })%></h3>  
           
	<%if (Model.PersoonLidInfo.LidInfo.Type == LidType.Leiding)
   {
	   if (Model.PersoonLidInfo.LidInfo.AfdelingIdLijst.Count == 0)
	   {
		   Response.Write(Model.PersoonLidInfo.LidInfo.PersoonInfo.VolledigeNaam + " heeft geen afdelingen.");
	   }
	   else
	   {
		   Response.Write(Model.PersoonLidInfo.LidInfo.PersoonInfo.VolledigeNaam + " is leiding van ");
		   int geschreven = 0;
		   foreach (var ai in Model.AlleAfdelingen)
		   {
			   if (Model.PersoonLidInfo.LidInfo.AfdelingIdLijst.Contains(ai.AfdelingID))
			   {
				   if (geschreven == Model.PersoonLidInfo.LidInfo.AfdelingIdLijst.Count-1)
				   {
					   Response.Write("de " + ai.AfdelingNaam + ".\n");
				   }
				   else if (geschreven == Model.PersoonLidInfo.LidInfo.AfdelingIdLijst.Count - 2)
				   {
					   Response.Write("de " + ai.AfdelingNaam + " en ");
				   }
				   else 
				   {
					   Response.Write("de " + ai.AfdelingNaam + ", ");
				   }
				   geschreven++;
			   }
		   }
	   }
   }else{
	   //FIXME: nog niet alle info wordt ingeladen( afdelingidlijst is altijd leeg)
	   Response.Write(Model.PersoonLidInfo.LidInfo.PersoonInfo.VolledigeNaam + " zit in de " +
		   Model.AlleAfdelingen.FirstOrDefault(s => s.AfdelingID == Model.PersoonLidInfo.LidInfo.AfdelingIdLijst.ElementAt(0)).AfdelingNaam + ".");
   }%>

	<%} %>
        
    <h3>Adressen</h3>

    <ul>
    <% foreach (Chiro.Gap.ServiceContracts.PersoonsAdresInfo pa in ViewData.Model.PersoonLidInfo.PersoonsAdresInfo)
       { %>
       <li>
            <%=Html.Encode(String.Format("{0} {1}", pa.AdresInfo.StraatNaamNaam, pa.AdresInfo.HuisNr))%>,
            <%=Html.Encode(String.Format("{0} {1} ({2}) ", pa.AdresInfo.PostNr, pa.AdresInfo.WoonPlaatsNaam, pa.AdresType))%>
            <%=Html.ActionLink("[verhuizen]", "Verhuizen", new { id = pa.AdresInfo.ID, aanvragerID = ViewData.Model.PersoonLidInfo.PersoonInfo.GelieerdePersoonID })%>
            <%=Html.ActionLink("[verwijderen]", "AdresVerwijderen", new { id = pa.AdresInfo.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonInfo.GelieerdePersoonID })%>
        </li>
    <%} %>
        <li><%=Html.ActionLink("[adres toevoegen]", "NieuwAdres", new { id = ViewData.Model.PersoonLidInfo.PersoonInfo.GelieerdePersoonID })%></li>
    </ul>   
    

    <h3>Communicatie</h3>

    <ul>
    <% 
           var gegroepeerdeComm = Model.PersoonLidInfo.CommunicatieInfo.GroupBy(cv => cv.CommunicatieType);
           
           foreach (var commType in gegroepeerdeComm)
           {
               %>
               <li>
                    <%=commType.Key.Omschrijving %>
                    <ul>
                    <%
                        foreach (var cv in commType)
                        {
                            %>
                            <li>
                                <%=cv.Voorkeur ? "<strong>" + Html.Encode(cv.Nummer) + "</strong>" : Html.Encode(cv.Nummer)%>.
                                <em><%=Html.Encode(cv.Nota) %></em>
                                <%=Html.ActionLink(
                                    "[verwijderen]", 
                                    "VerwijderenCommVorm",
																		new { commvormID = cv.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonInfo.GelieerdePersoonID })%>
                                <%=Html.ActionLink(
                                    "[bewerken]", 
                                    "BewerkenCommVorm",
																		new { commvormID = cv.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonInfo.GelieerdePersoonID })%>
                            </li>                            
                            <%
                        }
                    %>
                    </ul>
               </li>
               <%
           }
           %>
    <li><%=Html.ActionLink("[communicatievorm toevoegen]", "NieuweCommVorm", new { gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonInfo.GelieerdePersoonID })%></li>
    </ul>     
 
    <h3>categorieën</h3>

    <ul>
    <% foreach (Categorie cv in Model.PersoonLidInfo.PersoonInfo.CategorieLijst)
    { %>
    <li>
            <%=cv.Naam %>
            <%=Html.ActionLink("[verwijderen]", "VerwijderenCategorie", new { categorieID = cv.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonInfo.GelieerdePersoonID })%>
        </li>
    <%} %>
    
    <li><%=Html.ActionLink("[toevoegen aan categorie]", "ToevoegenAanCategorie", new { gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonInfo.GelieerdePersoonID })%></li>
    </ul>  
 
    <%} %>
    
    <% Html.RenderPartial("TerugNaarLijstLinkControl"); %>
</asp:Content>
