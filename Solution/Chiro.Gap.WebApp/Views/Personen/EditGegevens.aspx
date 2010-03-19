<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<GelieerdePersonenModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.min.js")%>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>"></script>
    <script type="text/javascript" src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <%=Html.ValidationSummary("Er zijn enkele opmerkingen:") %>
    
    <% Html.EnableClientValidation(); %>

    <% using (Html.BeginForm()) { %>
    <% Html.EnableClientValidation(); %>
    
    
    <ul id="acties">
        <li><input type="submit" value="Bewaren"/></li>
        <li><input type="reset"  value=" Reset "/></li>
    </ul>
    <br />
    <%
        if (Model.GelijkaardigePersonen != null && Model.GelijkaardigePersonen.Count() > 0)
        {
            // Toon gelijkaardige personen
    %>
    
    <p class="validation-summary-errors">
    Let op! Uw nieuwe persoon lijkt verdacht veel op (een) reeds bestaande perso(o)n(en).
    Als u zeker bent dat u niemand dubbel toevoegt, klik dan opnieuw op 
    &lsquo;Bewaren&rsquo;.
    </p>
    
    <ul>
    <% 
        foreach (PersoonInfo pi in Model.GelijkaardigePersonen)
        {
            %>
            <li><%Html.RenderPartial("PersoonsLinkControl", pi);%> - <%=String.Format("{0:d}", pi.GeboorteDatum) %></li>
            <%
        }
    %>
    </ul>    
    <%      
        }
    %>
    
    <fieldset>
        <legend>Persoonlijke gegevens</legend>          
            
            <p>
            <%=Html.LabelFor(s=>s.HuidigePersoon.Persoon.AdNummer)%>
            <%=Html.TextBoxFor(mdl=>mdl.HuidigePersoon.Persoon.AdNummer, 
                    new Dictionary<string, object> { 
                        {"readonly", "readonly"}, 
                        {"title", "AD-nummer kan niet ingegeven of gewijzigd worden." } })%>     
            </p>
            
            <p>
            <%=Html.LabelFor(s=>s.HuidigePersoon.Persoon.VoorNaam) %>
            <%=Html.EditorFor(s=>s.HuidigePersoon.Persoon.VoorNaam) %>
            </p>
            
            <p>
            <%=Html.LabelFor(s=>s.HuidigePersoon.Persoon.Naam) %>
            <%=Html.EditorFor(s=>s.HuidigePersoon.Persoon.Naam) %>
            </p>
            
            <p>
            <%=Html.LabelFor(s=>s.HuidigePersoon.Persoon.GeboorteDatum) %>
            <%=Html.EditorFor(s=>s.HuidigePersoon.Persoon.GeboorteDatum)%>
            </p>
            
            <p>
            <%=Html.LabelFor(s=>s.HuidigePersoon.Persoon.Geslacht)%>
            <%= Html.RadioButton("HuidigePersoon.Persoon.Geslacht", GeslachtsType.Man,   Model.HuidigePersoon.Persoon.Geslacht == GeslachtsType.Man)%> Man
            <%= Html.RadioButton("HuidigePersoon.Persoon.Geslacht", GeslachtsType.Vrouw, Model.HuidigePersoon.Persoon.Geslacht == GeslachtsType.Vrouw)%> Vrouw
            </p>
            
            <p>
            <%=Html.LabelFor(s=>s.HuidigePersoon.ChiroLeefTijd)%>
            <%=Html.EditorFor(s=>s.HuidigePersoon.ChiroLeefTijd)%>
            </p>

            <%=Html.HiddenFor(s=>s.HuidigePersoon.ID)%>
            <%=Html.HiddenFor(s=>s.HuidigePersoon.VersieString)%>
            <%=Html.HiddenFor(s=>s.HuidigePersoon.Persoon.ID)%>
            <%=Html.HiddenFor(s=>s.HuidigePersoon.Persoon.VersieString)%>
            <%
                if (Model.Forceer)
                {
                    // Ik krijg onderstaande niet geregeld met een html helper... :(
                    %>
                    <input type="hidden" name="Forceer" id="Forceer" value="True" />
                    <%
                }
            %>
            
     </fieldset>
     
     <%} %>
    
    <h3>Adressen</h3>

    <ul>
    <% foreach (PersoonsAdres pa in ViewData.Model.HuidigePersoon.Persoon.PersoonsAdres)
       { %>
       <li>
            <%=Html.Encode(String.Format("{0} {1}", pa.Adres.StraatNaam.Naam, pa.Adres.HuisNr))%>,
            <%=Html.Encode(String.Format("{0} {1} {2} ({3}) ", pa.Adres.StraatNaam.PostNummer, pa.Adres.PostCode, pa.Adres.WoonPlaats.Naam, pa.AdresType))%>
            [verhuizen],[verwijderen]
        </li>
    <% } %>
        <li>[nieuw adres]</li>
    </ul>   
    
    <h3>Communicatie</h3>

    <ul>
    <% foreach (CommunicatieVorm cv in Model.HuidigePersoon.Communicatie)
       { %>
        <li>
            <%=cv.CommunicatieType.Omschrijving%>:
            <%=Html.Encode(cv.Nummer)%>
            <%=cv.Voorkeur ? "(voorkeur)" : ""%>
        </li>
    <% } %>
    </ul>
    
    <h3>categorieën</h3>

    <ul>
    <% foreach (Categorie cv in Model.HuidigePersoon.Categorie)
    { %>
    <li>
            <%=cv.Naam %>
        </li>
    <%} %>
    </ul>
    
    <% if (Model.HuidigePersoon.ID != 0) { %>
        <%=Html.ActionLink("Stop met aanpassen zonder te bewaren", "EditRest", new { id = Model.HuidigePersoon.ID})%>
    <% } %>
    
    <% Html.RenderPartial("TerugNaarLijstLinkControl"); %>
</asp:Content>
