// <copyright company="Chirojeugd-Vlaanderen vzw">
// Copyright (c) 2009-2011
// Mail naar informatica@chiro.be voor alle info over deze broncode
// </copyright>

using System.Net.Mail;
using System.Runtime.Serialization;
using System.ServiceModel;

[ServiceContract]
public interface IService
{
	[OperationContract]
	string LoginAanvragen(int adnr, string voornaam, string naam, MailAddress mailadres);
}