<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.UitstapModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

	<% 
        // Deze view kan zowel gebruikt worden voor het aanmaken van nieuwe uitstappen, als voor het bewerken
        // van bestaande
        
		Html.EnableClientValidation(); // Deze instructie moet (enkel) voor de BeginForm komen
        using (Html.BeginForm())
                {%>

                	<ul id="acties">
                    <li><input type="submit" value="Bewaren" /></li>
                    </ul>

                    <fieldset>
                    <legend>Info over uitstap of bivak</legend>
                    
                    <%=Html.HiddenFor(mdl=>mdl.Uitstap.ID) %>
                    <%=Html.HiddenFor(mdl=>mdl.Uitstap.VersieString) %>

                    <%=Html.LabelFor(mdl=>mdl.Uitstap.Naam) %>
                    <%=Html.EditorFor(mdl => mdl.Uitstap.Naam) %>
                    <%=Html.ValidationMessageFor(mdl => mdl.Uitstap.Naam) %><br />

                    <%=Html.LabelFor(mdl=>mdl.Uitstap.IsBivak) %>
                    <%=Html.CheckBoxFor(mdl => mdl.Uitstap.IsBivak) %><br />

                    <%=Html.LabelFor(mdl=>mdl.Uitstap.DatumVan) %>
                    <%=Html.EditorFor(mdl => mdl.Uitstap.DatumVan) %>
                    <%=Html.ValidationMessageFor(mdl => mdl.Uitstap.DatumVan) %><br />

                    <%=Html.LabelFor(mdl=>mdl.Uitstap.DatumTot) %>
                    <%=Html.EditorFor(mdl => mdl.Uitstap.DatumTot) %>
                    <%=Html.ValidationMessageFor(mdl => mdl.Uitstap.DatumTot) %><br />

                    <%=Html.LabelFor(mdl=>mdl.Uitstap.Opmerkingen) %>
                    <%=Html.EditorFor(mdl => mdl.Uitstap.Opmerkingen) %>
                    <%=Html.ValidationMessageFor(mdl => mdl.Uitstap.Opmerkingen) %><br />

                    </fieldset>
                    
                <%}%>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
