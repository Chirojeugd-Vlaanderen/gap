<%@ Control Language="C#"%>
<%@ Import Namespace="MvcWebApp2"%>

<div>
    <% 
        //  De link om terug te keren naar de laatste lijst moet alleen getoond worden als we effectief van een lijst komen. 
        //  Welke lijst dat is, wordt door de controllers in de sessie opgeslagen.
        if (MvcWebApp2.Sessie.LaatsteLijst != String.Empty)
        {
    %>
    <%=Html.ActionLink("Terug naar de lijst", "List", new { Controller = MvcWebApp2.Sessie.LaatsteLijst, id = MvcWebApp2.Sessie.LaatstePagina })%>
    <% } %>
</div>
