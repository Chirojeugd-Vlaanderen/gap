<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<GelieerdePersonenModel>" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
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
        foreach (PersoonDetail pi in Model.GelijkaardigePersonen)
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
            <%=Html.LabelFor(s=>s.HuidigePersoon.AdNummer)%>
            <%=Html.TextBoxFor(mdl=>mdl.HuidigePersoon.AdNummer, 
                    new Dictionary<string, object> { 
                        {"readonly", "readonly"}, 
                        {"title", "AD-nummer kan niet ingegeven of gewijzigd worden." } })%>     
            </p>
            
            <p>
            <%=Html.LabelFor(s=>s.HuidigePersoon.VoorNaam) %>
            <%=Html.EditorFor(s=>s.HuidigePersoon.VoorNaam) %>
            <%=Html.ValidationMessageFor(s => s.HuidigePersoon.VoorNaam) %>
            </p>
            
            <p>
            <%=Html.LabelFor(s=>s.HuidigePersoon.Naam) %>
            <%=Html.EditorFor(s=>s.HuidigePersoon.Naam) %>
            <%=Html.ValidationMessageFor(s => s.HuidigePersoon.Naam) %>
            </p>
            
            <p>
            <%=Html.LabelFor(s=>s.HuidigePersoon.GeboorteDatum) %>
            <%=Html.EditorFor(s=>s.HuidigePersoon.GeboorteDatum)%>
            <%=Html.ValidationMessageFor(s => s.HuidigePersoon.GeboorteDatum) %>
            </p>
            
            <p>
            <%=Html.LabelFor(s=>s.HuidigePersoon.Geslacht)%>
            <%= Html.RadioButton("HuidigePersoon.Geslacht", GeslachtsType.Man,   Model.HuidigePersoon.Geslacht == GeslachtsType.Man)%> Man
            <%= Html.RadioButton("HuidigePersoon.Geslacht", GeslachtsType.Vrouw, Model.HuidigePersoon.Geslacht == GeslachtsType.Vrouw)%> Vrouw
            </p>
            
            <p>
            <%=Html.LabelFor(s=>s.HuidigePersoon.ChiroLeefTijd)%>
            <%=Html.EditorFor(s=>s.HuidigePersoon.ChiroLeefTijd)%>
            <%=Html.ValidationMessageFor(s => s.HuidigePersoon.ChiroLeefTijd) %>
            </p>

            <%=Html.HiddenFor(s=>s.HuidigePersoon.GelieerdePersoonID)%>
            <%=Html.HiddenFor(s=>s.HuidigePersoon.VersieString)%>
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

    <% if (Model.HuidigePersoon.GelieerdePersoonID != 0) { %>
        <%=Html.ActionLink("Stop met aanpassen zonder te bewaren", "EditRest", new { id = Model.HuidigePersoon.GelieerdePersoonID})%>
    <% } %>
    
     <%=Html.ActionLink("Terug naar alle persoonsgegevens", "EditRest", new { Controller = "Personen", id = Model.HuidigePersoon.GelieerdePersoonID})%>
</asp:Content>
