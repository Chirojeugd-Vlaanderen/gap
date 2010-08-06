<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Fout.Master" Inherits="ViewPage<HandleErrorInfo>" %>
<%@ Import Namespace="Chiro.Gap.Domain" %>
<%@ Import Namespace="Chiro.Gap.ServiceContracts.FaultContracts" %>
<%@ Import Namespace="System.Net" %>
<%@ Import Namespace="System.ServiceModel" %>
<asp:Content ID="errorContent" ContentPlaceHolderID="MainContent" runat="server">
	<%
		String boodschap = string.Empty;
		if (Model != null)
		{
			if (Model.Exception is FaultException<FoutNummerFault>)
			{
				var ex = (FaultException<FoutNummerFault>)Model.Exception;
				switch (ex.Detail.FoutNummer)
				{
					case FoutNummer.GeenGav:
						Html.RenderPartial("GeenToegangControl");
						break;
					case FoutNummer.GeenDatabaseVerbinding:
						Html.RenderPartial("GeenVerbindingControl");
						break;
					default:
						boodschap = "Er is iets foutgelopen, maar het is niet helemaal duidelijk wat.";
						break;
				}
			}
			else if (Model.Exception is FaultException<ExceptionDetail>)
			{
				boodschap = "Tijdens je bewerking is er iets foutgelopen, waarschijnlijk is ze mislukt. Kijk voor de zekerheid je gegevens eens na.";
				// boodschap = Model.Exception.Message; // Hebben alle FaultExceptions aangepaste foutmeldingen? => Nee: fouten in DAL of servicelayer niet
			}
			else if (Model.Exception is CommunicationObjectFaultedException)
			{
				boodschap = "Tijdens je bewerking is er iets foutgelopen, waarschijnlijk is ze mislukt. Kijk voor de zekerheid je gegevens eens na.";
			}
			else if (Model.Exception is ArgumentException)
			{
				boodschap = "Foute gegevens doorgegeven.";
			}
			else
			{
				boodschap = "Er is iets foutgelopen, maar het is niet helemaal duidelijk wat.";
			}

			if (boodschap != string.Empty)
			{
				boodschap += String.Format(" (foutcode {0})", Response.StatusCode);
			}
		}
		else
		{
			// Er is iets foutgelopen, maar we hebben geen foutgegevens ter beschikking. Dat gebeurt typisch
			// bij niet-afgehandelde fouten waarvoor de defaultRedirect gebruikt wordt. In dat geval bevat de url
			// wel een parameter 'aspxerrorpath'.
			switch (Response.StatusCode)
			{
				case (int)HttpStatusCode.NotFound:
					boodschap = "De pagina die je opvroeg, bestaat niet of de gegevens zijn niet beschikbaar.";
					break;
				case (int)HttpStatusCode.InternalServerError:
					boodschap = "Er is intern iets foutgegaan.";
					break;
				case (int)HttpStatusCode.ServiceUnavailable:
					Html.RenderPartial("GeenVerbindingControl");
					break;
				case (int)HttpStatusCode.Forbidden:
					Html.RenderPartial("GeenToegangControl");
					break;
				default:
					boodschap = "Er is iets foutgegaan maar het is niet duidelijk wat.";
					break;
			}
		}

		if (boodschap != string.Empty)
		{	%>
	<div class="Foutmelding">
		<%=boodschap %>
	</div>
	<% } %>
</asp:Content>