<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.UitstapDeelnemersModel>" MasterPageFile="~/Views/Shared/Site.Master" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="kaderke">
    <div class="kadertitel"><%=Model.Uitstap.IsBivak ? "Details bivak" : "Details uitstap"%></div>

    <p>
    <em><%=String.Format("{0:d}", Model.Uitstap.DatumVan)%> - <%=String.Format("{0:d}", Model.Uitstap.DatumTot)%></em>. 
    <%=Model.Uitstap.Opmerkingen%>
    [<%=Html.ActionLink("Bewerken", "Bewerken", new {id = Model.Uitstap.ID})%>]
    </p>

    <p>
    <%
    	if (string.IsNullOrEmpty(Model.Uitstap.PlaatsNaam))
     {
%>
        [<%=Html.ActionLink("Bivakplaats ingeven", "PlaatsBewerken", new {id = Model.Uitstap.ID})%>]
    <%
     }
     else
     {
%>
          
          <%=Model.Uitstap.PlaatsNaam%>, 
          <%=Model.Uitstap.Adres.StraatNaamNaam%> <%=Model.Uitstap.Adres.HuisNr%> <%=Model.Uitstap.Adres.Bus%>,
          <%=Model.Uitstap.Adres.PostNr%> <%=Model.Uitstap.Adres.PostCode%> <%=Model.Uitstap.Adres.WoonPlaatsNaam%>
          (<%=Model.Uitstap.Adres.LandNaam%>)
          [<%=Html.ActionLink("Bivakplaats wijzigen", "PlaatsBewerken", new {id = Model.Uitstap.ID})%>]
          <%
     }
%>
    </p>

    </div>

    <%
        if (Model.Deelnemers == null || Model.Deelnemers.FirstOrDefault() == null)
        {
    %>
    <p>
    Je hebt nog geen deelnemers opgegeven voor deze uitstap.  
    </p>
    <%
        }
        else
        {
    %>

    <h3>Deelnemerslijst</h3>

    <table>
    <tr>
    <th /> <!-- volgnr -->
    <th>Type</th>
    <th>Afd</th>
    <th>Naam</th>
    <th>Med. Fiche</th>
    <th>Betaald</th>
    <th>Opmerkingen</th>
    <th>Acties</th>
    </tr>

    <%
      int volgnr = 0;
      foreach (var d in Model.Deelnemers.OrderByDescending(d => d.Type).ThenByDescending(d => d.Afdelingen.FirstOrDefault() == null ? String.Empty : d.Afdelingen.FirstOrDefault().Afkorting).ThenBy(d => d.VoorNaam).ThenBy(d => d.FamilieNaam))
      {
      	  string klasse = (++volgnr & 1) == 0 ? "even" : "oneven";
          if (d.IsContact)
          {
          	klasse = klasse + " highlight";
          }
    %>
        
        <tr class="<%=klasse%>">
        <td><%=volgnr %></td>
        <td><%=d.Type == DeelnemerType.Deelnemer ? "lid" : d.Type == DeelnemerType.Begeleiding ? "leiding" : "logistiek" %></td>
        <td><%=Html.AfdelingsLinks(d.Afdelingen, Model.Uitstap.GroepsWerkJaarID, Model.GroepID) %></td>
        <td><%=Html.PersoonsLink(d.GelieerdePersoonID, d.VoorNaam, d.FamilieNaam) %></td>
        <td><%=d.MedischeFicheOk ? "ja": "nee" %></td>
        <td><%=d.HeeftBetaald ? "ja": "nee" %></td>
        <td><%=d.Opmerkingen %></td>
        <td>
            <%:Html.ActionLink("uitschrijven", "Uitschrijven", new {id=d.DeelnemerID}) %>
            <%:d.IsContact ? MvcHtmlString.Empty : Html.ActionLink("instellen als contact", "ContactInstellen", new {id=d.DeelnemerID}) %>
        </td>
        </tr>

    <%
      }
    %>

    </table>

    <%
        }
     %>

     <p>
     Je kunt in het
    <%=Html.ActionLink("personen-", "Index", new {Controller="Personen", groepID = Model.GroepID} ) %> of
    <%=Html.ActionLink("ledenoverzicht", "Index", new {Controller="Leden", groepID = Model.GroepID} ) %>
    deelnemers aanvinken en inschrijven voor deze uitstap.
    </p>

                    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
