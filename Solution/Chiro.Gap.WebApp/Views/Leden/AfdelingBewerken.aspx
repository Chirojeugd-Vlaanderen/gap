<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<LidAfdelingenModel>" %>
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
              if (Model.Info.Type == LidType.Leiding)
              {
                  %>
                  <p>Selecteer afdeling(en) voor <%= Model.Info.VolledigeNaam %></p>
                  <%
                  
                  List<CheckBoxListInfo> info =
                      (from pa in Model.BeschikbareAfdelingen
                       select new CheckBoxListInfo(
                                       pa.AfdelingsJaarID.ToString()
                                       , pa.Naam
                                       , Model.Info.AfdelingsJaarIDs.Contains(pa.AfdelingsJaarID))).ToList<CheckBoxListInfo>();

                  Response.Write(Html.CheckBoxList("Info.AfdelingsJaarIDs", info));
              }
              else
              {
                  %>
                  <p>Selecteer de afdeling voor <%= Model.Info.VolledigeNaam %></p>
                  <%
                  foreach(var ai in Model.BeschikbareAfdelingen)
                  {
                      Response.Write("<p>" + Html.RadioButton("Info.AfdelingsJaarIDs[0]", ai.AfdelingsJaarID, Model.Info.AfdelingsJaarIDs[0] == ai.AfdelingsJaarID)+ ai.Naam + "</p>");   
                  }
              }
           %>
           
           
           <%= Html.Hidden("HuidigLid.LidID")%>
           <%= Html.Hidden("HuidigLid.PersoonInfo.GelieerdePersoonID") %>
           <%= Html.Hidden("HuidigLid.Type")%>
           </fieldset>
           
           <%
        } %>
       
 

</asp:Content>
