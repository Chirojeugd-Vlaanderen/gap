<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GroepenLijstControl.ascx.cs" Inherits="WebApp.Views.Groepen.GroepenLijstControl" %>

<%@ Import Namespace="Cg2.Core.Domain" %>

<asp:DataList ID="groepenDataLijst" runat="server">
<ItemTemplate>
<table>
    <tr>
        <td><strong>ID:</strong></td>
        <td><%# ((Groep)(Container.DataItem)).ID %></td>
    </tr>
    <tr>
        <td><strong>Code:</strong></td>
        <td><%# ((Groep)(Container.DataItem)).Code %></td>
    </tr>
    <tr>
        <td><strong>Naam:</strong></td>
        <td><%# ((Groep)(Container.DataItem)).Naam %></td>
    </tr>
    <tr>
        <td><strong>Oprichtingsjaar:</strong></td>
        <td><%# ((Groep)(Container.DataItem)).OprichtingsJaar %></td>
    </tr>
    <tr>
        <td><strong>Website:</strong></td>
        <td><%# ((Groep)(Container.DataItem)).WebSite %></td>
    </tr>
   
</table>
</ItemTemplate>
</asp:DataList>