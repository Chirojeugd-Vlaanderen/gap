<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.GroepsInstellingenModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
 
    <fieldset>
        <legend>Algemene groepsinfo</legend>    
        
        <p>
        <%=Html.LabelFor(mdl => mdl.Info.ID) %>
        <%=Html.DisplayFor(mdl => mdl.Info.ID) %>
        </p>

        <p>
        <%=Html.LabelFor(mdl => mdl.Info.Naam) %>
        <%=Html.DisplayFor(mdl => mdl.Info.Naam) %>
        </p>

        <p>
        <%=Html.LabelFor(mdl => mdl.Info.Plaats) %>
        <%=Html.DisplayFor(mdl => mdl.Info.Plaats) %>
        </p>
       
        <p>
        <%=Html.LabelFor(mdl => mdl.Info.StamNummer) %>
        <%=Html.DisplayFor(mdl => mdl.Info.StamNummer) %>             
        </p>
        
    </fieldset>

    <h3>Actieve afdelingen dit werkjaar</h3>
    
    <ul>
    <%
        foreach (var afd in Model.Info.AfdelingenDitWerkJaar.OrderByDescending(afd=>afd.GeboorteJaarVan))
        {
            %>
            <li><%=Html.Encode(String.Format("{0} - {1} ({2})", afd.Afkorting, afd.Naam, afd.OfficieleAfdelingNaam)) %></li>
            <%
        }
    %>
            <li>[<%=Html.ActionLink("afdelingsverdeling aanpassen", "Index", "Afdeling") %>]</li>
    </ul>      

    <h3>Persoonscategorie&euml;n</h3>

    <ul>
    <%
        foreach (var cat in Model.Info.Categorie.OrderBy(cat=>cat.Code))
        {
            %>
            <li><%=Html.Encode(String.Format("{0} - {1}", cat.Code, cat.Naam)) %></li>
            <%
        }
    %>
    </ul>  

<% using (Html.BeginForm())
   { %>
    <fieldset>
    <legend>Categorie toevoegen</legend>
    <p>
    <%=Html.LabelFor(mdl=>mdl.NieuweCategorie.Naam) %>
    <%=Html.EditorFor(mdl=>mdl.NieuweCategorie.Naam) %>
    <%=Html.ValidationMessageFor(mdl=>mdl.NieuweCategorie.Naam) %>
    </p>
    <p>
    <%=Html.LabelFor(mdl=>mdl.NieuweCategorie.Code) %>
    <%=Html.EditorFor(mdl=>mdl.NieuweCategorie.Code) %>
    <%=Html.ValidationMessageFor(mdl=>mdl.NieuweCategorie.Code) %>
    </p>
    <p>
    <input type="submit" value="Bewaren"/>
    </p>
    </fieldset>        
<%} %>



</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
