<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.PersonenLedenModel>" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="opzij">
    <h3>Contact</h3>
    <ul>
    <% 
		var gegroepeerdeComm = Model.PersoonLidInfo.CommunicatieInfo.GroupBy(
				cv => new
				{
					Omschrijving = cv.CommunicatieTypeOmschrijving, 
					Validatie = cv.CommunicatieTypeValidatie, 
					Voorbeeld = cv.CommunicatieTypeVoorbeeld
				});

		foreach (var commType in gegroepeerdeComm){
		   %>
		   <li><%=commType.Key.Omschrijving %>
				<ul>
				<%foreach (var cv in commType){
	                string ctTekst = String.Format(
		                cv.CommunicatieTypeID == 3 ? "<a href='mailto:{0}'>{0}</a>" : "{0}",
		                Html.Encode(cv.Nummer));
                    %>
					<li>
						<%=cv.Voorkeur ? "<strong>" + ctTekst + "</strong>" : ctTekst%>
						<em><%=Html.Encode(cv.Nota)%></em>
						<%=Html.ActionLink("[verwijderen]", "VerwijderenCommVorm", new { commvormID = cv.ID })%>
						<%=Html.ActionLink("[bewerken]", "CommVormBewerken", new { commvormID = cv.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
					</li>                            
				<%}%>
				</ul>
		   </li>
		   <%
		}
		%>
    <li><%=Html.ActionLink("[communicatievorm toevoegen]", "NieuweCommVorm", new { gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%></li>
    </ul>     

    <h3>Categorieën</h3>

    <ul>
    <% foreach (var info in Model.PersoonLidInfo.PersoonDetail.CategorieLijst)
    { %>
    <li>
				<%=Html.ActionLink(String.Format("{0} ({1})", info.Naam, info.Code), 
				    "List", 
			        "Personen",
				    new { page = 1, id = info.ID, groepID = Model.GroepID },
				    new { title= info.Naam })%>    
            <%=Html.ActionLink("[verwijderen]", "VerwijderenCategorie", new { categorieID = info.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
        </li>
    <%} %>
    
    <li><%=Html.ActionLink("[toevoegen aan categorie]", "ToevoegenAanCategorie", new { gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%></li>
    </ul>


    </div>


        <h3>Persoonlijke gegevens</h3>
		<%=Html.Geslacht(Model.PersoonLidInfo.PersoonDetail.Geslacht) %>

        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.VolledigeNaam) %>
		<br />

        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.GeboorteDatum)%>
        &nbsp;Chiroleeftijd: <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.ChiroLeefTijd)%><br />
        
        <%if (Model.PersoonLidInfo.PersoonDetail.AdNummer != null)
          {%>
        AD-nummer: <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.AdNummer)%><br />
        <%
          }%>
        <%=Html.ActionLink("[persoonlijke gegevens aanpassen]", "EditGegevens", new {id=Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID}) %>
        
    <h3>Adressen</h3>

    <ul>
    <% foreach (PersoonsAdresInfo pa in ViewData.Model.PersoonLidInfo.PersoonsAdresInfo)
       { %>
       <li>
			<%if(Model.PersoonLidInfo.PersoonDetail.VoorkeursAdresID == pa.PersoonsAdresID) %>
			<%{%>
			<b>post: 
			<%=Html.Encode(String.Format("{0} {1} {2}", pa.StraatNaamNaam, pa.HuisNr, pa.Bus))%>,
			<%=Html.Encode(String.Format("{0} {1} ({2}) ", pa.PostNr, pa.WoonPlaatsNaam, pa.AdresType))%>
			</b>
			<%}else{%>
			<%=Html.Encode(String.Format("{0} {1} {2}", pa.StraatNaamNaam, pa.HuisNr, pa.Bus))%>,
			<%=Html.Encode(String.Format("{0} {1} ({2}) ", pa.PostNr, pa.WoonPlaatsNaam, pa.AdresType))%>
			<%=Html.ActionLink("[Voorkeursadres maken]", "VoorkeurAdresMaken", new { persoonsAdresID = pa.PersoonsAdresID, gelieerdePersoonID = Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
			<%} %>            
            
            <%=Html.ActionLink("[verhuizen]", "Verhuizen", new { id = pa.ID, aanvragerID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
            <%=Html.ActionLink("[verwijderen]", "AdresVerwijderen", new { id = pa.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
        </li>
    <%} %>
        <li><%=Html.ActionLink("[adres toevoegen]", "NieuwAdres", new { id = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%></li>
    </ul>          
	
	<%if ((Model.PersoonLidInfo.PersoonDetail.IsLid || Model.PersoonLidInfo.PersoonDetail.IsLeiding) && 
       !Model.PersoonLidInfo.LidInfo.NonActief){ 
       
       // Lidgegevens worden enkel getoond voor actieve leden.
       %>
	<h3>Lidgegevens</h3>
	
	<ul>
	<li>Betaalde <%= Model.PersoonLidInfo.LidInfo.LidgeldBetaald ? String.Empty : "nog geen" %> lidgeld. 
	<%= Html.ActionLink("[aanpassen]", "EditLidGegevens", new{ Controller = "Leden", id = Model.PersoonLidInfo.LidInfo.LidID}) %></li>
	<li>Instapperiode <%= String.Format(
	                      Model.PersoonLidInfo.LidInfo.EindeInstapperiode < DateTime.Today ? "verliep op {0:d}" : "t/m {0:d}",
	                                         Model.PersoonLidInfo.LidInfo.EindeInstapperiode)  %></li>
	<li>Functies:
				<%foreach (var f in Model.PersoonLidInfo.LidInfo.Functies){%>
				<%=Html.ActionLink(f.Code, 
				    "FunctieLijst", 
			        "Leden",
				    new { groepsWerkJaarID = Model.PersoonLidInfo.LidInfo.GroepsWerkJaarID,
                                        funcID = f.ID,
                                        groepID = Model.GroepID },
				    new { title= f.Naam })%>
                <% }%>
	<%= Html.ActionLink("[functies aanpassen]", "EditLidGegevens", new{ Controller = "Leden", id = Model.PersoonLidInfo.LidInfo.LidID}) %></li>
	
	<li>
	    <%if (Model.PersoonLidInfo.LidInfo.Type == LidType.Leiding)
       {
           Response.Write(String.Format(
               "Afdeling(en): {0} ", 
               Html.PrintLijst(Model.PersoonLidInfo.LidInfo.AfdelingIdLijst, Model.AlleAfdelingen)));
       }else{
	       if(Model.AlleAfdelingen.FirstOrDefault(s => s.AfdelingID == Model.PersoonLidInfo.LidInfo.AfdelingIdLijst.ElementAt(0))!=null)
	       {
	   	    Response.Write(String.Format("Afdeling: {0} ",
	   	                   Model.AlleAfdelingen.First(
	   	               	    s => s.AfdelingID == Model.PersoonLidInfo.LidInfo.AfdelingIdLijst.ElementAt(0)).AfdelingNaam));
	       }
       }%>	
       
       <%= Html.ActionLink("[aanpassen]", "AfdelingBewerken", new { Controller="Leden", groepsWerkJaarID = Model.PersoonLidInfo.LidInfo.GroepsWerkJaarID, lidID = Model.PersoonLidInfo.LidInfo.LidID })%>
	</li>
	
	<% if (Model.PersoonLidInfo.LidInfo.VerzekeringLoonVerlies) {%>
	<li>Verzekerd tegen loonverlies</li>
    <% }
    else if (Model.PersoonLidInfo.PersoonDetail.GeboorteDatum != null && Model.KanVerzekerenLoonVerlies)
    { %>
            	<li>Niet extra verzekerd tegen loonverlies. 
            	<%=Html.ActionLink("[verzekeren tegen loonverlies]", "LoonVerliesVerzekeren", new {Controller="Leden", id = Model.PersoonLidInfo.LidInfo.LidID}) %>
            	(Kostprijs: &euro; <%=Model.PrijsVerzekeringLoonVerlies %>)
            	</li>
    <%}%>
        
	<!-- Dubbelpunt moet verhuisd worden naar personenniveau -->
	</ul>
 

	<%}else{ 
		if (Model.PersoonLidInfo.PersoonDetail.KanLidWorden || Model.PersoonLidInfo.PersoonDetail.KanLeidingWorden)
		{
			%>
			
			<h3><%=Model.PersoonLidInfo.PersoonDetail.VolledigeNaam %> is niet ingeschreven</h3>
			
			<%if (Model.PersoonLidInfo.PersoonDetail.KanLidWorden){%>
				<p>
				<%=Html.ActionLink("Inschrijven als lid", "LidMaken", new { Controller = "Personen", gelieerdepersoonID = Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
				 </p>
			<%}
			if (Model.PersoonLidInfo.PersoonDetail.KanLeidingWorden)
			{%>
				<p>
				<%=Html.ActionLink("Inschrijven als leiding","LeidingMaken",new{Controller = "Personen",gelieerdepersoonID = Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID})%>
				</p>
			<%}
		}
    }%>
    
 
    <% Html.RenderPartial("TerugNaarLijstLinkControl"); %>
</asp:Content>
