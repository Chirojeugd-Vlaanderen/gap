<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Chiro.Gap.WebApp.Models.IAdresBewerkenModel>" %>

        <p>
            <%=Html.LabelFor(mdl => mdl.Land) %>
            <%=Html.DropDownListFor(mdl => mdl.Land, new SelectList(Model.AlleLanden, "Naam", "Naam")) %>
        </p>

		<p>
			<%=Html.LabelFor(mdl => mdl.PostNr) %>
			<%=Html.EditorFor(mdl => mdl.PostNr)%>
			<%=Html.ValidationMessageFor(mdl => mdl.PostNr)%>
        </p>

        <p class="buitenland">
			<%=Html.LabelFor(mdl => mdl.PostCode) %>
			<%=Html.EditorFor(mdl => mdl.PostCode)%>
			<%=Html.ValidationMessageFor(mdl => mdl.PostCode)%>            
        </p>

		<noscript>
			<input type="submit" name="action" value="Woonplaatsen ophalen" />
		</noscript>
		<p>
			<%=Html.LabelFor(mdl => mdl.Straat)%>
			<%=Html.EditorFor(mdl => mdl.Straat)%>
			<%=Html.ValidationMessageFor(mdl => mdl.Straat)%>
		</p>
		<p>
			<%=Html.LabelFor(mdl => mdl.HuisNr)%>
			<%=Html.EditorFor(mdl => mdl.HuisNr)%>
			<%=Html.ValidationMessageFor(mdl => mdl.HuisNr)%>
		</p>
		<p>
			<%=Html.LabelFor(mdl => mdl.Bus)%>
			<%=Html.EditorFor(mdl => mdl.Bus)%>
			<%=Html.ValidationMessageFor(mdl => mdl.Bus)%>
		</p>
		<p class="binnenland">
			<%=Html.LabelFor(mdl => mdl.WoonPlaats)%>
			<%=Html.DropDownListFor(mdl => mdl.WoonPlaats, new SelectList(Model.BeschikbareWoonPlaatsen, "Naam", "Naam"))%>
			<%=Html.ValidationMessageFor(mdl => mdl.WoonPlaats)%>
        </p>
        <p class="buitenland">
			<%=Html.LabelFor(mdl => mdl.WoonPlaatsBuitenLand)%>
			<%=Html.EditorFor(mdl => mdl.WoonPlaatsBuitenLand)%>
			<%=Html.ValidationMessageFor(mdl => mdl.WoonPlaatsBuitenLand)%>
		</p>