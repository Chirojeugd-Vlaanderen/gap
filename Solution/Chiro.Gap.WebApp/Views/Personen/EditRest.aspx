<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.PersonenLedenModel>" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <% using (Html.BeginForm()) {%>

    <fieldset>
    <legend>Persoonlijke gegevens <%=Html.ActionLink("Aanpassen", "EditGegevens", new {id=Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID}) %></legend>    
    
        <p>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonDetail.AdNummer)%>
        <%=Html.TextBox("AdNummer", Model.PersoonLidInfo.PersoonDetail.AdNummer, 
                new Dictionary<string, object> { 
                    {"readonly", "readonly"}, 
                    {"title", "AD-nummer kan niet ingegeven of gewijzigd worden." } })%>
        </p>
        
        <p>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonDetail.VoorNaam)%>
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.VoorNaam)%>
        <%=Html.HiddenFor(s => s.PersoonLidInfo.PersoonDetail.VoorNaam)%>
        </p>
        
        <p>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonDetail.Naam)%>
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.Naam)%>
        <%=Html.HiddenFor(s => s.PersoonLidInfo.PersoonDetail.Naam)%>
        </p>
        
        <p>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonDetail.GeboorteDatum)%>
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.GeboorteDatum)%>
        <%=Html.HiddenFor(s => s.PersoonLidInfo.PersoonDetail.GeboorteDatum)%>
        </p>
        
        <p>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonDetail.Geslacht)%>
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.Geslacht)%>
        <%=Html.HiddenFor(s => s.PersoonLidInfo.PersoonDetail.Geslacht)%>
        </p>
        
        <p>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonDetail.ChiroLeefTijd)%>
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.ChiroLeefTijd)%>
        <%=Html.HiddenFor(s => s.PersoonLidInfo.PersoonDetail.ChiroLeefTijd)%>
        </p>
        
        <%=Html.HiddenFor(s => s.PersoonLidInfo.PersoonDetail.PersoonID)%>
        <%=Html.HiddenFor(s => s.PersoonLidInfo.PersoonDetail.GelieerdePersoonID)%>

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
			  Response.Write(" tot " + Html.DisplayFor(s => s.PersoonLidInfo.LidInfo.EindeInstapperiode));
		  } %>
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
		   Response.Write(Model.PersoonLidInfo.PersoonDetail.VolledigeNaam + " heeft geen afdelingen.");
	   }
	   else
	   {
		   Response.Write(Model.PersoonLidInfo.PersoonDetail.VolledigeNaam + " is leiding van ");
		   Response.Write(Html.PrintLijst(Model.PersoonLidInfo.LidInfo.AfdelingIdLijst, Model.AlleAfdelingen));
	   }
   }else{
	   //FIXME: nog niet alle info wordt ingeladen( afdelingidlijst is altijd leeg)
	   Response.Write(Model.PersoonLidInfo.PersoonDetail.VolledigeNaam + " zit in de " +
		   Model.AlleAfdelingen.FirstOrDefault(s => s.AfdelingID == Model.PersoonLidInfo.LidInfo.AfdelingIdLijst.ElementAt(0)).AfdelingNaam + ".");
   }%>

	<%} %>
        
    <h3>Adressen</h3>

    <ul>
    <% foreach (PersoonsAdresInfo pa in ViewData.Model.PersoonLidInfo.PersoonsAdresInfo)
       { %>
       <li>
            <%=Html.Encode(String.Format("{0} {1} {2}", pa.StraatNaamNaam, pa.HuisNr, pa.Bus))%>,
            <%=Html.Encode(String.Format("{0} {1} ({2}) ", pa.PostNr, pa.WoonPlaatsNaam, pa.AdresType))%>
            <%=Html.ActionLink("[verhuizen]", "Verhuizen", new { id = pa.ID, aanvragerID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
            <%=Html.ActionLink("[verwijderen]", "AdresVerwijderen", new { id = pa.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
        </li>
    <%} %>
        <li><%=Html.ActionLink("[adres toevoegen]", "NieuwAdres", new { id = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%></li>
    </ul>   
    

    <h3>Communicatie</h3>

    <ul>
    <% 
           var gegroepeerdeComm = Model.PersoonLidInfo.CommunicatieInfo.GroupBy(
					cv => new
					{
						Omschrijving = cv.CommunicatieTypeOmschrijving, 
						Validatie = cv.CommunicatieTypeValidatie, 
						Voorbeeld = cv.CommunicatieTypeVoorbeeld
					});
           
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
																		new { commvormID = cv.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
                                <%=Html.ActionLink(
                                    "[bewerken]", 
                                    "BewerkenCommVorm",
																		new { commvormID = cv.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
                            </li>                            
                            <%
                        }
                    %>
                    </ul>
               </li>
               <%
           }
           %>
    <li><%=Html.ActionLink("[communicatievorm toevoegen]", "NieuweCommVorm", new { gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%></li>
    </ul>     
 
    <h3>categorieën</h3>

    <ul>
    <% foreach (var info in Model.PersoonLidInfo.PersoonDetail.CategorieLijst)
    { %>
    <li>
            <%=info.Naam %>
            <%=Html.ActionLink("[verwijderen]", "VerwijderenCategorie", new { categorieID = info.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
        </li>
    <%} %>
    
    <li><%=Html.ActionLink("[toevoegen aan categorie]", "ToevoegenAanCategorie", new { gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%></li>
    </ul>  
 
    <%} %>
    
    <% Html.RenderPartial("TerugNaarLijstLinkControl"); %>
</asp:Content>
