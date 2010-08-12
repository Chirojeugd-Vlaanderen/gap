<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master" Inherits="ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1"  ContentPlaceHolderID="helpContent" runat="server">
	<%=Model.HelpBestand %>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="title"  runat="server">
	<%=Model.Titel %>
</asp:Content>
