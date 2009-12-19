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
                      (from pa in Model.AfdelingsInfoDictionary
                       select new CheckBoxListInfo(
                                       pa.Value.ID.ToString()
                                       , pa.Value.Naam
                                       , Model.AfdelingIDs.Contains(pa.Value.ID))).ToList<CheckBoxListInfo>();

                  Response.Write(Html.CheckBoxList("AfdelingIDs", info));
              }
              else
              {
                  %>
                  <p>Selecteer de afdeling van <%= Model.HuidigLid.PersoonInfo.VolledigeNaam %></p>
                  <%
                  foreach(var ai in Model.AfdelingsInfoDictionary)
                  {
                      Response.Write("<p>" + Html.RadioButton("AfdelingID", ai.Value.ID, Model.HuidigLid.AfdelingIdLijst.ElementAt(0) == ai.Value.ID)+ ai.Value.Naam + "</p>");   
                  }
              }
           %>
           
           
           <%= Html.Hidden("HuidigLid.LidID")%>
           <%= Html.Hidden("HuidigLid.Type")%>
           </fieldset>
           
           <%
        } %>
       
 

</asp:Content>
