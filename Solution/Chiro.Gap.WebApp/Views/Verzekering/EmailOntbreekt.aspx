<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.MasterViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
    
    <h2>Stel je e-mailadres in</h2>
    
    <p class="error">
        Om ongevalsaangiften in te dienen of op te volgen, moet je je
        <%=Html.ActionLink("e-mailadres instellen", "MijnGegevens", "Personen") %>.
    </p>

</asp:Content>
