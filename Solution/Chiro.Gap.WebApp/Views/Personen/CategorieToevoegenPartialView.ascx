<%@ Control Language="C#" Inherits="ViewPage<Chiro.Gap.WebApp.Models.CategorieModel>" %>

<div class="test">
	<ul id="acties">
		<li>
			<input type="submit" value="Bewaren" />
		</li>
	</ul>
    
	<fieldset>
		<legend>Aan welke categorieën wil je
			<%= Model.GelieerdePersoonIDs.Count == 1 ? "hem/haar" : "hen" %>
			toevoegen?</legend>
		
		Personen om aan de categorie toe te voegen:<br />
		<ul>
		<%foreach (var p in Model.GelieerdePersoonNamen){ %>	
			<li><%=p %><br/></li>
		<%} %>
		</ul>
		
		<%
			List<CheckBoxListInfo> info
			   = (from pa in Model.Categorieen
				  select new CheckBoxListInfo(
					 pa.ID.ToString()
					 , pa.Naam
					 , false)).ToList();
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
    </div>