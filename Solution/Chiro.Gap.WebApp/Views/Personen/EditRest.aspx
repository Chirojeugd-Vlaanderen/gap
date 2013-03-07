<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="ViewPage<PersoonEnLidModel>" %>

<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="opzij">
        <h3>
            Contact</h3>
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
                    <%foreach (var cv in commType)
                      {
                          string ctTekst = String.Format(
                              cv.CommunicatieTypeID == (int)CommunicatieTypeEnum.Email ? "<a href='mailto:{0}'>{0}</a>" : "{0}",
                              Html.Encode(cv.Nummer));
                    %>
                    <li>
                        <%=cv.Voorkeur ? "<strong>" + ctTekst + "</strong>" : ctTekst%>
                        <em>
                            <%=Html.Encode(cv.Nota)%></em>
                        <%=Html.ActionLink("[verwijderen]", "VerwijderenCommVorm", new { commvormID = cv.ID })%>
                        <%=Html.ActionLink("[bewerken]", "CommVormBewerken", new { commvormID = cv.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
                    </li>
                    <%}%>
                </ul>
            </li>
            <%
                }
            %>
            <li>
                <%=Html.ActionLink("[communicatievorm toevoegen]", "NieuweCommVorm", new { gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%></li>
        </ul>
        <h3>
            Categorie&euml;n</h3>
        <ul>
            <% foreach (var info in Model.PersoonLidInfo.PersoonDetail.CategorieLijst)
               { %>
            <li>
                <%=Html.ActionLink(String.Format("{0} ({1})", info.Naam, info.Code), 
				    "List", 
			        "Personen",
					new { page = 1, id = info.ID, groepID = Model.GroepID, sortering = PersoonSorteringsEnum.Naam },
				    new { title= info.Naam })%>
                <%=Html.ActionLink("[verwijderen]", "VerwijderenCategorie", new { categorieID = info.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
            </li>
            <%} %>
            <li>
                <%=Html.ActionLink("[toevoegen aan categorie]", "ToevoegenAanCategorie", new { gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%></li>
        </ul>
        <h3>
            Gebruiker</h3>
        <ul>
            <%
                if (Model.PersoonLidInfo.GebruikersInfo == null)
                {
                    // Geen account
                    %>
                    <li><%: Model.PersoonLidInfo.PersoonDetail.VolledigeNaam %> heeft geen Chirologin.</li>
                    <li><%: Html.ActionLink("[Chirologin maken]", "LoginMaken", new { Controller = "GebruikersRecht", id = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID }) %></li>
                    <li><%: Html.ActionLink("[gebruikersrecht toekennen]", "AanGpToekennen", new { Controller = "GebruikersRecht", id = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID }) %></li>
                    <%
                }
                else if (Model.PersoonLidInfo.GebruikersInfo.VervalDatum < DateTime.Now)
                {
                    // Account zonder gebruikersrecht
                    %>
                    <li>Chirologin: <%: Model.PersoonLidInfo.GebruikersInfo.GavLogin %></li>
                    <li><%: Model.PersoonLidInfo.PersoonDetail.VolledigeNaam %> heeft geen toegang tot de gegevens van jouw groep</li>
                    <li><%=Html.ActionLink("[gebruikersrecht toekennen]", "AanGpToekennen", new { Controller = "GebruikersRecht", id = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID }) %></li>
                    <%                
                }
                else
                {
                    // Account met gebruikersrecht
                    // DisplayFor formatteert datums correct.
                    %>
                    <li>Chirologin: <%: Model.PersoonLidInfo.GebruikersInfo.GavLogin %></li>
                    <li>Vervaldatum gebruikersrecht: <%: Html.DisplayFor(src => src.PersoonLidInfo.GebruikersInfo.VervalDatum)%></li>
                    <% 
                        if (Model.PersoonLidInfo.GebruikersInfo.IsVerlengbaar)
                        {
                            // gebruikersrecht toekennen/verlengen is onderliggend dezelfde controller action
                        %>
                        <li><%: Html.ActionLink("[gebruikersrecht verlengen]", "AanGpToekennen", new { Controller = "GebruikersRecht", id = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID }) %></li>
                        <%                
                        }
                    %>
                    <li><%: Html.ActionLink("[gebruikersrecht afnemen]", "VanGpAfnemen", new { Controller = "GebruikersRecht", id = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID }) %></li>
                    <%                              
                }
            %>
        </ul>
    </div>
    <h3>
        Persoonlijke gegevens</h3>
    <p>
        <%= Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.VolledigeNaam) %>
        (<%=Html.Geslacht(Model.PersoonLidInfo.PersoonDetail.Geslacht) %>)
        <br />
        <%if (Model.PersoonLidInfo.PersoonDetail.AdNummer != null)
          {%>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonDetail.AdNummer) %>
       &nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "AD-nummer", new { helpBestand = "Trefwoorden" }, new { title = "Wat is een AD-nummer?" } ) %>:
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.AdNummer) %><br />
        <%
    }%>
        <%=Html.LabelFor(s => s.PersoonLidInfo.PersoonDetail.GeboorteDatum)%>:
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.GeboorteDatum)%>
        <br />
        <%
            if ((Model.GroepsNiveau & Niveau.Groep) != 0)
            {
                // Chiroleeftijd is enkel relevant voor plaatselijke groepen
        %>
        Chiroleeftijd
       &nbsp;<%= Html.ActionLink("[?]", "ViewTonen", "Handleiding", null, null, "Chiroleeftijd", new { helpBestand = "Trefwoorden" }, new { title = "Wat is je Chiroleeftijd?" } ) %>:
        <%=Html.DisplayFor(s => s.PersoonLidInfo.PersoonDetail.ChiroLeefTijd)%><br />
        <%
         }
        %>
        <%=Html.ActionLink("[persoonlijke gegevens aanpassen]", "EditGegevens", new {id=Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID}) %><br />
    </p>
    <p>
        <%if (Model.PersoonLidInfo.PersoonDetail.DubbelPuntAbonnement)
          {
        %>
        <%=Model.PersoonLidInfo.PersoonDetail.VolledigeNaam %>
        is geabonneerd op <a href='http://www.chiro.be/dubbelpunt'>Dubbelpunt</a>.
        <%
    }
          else
          {
        %>
        Geen <a href="http://www.chiro.be/dubbelpunt">Dubbelpuntabonnement</a>.
        <%=Html.ActionLink("[abonneren]", "DubbelPuntAanvragen", new {Controller="Abonnementen", id = Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID}) %>
        (Kostprijs: &euro;
        <%=Model.PrijsDubbelPunt.ToString() %>)
        <%
    }
        %>
    </p>
    <h3>
        Adressen</h3>
    <ul>
        <% foreach (PersoonsAdresInfo pa in ViewData.Model.PersoonLidInfo.PersoonsAdresInfo)
           { %>
        <li>
            <%if (Model.PersoonLidInfo.PersoonDetail.VoorkeursAdresID == pa.PersoonsAdresID) %>
            <%{%>
            <b>post:
                <%=Html.Encode(String.Format("{0} {1} {2}", pa.StraatNaamNaam, pa.HuisNr, pa.Bus))%>,
                <%=Html.Encode(String.Format("{0} {3} {1} ({2}) ", pa.PostNr, pa.WoonPlaatsNaam, pa.AdresType, pa.PostCode))%>
            </b>
            <%}
              else
              {%>
            <%=Html.Encode(String.Format("{0} {1} {2}", pa.StraatNaamNaam, pa.HuisNr, pa.Bus))%>,
            <%=Html.Encode(String.Format("{0} {3} {1} ({2}) ", pa.PostNr, pa.WoonPlaatsNaam, pa.AdresType, pa.PostCode))%>
            <%=Html.ActionLink("[Voorkeursadres maken]", "VoorkeurAdresMaken", new { persoonsAdresID = pa.PersoonsAdresID, gelieerdePersoonID = Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
            <%} %>
            <%=Html.ActionLink("[verhuizen]", "Verhuizen", new { id = pa.ID, aanvragerID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
            <%=Html.ActionLink("[verwijderen]", "AdresVerwijderen", new { id = pa.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>
        </li>
        <%} %>
        <li>
            <%=Html.ActionLink("[adres toevoegen]", "NieuwAdres", new { id = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%></li>
    </ul>
    <%if ((Model.PersoonLidInfo.PersoonDetail.IsLid || Model.PersoonLidInfo.PersoonDetail.IsLeiding) &&
       !Model.PersoonLidInfo.LidInfo.NonActief)
      {

          // Lidgegevens worden enkel getoond voor actieve leden.
          // TODO: Het bestaan van non-actieve leden mag volgens mij niet geweten zijn aan de UI-kant
	   
    %>
    <h3>
        Ingeschreven voor
        <%=Model.HuidigWerkJaar%>-<%=Model.HuidigWerkJaar+1%></h3>
    <ul>
        <li>Functies:
            <%foreach (var f in Model.PersoonLidInfo.LidInfo.Functies)
              {%>
            <%=Html.ActionLink(f.Code, 
				    "Functie", 
			        "Leden",
				    new { groepsWerkJaarID = Model.PersoonLidInfo.LidInfo.GroepsWerkJaarID,
							id = f.ID,
							groepID = Model.GroepID, 
						},
				    new { title= f.Naam })%>
            <% }%>
            <%= Html.ActionLink("[functies aanpassen]", "FunctiesToekennen", new { Controller = "Leden", id = Model.PersoonLidInfo.LidInfo.LidID })%>
        </li>
        <%
       if ((Model.GroepsNiveau & Niveau.Groep) != 0)
       {
           // Lid/Leiding en afdelingen zijn enkel relevant voor plaatselijke groepen.
        %>
        <li>Ingeschreven als
            <%=Model.PersoonLidInfo.LidInfo.Type == LidType.Kind ? "lid" : "leiding"%>.
            <%=Html.ActionLink("[wissel lid/leiding]",
			                                  "TypeToggle",
			                                  new {Controller = "Leden", id = Model.PersoonLidInfo.LidInfo.LidID})%>
        </li>
        <li>
            <%
            if (Model.PersoonLidInfo.LidInfo.Type == LidType.Leiding)
            {
                Response.Write(String.Format(
                    "Afdeling(en): {0} ",
                    Html.PrintLijst(Model.PersoonLidInfo.LidInfo.AfdelingIdLijst, Model.AlleAfdelingen)));
            }
            else
            {
                // TODO: Opkuis
                if (Model.PersoonLidInfo.LidInfo.AfdelingIdLijst.Count > 0 &&
                    Model.AlleAfdelingen.FirstOrDefault(
                        s => s.AfdelingID == Model.PersoonLidInfo.LidInfo.AfdelingIdLijst.ElementAt(0)) != null)
                {
                    Response.Write(String.Format("Afdeling: {0} ",
                                     Model.AlleAfdelingen.First(
                                                    s => s.AfdelingID == Model.PersoonLidInfo.LidInfo.AfdelingIdLijst.ElementAt(0)).
                                                    AfdelingNaam));
                }
            }
            %>
            <%=Html.ActionLink("[aanpassen]",
							  "AfdelingBewerken",
							  new
	                                  			{
	                                  				Controller = "Leden",
	                                  				groepsWerkJaarID = Model.PersoonLidInfo.LidInfo.GroepsWerkJaarID,
	                                  				lidID = Model.PersoonLidInfo.LidInfo.LidID
	                                  			})%>
        </li>
        <%
        }
        if ((Model.GroepsNiveau & (Niveau.Gewest | Niveau.Verbond | Niveau.Nationaal)) == 0)
        {
            // Lidgeld en instapperiode zijn niet van toepassing op kadergroepen.
        %>
        <li>Betaalde
            <%= Model.PersoonLidInfo.LidInfo.LidgeldBetaald ? String.Empty : "nog geen" %>
            lidgeld.
            <%= Html.ActionLink("[aanpassen]", "LidGeldToggle", new{ Controller = "Leden", id = Model.PersoonLidInfo.LidInfo.LidID}) %>
        </li>
        <li>Instapperiode
            <%= String.Format(
				      Model.PersoonLidInfo.LidInfo.EindeInstapperiode < DateTime.Today ? "verliep op {0:d}" : "tot {0:d}",
							 Model.PersoonLidInfo.LidInfo.EindeInstapperiode)  %>
        </li>
        <%
        }
        %>
        <% if (Model.PersoonLidInfo.LidInfo.VerzekeringLoonVerlies)
           {%>
        <li>Verzekerd tegen loonverlies</li>
        <% }
           else if (Model.PersoonLidInfo.PersoonDetail.GeboorteDatum != null && Model.KanVerzekerenLoonVerlies)
           { %>
        <li>Niet extra verzekerd tegen loonverlies.
            <%=Html.ActionLink("[verzekeren tegen loonverlies]", "LoonVerliesVerzekeren", new {Controller="Leden", id = Model.PersoonLidInfo.LidInfo.LidID}) %>
            <%if (Model.GroepsNiveau.HasFlag(Niveau.KaderGroep))
              { %>
              (dit is gratis voor kaderleden)
              <% }
              else
              { %>
            (Kostprijs: &euro;
            <%= Model.PrijsVerzekeringLoonVerlies.ToString()%>) 
            <% } %>
        </li>
        <%}%>
        <li>
            <%=Html.ActionLink("Uitschrijven", "Uitschrijven", new { Controller = "Personen", gelieerdepersoonID = Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })
            %>
        </li>
    </ul>
    <%}
      else
      {
          if (Model.PersoonLidInfo.PersoonDetail.KanLidWorden || Model.PersoonLidInfo.PersoonDetail.KanLeidingWorden)
          {
    %>
    <h3>
        <%=Model.PersoonLidInfo.PersoonDetail.VolledigeNaam %>
        is niet ingeschreven</h3>
    <%
    %>
    <p>
        <%=Html.ActionLink(String.Format(
			"inschrijven als {0}", 
			Model.PersoonLidInfo.PersoonDetail.KanLidWorden ? "lid": "leiding"), 
			"Inschrijven", 
			new
				{
					Controller = "Personen", 
					gelieerdepersoonID = Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID
				})%>
    </p>
    <%
       }
   }
    %>
    <% Html.RenderPartial("TerugNaarLijstLinkControl"); %>
</asp:Content>
