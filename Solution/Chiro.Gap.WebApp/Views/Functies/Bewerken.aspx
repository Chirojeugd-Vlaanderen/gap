<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.FunctieModel>" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div id="bewerken">
    <h2>Bewerken</h2>
    
    <% using (Html.BeginForm())
       { %>
        <table>
            <tr>
                <td><%=Html.LabelFor(xx => xx.HuidigeFunctie.Naam) %> </td>
                <td><%=Html.EditorFor(mdl => mdl.HuidigeFunctie.Naam) %></td>
                <td><%=Html.ValidationMessageFor(mdl => mdl.HuidigeFunctie.Naam) %></td>
            </tr>
            <tr>
                <td><%=Html.LabelFor(xx => xx.HuidigeFunctie.Code) %></td> 
                <td><%=Html.EditorFor(mdl => mdl.HuidigeFunctie.Code) %></td>
                <td><%=Html.ValidationMessageFor(mdl => mdl.HuidigeFunctie.Code) %></td>
                <%=Html.HiddenFor(mdl => mdl.HuidigeFunctie.ID) %>  
            </tr>
            <tr>
                <td><%=Html.LabelFor(xx => xx.HuidigeFunctie.MaxAantal) %> </td>
                <td><%=Html.EditorFor(mdl => mdl.HuidigeFunctie.MaxAantal)%></td>
                <td><%=Html.ValidationMessageFor(mdl => mdl.HuidigeFunctie.MaxAantal) %></td>
            </tr>
            <tr>
                <td><%=Html.LabelFor(xx => xx.HuidigeFunctie.MinAantal) %> </td>
                <td><%=Html.EditorFor(mdl => mdl.HuidigeFunctie.MinAantal)%></td>
                <td><%=Html.ValidationMessageFor(mdl => mdl.HuidigeFunctie.MinAantal) %></td>
            </tr>
			<tr>
			    <td><%=Html.LabelFor(mdl => mdl.HuidigeFunctie.Type)%></td>
                <td>
                    <%var values = from LidType lt in Enum.GetValues(typeof (LidType))
                     where lt != LidType.Geen
			         select new{
	                        value = lt,
	                        text = String.Format(
	                    	    "Ingeschreven {0}",
	                    	    lt == LidType.Kind ? "leden" : lt == LidType.Leiding ? "leiding" : "leden en leiding")
	                    };%>
			         <%=Html.DropDownListFor(mdl => mdl.HuidigeFunctie.Type,new SelectList(values.Reverse(), "value", "text"))%>
                 </td>
                 <td><%=Html.ValidationMessageFor(mdl => mdl.HuidigeFunctie.Type) %></td>
			</tr>
        </table>
    <input type="submit" value="Bewaren" id="bewaarFunctie"/>

    <% } %>
    
</div>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
</asp:Content>
