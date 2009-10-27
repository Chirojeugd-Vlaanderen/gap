<%@ Control Language="C#"%>
<%@ Import Namespace="Chiro.Gap.WebApp"%>

<div>
    <% 
        //  De link om terug te keren naar de laatste lijst moet alleen getoond worden als we effectief van een lijst komen. 
        //  Welke lijst dat is, wordt door de controllers in de sessie opgeslagen.
        if (Chiro.Gap.WebApp.Sessie.LaatsteLijst != String.Empty)
        {
    %>
    <%=Html.ActionLink("Terug naar de lijst", "List", new { Controller = Chiro.Gap.WebApp.Sessie.LaatsteLijst, id = Chiro.Gap.WebApp.Sessie.LaatstePagina })%>
    <% } %>
</div>
