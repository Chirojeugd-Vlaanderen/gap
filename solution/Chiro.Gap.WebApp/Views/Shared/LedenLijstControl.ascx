﻿<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.LidInfoModel>" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.DataContracts" %>
<% // TODO: onderstaand script moet naar 'head'. %>
<script type="text/javascript">
    $(document).ready(function () {
        $("#checkall").click(function () {
            $("INPUT[@name=SelectieGelieerdePersoonIDs][type='checkbox']").attr('checked', $("#checkall").is(':checked'));
        });
    });
</script>
<table class="overzicht">
    <tr>
        <th>
            <%=Html.CheckBox("checkall") %>
        </th>
        <th class="center">&#35;</th>
        <th class="center">Type</th>
        <th>
            <%= Html.ActionLink(
				"Naam", 
				"Lijst", 
				new { Controller = "Leden", id = Model.IDGetoondGroepsWerkJaar, sortering = LidEigenschap.Naam, afdelingID = Model.AfdelingID, ledenLijst = Model.SpecialeLijst, functieID = Model.FunctieID }, 
				new { title = "Sorteren op naam" })%>
        </th>
        <th>
            <%= Html.ActionLink(
				"Geboortedatum", 
				"Lijst", 
				new { Controller = "Leden", id = Model.IDGetoondGroepsWerkJaar, sortering = LidEigenschap.Leeftijd, afdelingID = Model.AfdelingID, ledenLijst = Model.SpecialeLijst, functieID = Model.FunctieID }, 
				new { title = "Sorteren op geboortedatum" })%>
        </th>
        <th>
            <%=Html.Geslacht(GeslachtsType.Man) %>
            <%=Html.Geslacht(GeslachtsType.Vrouw) %>
        </th>
        <th>Betaald </th>
        <% 
            // Afdelingen enkel relevant voor plaatselijke groepen		
            if ((Model.GroepsNiveau & Niveau.Groep) != 0)
            {
        %>
        <th>
            <%=Html.ActionLink(
							"Afd.",
							"Lijst",
 							new{
 								Controller = "Leden",
								id = Model.IDGetoondGroepsWerkJaar,
								groepID = Model.GroepID,
								afdelingID = Model.AfdelingID,
								ledenLijst = Model.SpecialeLijst,
								functieID = Model.FunctieID,
								sortering = LidEigenschap.Afdeling
 								},
							new {title = "Sorteren op afdeling"})%>
        </th>
        <%
            }
        %>
        <th>Func. </th>
        <th>
            <%= Html.ActionLink(
				"Instap tot",
				"Lijst",
				new{
					Controller = "Leden", 
					id = Model.IDGetoondGroepsWerkJaar, 
					sortering = LidEigenschap.InstapPeriode, 
					afdelingID = Model.AfdelingID, 
					ledenLijst = Model.SpecialeLijst, 
					functieID = Model.FunctieID
				},
				new { title = "Sorteren op einde instapperiode" })%>
        </th>
        <th>Telefoon</th>
        <th>E-mail</th>
    </tr>
    <%
        var volgnr = 0;
        foreach (LidOverzicht lidOverzicht in ViewData.Model.LidInfoLijst)
        {
            volgnr++;
            if (volgnr % 2 == 0)
            {%>
    <tr class="even">
        <%}
            else
            {%>
        <tr class="oneven">
            <%}%>
            <td>
                <input type="checkbox" name="SelectieGelieerdePersoonIDs" value="<%=lidOverzicht.GelieerdePersoonID %>"
                    <%=Model.SelectieGelieerdePersoonIDs != null && Model.SelectieGelieerdePersoonIDs.Contains(lidOverzicht.GelieerdePersoonID) ? "checked=\"checked\"" : String.Empty%> />
            </td>
            <td>
                <%=volgnr.ToString() %>
            </td>
            <td>
                <%= lidOverzicht.Type == LidType.Kind ? "Lid" : "Leiding" %>
            </td>
            <td>
                <%=Html.PersoonsLink(lidOverzicht.GelieerdePersoonID, lidOverzicht.VoorNaam, lidOverzicht.Naam)%>
                <%=lidOverzicht.SterfDatum.HasValue? "&nbsp;(&dagger;)" : string.Empty %>
            </td>
            <td class="right">
                <%=lidOverzicht.GeboorteDatum == null ? "<span class=\"error\">onbekend</span>" : ((DateTime)lidOverzicht.GeboorteDatum).ToString("d")%>
            </td>
            <td class="center">
                <%= Html.Geslacht(lidOverzicht.Geslacht)%>
            </td>
            <td>
                <%= lidOverzicht.LidgeldBetaald?"Ja":"Nee"%>
            </td>
            <% 
                if ((Model.GroepsNiveau & Niveau.Groep) != 0)
                {
                    // Afdelingen enkel relevant voor plaatselijke groepen
            %>
            <td>
                <%:Html.AfdelingsLinks(lidOverzicht.Afdelingen, Model.IDGetoondGroepsWerkJaar, Model.GroepID)%>
            </td>
            <%}%>
            <td>
                <% foreach (var functieInfo in lidOverzicht.Functies)
                   { %>
                <%=Html.ActionLink(
					Html.Encode(functieInfo.Code), 
					"Functie", 
					new { Controller = "Leden", groepsWerkJaarID = Model.IDGetoondGroepsWerkJaar, groepID = Model.GroepID, id = functieInfo.ID }, 
					new { title = "Toon alleen mensen met functie " + functieInfo.Naam })%>
                <% } %>
            </td>
            <td>
                <%=lidOverzicht.EindeInstapPeriode == null ? String.Empty : ((DateTime)lidOverzicht.EindeInstapPeriode).ToString("d") %>
            </td>
            <td>
                <%if (!lidOverzicht.SterfDatum.HasValue)
                  {%>
                <%=Html.Telefoon(lidOverzicht.TelefoonNummer)%>
                <%
                    }%>
            </td>
            <td>
                <%if (!lidOverzicht.SterfDatum.HasValue)
                  {%>
                <a href='mailto:<%=lidOverzicht.Email %>'>
                    <%=lidOverzicht.Email %></a>
                <%
                    }%>
            </td>
        </tr>
        <% } %>
</table>