<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<LedenModel>" %>
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
           <legend>Afdelingen</legend>     
           
          <%
              if (Model.HuidigLid.Type == LidType.Leiding)
              {
                  %>
                  <p>Selecteer de afdeling(en) van <%= Model.HuidigLid.PersoonInfo.VolledigeNaam %></p>
                  <%
                  
                  List<CheckBoxListInfo> info =
                      (from pa in Model.AlleAfdelingen
                       select new CheckBoxListInfo(
                                       pa.AfdelingID.ToString()
                                       , pa.Naam
                                       , Model.AfdelingIDs.Contains(pa.AfdelingID))).ToList<CheckBoxListInfo>();

                  Response.Write(Html.CheckBoxList("AfdelingIDs", info));
              }
              else
              {
                  %>
                  <p>Selecteer de afdeling van <%= Model.HuidigLid.PersoonInfo.VolledigeNaam %></p>
                  <%
                  foreach(var ai in Model.AlleAfdelingen)
                  {
                      Response.Write("<p>" + Html.RadioButton("AfdelingID", ai.AfdelingID, Model.HuidigLid.AfdelingIdLijst.ElementAt(0) == ai.AfdelingID)+ ai.Naam + "</p>");   
                  }
              }
           %>
           
           
           <%= Html.Hidden("HuidigLid.LidID")%>
           <%= Html.Hidden("HuidigLid.Type")%>
           </fieldset>
           
           <%
        } %>
       
 

</asp:Content>
