<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Chiro.Gap.WebApp.Models.CategorieModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%= ResolveUrl("~/Scripts/jquery-1.3.2.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/jquery.validate.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcJQueryValidation.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftAjax.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcAjax.js")%>" type="text/javascript"></script>
    <script src="<%= ResolveUrl("~/Scripts/MicrosoftMvcValidation.js")%>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  
    <%
        Html.EnableClientValidation();
        using (Html.BeginForm())
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
