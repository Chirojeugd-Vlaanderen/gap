<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CategorieModel>" %>
<%@ Import Namespace="Chiro.Gap.Orm" %>
<%@ Import Namespace="Chiro.Gap.WebApp" %>
<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
    <% using (Html.BeginForm())
       {
           %>
           
           <ul id="acties">
           <li><input type="submit" value="Bewaren" /></li>
           </ul>
           
           <fieldset>
           <legend>Aan welke categorieën wil je <%= Model.GelieerdePersoonIDs.Count == 1 ? "hem/haar" : "hen" %> toevoegen?</legend>     
                 
           <%
                List<CheckBoxListInfo> info
                   = (from pa in Model.Categorieen
                      select new CheckBoxListInfo(
                         pa.ID.ToString()
                         , pa.Naam
                         , false)).ToList<CheckBoxListInfo>();
            %>
           
           <%= Html.CheckBoxList("GeselecteerdeCategorieIDs", info) %>
 
           <% 
               foreach (int id in Model.GelieerdePersoonIDs)
               {
                   %>
                   <input type="hidden" name="GelieerdePersoonIDs" value="<%=id %>" />
                   <%
               }
           %>
           </fieldset>
           
           <%
        } %>
       
 

</asp:Content>
