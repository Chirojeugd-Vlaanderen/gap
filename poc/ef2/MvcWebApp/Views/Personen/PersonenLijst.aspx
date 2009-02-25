<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="PersonenLijst.aspx.cs" Inherits="MvcWebApp.Views.Lijst.PersonenLijst" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <form id="form1" action="<%= Url.Action("PersonenLijst", "Personen")%>" method="post">
    <% Html.RenderPartial("PersonenLijstUserControl"); %>
    
    <p>
        <input type="submit" value="Lid maken!" />
    </p>
    </form>
</asp:Content>
