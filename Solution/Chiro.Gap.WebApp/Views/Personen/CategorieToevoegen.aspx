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
           <legend>Aan welke categorieen wil je <%= Model.GelieerdePersonenIDs.Count == 1 ? "hem/haar" : "hen" %> toevoegen?</legend>     
           
           <!--<%=Html.DropDownList("Model.selectie", new SelectList(Model.Categorieen.Select(x => new { value = x.ID, text = x.Naam }), "value", "text"))%>-->
           
           <%
                List<CheckBoxListInfo> info
                   = (from pa in Model.Categorieen
                      select new CheckBoxListInfo(
                         pa.ID.ToString()
                         , pa.Naam
                         , false)).ToList<CheckBoxListInfo>();
            %>
           
           <%= Html.CheckBoxList("GeselecteerdeCategorieen", info) %>
 
           <% //Html.Hidden("GelieerdePersonenIDs"); 
           foreach(int i in Model.GelieerdePersonenIDs)
           {
               Response.Write("<input type=\"hidden\" name=\"GelieerdePersonenIDs\" value=\"" + i + "\"/>");
           }%>
           </fieldset>
           
           <%
        } %>
       
 

</asp:Content>
