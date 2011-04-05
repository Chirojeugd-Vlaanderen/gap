<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.UitstapOverzichtModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="kaderke">
    <div class="kadertitel">Overzicht uitstappen</div>
    [<%=Html.ActionLink("nieuwe uitstap/bivak", "Nieuw", "Uitstappen") %>]
    </div>



</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
