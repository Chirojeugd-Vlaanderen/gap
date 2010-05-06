<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.GroepsInstellingenModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
 
    <fieldset>
        <legend>Algemene groepsinfo</legend>    
        
        <p>
        <%=Html.LabelFor(mdl => mdl.Detail.ID) %>
        <%=Html.DisplayFor(mdl => mdl.Detail.ID)%>
        </p>

        <p>
        <%=Html.LabelFor(mdl => mdl.Detail.Naam)%>
        <%=Html.DisplayFor(mdl => mdl.Detail.Naam)%>
        </p>

        <p>
        <%=Html.LabelFor(mdl => mdl.Detail.Plaats)%>
        <%=Html.DisplayFor(mdl => mdl.Detail.Plaats)%>
        </p>
       
        <p>
        <%=Html.LabelFor(mdl => mdl.Detail.StamNummer)%>
        <%=Html.DisplayFor(mdl => mdl.Detail.StamNummer)%>             
        </p>
        
    </fieldset>

    <h3>Actieve afdelingen dit werkjaar</h3>
    
    <ul>
    <%
        foreach (var afd in Model.Detail.Afdelingen.OrderByDescending(afd => afd.GeboorteJaarVan))
        {
            %>
            <li><%=Html.Encode(String.Format("{0} - {1} ({2})", afd.AfdelingAfkorting, afd.AfdelingNaam, afd.OfficieleAfdelingNaam)) %></li>
            <%
        }
    %>
            <li>[<%=Html.ActionLink("afdelingsverdeling aanpassen", "Index", "Afdeling") %>]</li>
    </ul>      

    <h3>Persoonscategorie&euml;n</h3>

    <ul>
    <%
        foreach (var cat in Model.Detail.Categorieen.OrderBy(cat => cat.Code))
        {
            %>
            <li>
                <%=Html.Encode(String.Format("{0} - {1}", cat.Code, cat.Naam)) %>
                [<%=Html.ActionLink("verwijderen", "CategorieVerwijderen", new {id = cat.ID }) %>]
            </li>
            <%
        }
    %>
    </ul>  

<% 
    Html.EnableClientValidation();
    using (Html.BeginForm())
   { %>
   
    <fieldset>
    <legend>Categorie toevoegen</legend>
    <p>
    <%=Html.LabelFor(mdl=>mdl.NieuweCategorie.Naam) %>
    <%=Html.EditorFor(mdl=>mdl.NieuweCategorie.Naam) %><br />
    <%=Html.ValidationMessageFor(mdl=>mdl.NieuweCategorie.Naam) %>
    </p>
    
    <p>
    <%=Html.LabelFor(mdl=>mdl.NieuweCategorie.Code) %>
    <%=Html.EditorFor(mdl=>mdl.NieuweCategorie.Code) %><br />
    <%=Html.ValidationMessageFor(mdl=>mdl.NieuweCategorie.Code) %>
    </p>
    
    <p>
    <input type="submit" value="Bewaren"/>
    </p>
    </fieldset>        
<%} %>

</asp:Content>
