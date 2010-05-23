<%@ Control Language="C#"%>

<div>
    <% 
    //  De link om terug te keren naar de laatste lijst moet alleen getoond worden als we effectief van een lijst komen. 
    //  Welke lijst dat is, wordt door de controllers in de ClientState opgeslagen.
    if (Chiro.Gap.WebApp.ClientState.VorigePagina != null) %>
    <%  { %>
    <%//=Html.("Terug naar vorig overzicht", Chiro.Gap.WebApp.ClientState.VorigePagina)%>
    <a href="<%=Chiro.Gap.WebApp.ClientState.VorigePagina%>">Terug naar vorig overzicht</a>
    <%  } %>
</div>
