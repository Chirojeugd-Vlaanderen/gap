<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Fout.Master" Inherits="System.Web.Mvc.ViewPage<System.Web.Mvc.HandleErrorInfo>" %>
<%@ Import Namespace="System.ServiceModel" %>
<%@ Import Namespace="System.Net" %>
<asp:Content ID="errorContent" ContentPlaceHolderID="MainContent" runat="server">
	<%
		String boodschap;
		if (Model != null)
		{
			if (Model.Exception.GetType() == typeof(FaultException<ExceptionDetail>))
			{
				boodschap = Model.Exception.Message; // Hebben alle FaultExceptions aangepaste foutmeldingen? => Nee: fouten in DAL of servicelayer niet
			}
			else if (Model.Exception.GetType() == typeof(CommunicationObjectFaultedException))
			{
				boodschap = "De service die gegevens ophaalt, is momenteel niet beschikbaar.";
			}
			else if (Model.Exception.GetType() == typeof(ArgumentException))
			{
				boodschap = "Foute gegevens doorgegeven.";
			}
			else
			{
				boodschap = "Er is iets foutgelopen, maar het is niet helemaal duidelijk wat.";
			}

			boodschap += String.Format(" ({0})", Response.StatusCode);
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
				default:
					boodschap = "Er is iets foutgegaan maar het is niet duidelijk wat.";
					break;
			}
		}
	%>
	<div class="Foutmelding">
		<%=boodschap %>
	</div>
</asp:Content>
