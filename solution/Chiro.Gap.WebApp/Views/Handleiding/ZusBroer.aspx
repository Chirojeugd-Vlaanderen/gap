<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Handleiding.Master"
    Inherits="System.Web.Mvc.ViewPage<HandleidingModel>" %>

<%@ Import Namespace="Chiro.Gap.WebApp.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="title" runat="server">
    Handleiding: Zus/broer toevoegen
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HelpContent" runat="server">
    <h2>
        Gegevens van zus of broer toevoegen</h2>
    <p>
        Als je verschillende kinderen uit hetzelfde gezin in je groep hebt, is het vervelend
        als je alle adres- en telefoongegevens dubbel moet invullen. Goed nieuws: dankzij
        de link 'zus/broer maken' is dat niet meer nodig!</p>
    <p>
        Let wel: die procedure legt geen link tussen personen. Het kopieert alleen de gezinsgebonden
        communicatievormen van een bestaande naar een nieuwe persoon. De enige link die
        voor GAP bestaat tussen personen, is dat ze op hetzelfde adres wonen. Het is dus
        niet nodig om van bestaande personen aan te geven dat ze broer of zus zijn van elkaar
        - en dat is trouwens ook niet mogelijk. Als je achteraf nog telefoonnummers en/of
        mailadressen toevoegt, vink dan aan dat ze 'Voor het hele gezin' zijn: dan worden
        ze gekopieerd naar iedereen die op hetzelfde adres woont.</p>
    <p>
        Stappen in het proces:</p>
    <ul>
        <li>Klik op het tabblad 'Iedereen'.</li>
        <li>Zoek de persoon voor wie je een broer of zus wilt toevoegen. Ga eventueel nog na
            of het thuisadres en bijvoorbeeld het telefoonnummer van de ouders ingevuld is.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Link_Broer_of_zus.png") %>" alt="Gegevens voor een zus of broer toevoegen" />
    <ul>
        <li>Klik op de link 'zus/broer maken'. Je gaat dan naar het scherm waar je een nieuwe
            persoon aanmaakt, en de familienaam is al ingevuld.</li>
    </ul>
    <img src="<%=ResolveUrl("~/Content/Screenshots/Formulier_Broer_zus.png") %>" alt="Gegevens voor een zus of broer toevoegen" />
    <ul>
        <li>Vul de ontbrekende gegevens in (en pas eventueel de familienaam aan als het over
            een nieuwsamengesteld gezin gaat). Klik op de knop 'Bewaren'.</li>
        <li>Nu zit je op de persoonsfiche. Daar kun je zien dat het thuisadres en alle gezinsgebonden
            communicatievormen overgenomen zijn van de oorspronkelijke persoon.</li>
    </ul>
</asp:Content>
