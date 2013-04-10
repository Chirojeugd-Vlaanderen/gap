/*
Frontend javascript (JQuery)
GAP, Chirojeugd Vlaanderen
2013
*/

//--------------------------------------------------------------------------------
//Javascript dat een script-tag genereert waarin het script 'algemeneFuncties.js'
//wordt aangeroepen.
//--------------------------------------------------------------------------------

var script = document.createElement("script");
script.type = "text/javascript";
script.src = "/Scripts/algemeneFuncties.js";
document.body.appendChild(script);

//--------------------------------------------------------------------------------
//Document ready functie
//--------------------------------------------------------------------------------
$(function () {
    //Tabel layout
    $('table tr:even').css("background-color", "lightGray");

    //settings voor icoontjes bij adres
    /*'bewerk' iconen
    $('tr').find('td:eq(2)')
    .not($('#ad, #instap'))
    .append("<div class=\"ui-icon ui-icon-pencil\" title=\"Bewerken\" style=\"cursor: pointer\"></div>");
    // 'Verwijderen' iconen
    $('.persoonlijkeGegevens tr > td:last-child')
    .not($('#naamIconen, #gdIconen, #geslachtIconen'))
    .append("<div class=\"ui-icon ui-icon-circle-minus\" title=\"Verwijderen\" id=\"adrVerw\" style=\"cursor: pointer\"></div>");
    //'toevoegen' iconen
    $('table #adressen:last td:last, table #email:last td:last,table #tel:last td:last')
    .append("<div class=\"ui-icon ui-icon-circle-plus\" title=\"Toevoegen\" id=\"adrToev\" style=\"cursor: pointer\"></div>");
    */
    $('.ui-icon').tooltip();

    //-------------------------------------------------------------------------
    //inline editing
    //TODO: Ervoor zorgen dat de ingevulde gegevens ook daadwerkelijk opgeslagen worden.
    //-------------------------------------------------------------------------

    //defaults
    $.fn.editable.defaults.mode = 'inline';
    $.fn.editable.defaults.clear = true;
    $.fn.editable.defaults.toggle = 'manual';

    $.datepicker.setDefaults.changeMonth = false;

    //variabelen
    var GID = $('#groepID').val();
    var id = $('#lidIdH').val();
    var GPid = $('#GPid').val();

    //naam veranderen
    $('#naamInfo').editable({
        validate: function (value) {
            if ($.trim(value) == '') return 'Dit veld moet ingevuld worden!';
        }
    })
    .on('save', function (e) {
        e.preventDefault();
        var url = "/" + GID + "/Personen/EditGegevens/" + GPid;
        $.get(url, function () {
            success:
            {
                location.reload();
            }
        });
    });

    $('#bewerkNaam')
        .click(function (e) {
            e.stopPropagation();
            e.preventDefault();
            $('#naamInfo').editable('toggle');
        });

    $('#bewerkGeslacht').click(function (e) {
        e.preventDefault();
        var g = $('#geslachtsInfo').text();
        if (g == 'Man') {
            $('#geslachtsInfo').text('Vrouw');
        } else {
            $('#geslachtsInfo').text('Man');
        }
    });

    //contact gegevens (tel/email)
    $('.contact').editable();

    $('.contactBewerken').click(function (e) {
        e.stopPropagation();
        e.preventDefault();
        //TODO: validatie (email adres - tel) 
        $(this).parent().parent().find('td:eq(1)').editable('toggle');
    });

    //geboortedatum

    //Adres
    //verster wordt getoond bij het wijzigen van een adres, gegevens worden automatisch ingevuld
    // (uitgelezen uit de hiddenfields op de pagina)
    $('#bewerkAdres').click(function (e) {
        e.preventDefault();
        var straatnaam = $('#strH').val();
        var huisnummer = $('#hsnrH').val();
        var postcode = $('#pstcdH').val();
        var bus = $('#busH').val();

        $('#straatnaam').val(straatnaam);
        $('#huisnr').val(huisnummer);
        $('#postcode').val(postcode);
        $('#bus').val(bus);

        toonGemeenten(postcode);

        $('#adresDialog').dialog({
            modal: true,
            title: "Adres wijzigen",
            buttons: {
                "Wijzig adres": function () {
                    //TODO: Adres opslaan
                },
                "Annuleren": function () {
                    $(this).dialog("close");
                }
            }
        });
    });

    $('#postcode').on('change', function () {
        var postcode = $(this).val();
        toonGemeenten(postcode);
    });

    //Chiroleeftijd
    $('#bewerkCl').click(function (e) {
        e.stopPropagation();
        e.preventDefault();
        $('#chiroleeftijdInfo').editable('toggle');
    });

    $('#chiroleeftijdInfo').editable({
        source: [
                { value: 1, text: '0' },
                { value: 2, text: '1' },
                { value: 3, text: '2' },
                { value: 4, text: '3' },
                { value: 5, text: '4' }
            ],
        showbuttons: false
    });

    //Ingeschreven als (lid/ leiding)
    $('#bewerkLidInfo').click(function (e) {
        e.stopPropagation();
        e.preventDefault();
        var url = "/" + GID + "/Leden/TypeToggle/" + id;
        $.post(url, { id: id, groepID: GID }, function () {
            success:
            {
                location.reload();
            }
        });
        //Throws  not inplemented
    });

    $('#lidInfoInfo').editable({
        source: [
                { value: 1, text: 'Lid' },
                { value: 2, text: 'Leiding' }
            ],
        showbuttons: false
    });

    // afdeling bewerken
    //TODO: Ophalen afdelingsnamen (sommige groepen gebruiken niet de officieële namen)
    $('#bewerkAfdeling').click(function (e) {
        e.stopPropagation();
        e.preventDefault();
        $('#afdelingInfo').editable('toggle');
    });

    $('#afdelingInfo').editable({
        source: [
                { value: 1, text: 'Ribbels' },
                { value: 2, text: 'Speelclub' },
                { value: 3, text: "Rakwi's" },
                { value: 4, text: "Tito's" },
                { value: 5, text: "Keti's" },
                { value: 6, text: "Aspi's" }
            ],
        showbuttons: false
    });

    //lidgeld Toggle

    $('#bewerkLidgeld').click(function (e) {
        e.preventDefault();
        var url = "/" + GID + "/Leden/LidGeldToggle/" + id;
        $.post(url, { id: id, groepID: GID }, function () {
            success:
            {
                location.reload();
            }
        });
    });

    //functies bewerken
    $('#bewerkFuncties').click(function (e) {
        e.preventDefault();

        var url = "/" + GID + "/Leden/FunctiesToekennen/" + id + " " + "#functies";
        $('#extraInfoDialog').load(url);
        $('#extraInfoDialog').dialog({
            modal: true,
            width: 500,
            title: "Functies bewerken",
            buttons: {
                "Bevestigen": function () {

                    /*TODO: Values van gecheckte functies doorgeven}*/
                    $(this).dialog('close');
                },
                "Annuleren": function () {
                    $(this).dialog('close');
                }
            }
        });
    });


    // verzekering tegen loonverlies
    $('#bewerkVerzekering').click(function (e) {
        e.preventDefault();
        var url = "/" + GID + "/Leden/LoonVerliesVerzekeren/" + id + " " + "#ver";
        $('#bewerkVerzekeringDialog').load(url);

        $('#bewerkVerzekeringDialog').dialog({
            modal: true,
            title: "Verzekeren tegen loonverlies",
            width: 500,
            buttons: {
                "Bevestigen": function () {
                    var url = "/" + GID + "/Leden/LoonVerliesVerzekeren/" + id;

                    $.post(url, function (data) {
                        $(this).html(data);
                    });
                },
                "Annuleren": function () {
                    $(this).dialog('close');
                }
            }

        });
    });


    //-------------------------------------------------------------------------
    //Einde inline editing
    //-------------------------------------------------------------------------


    //buttons maken
    $('#print').button({
        icons: {
            primary: "ui-icon-print"
        }
    });
    $('#loginMaken, #gbrToekennen').button({
        icons: {
            primary: "ui-icon-wrench"
        }
    });

    $('#loginMaken').click(function () {
        var url = "/" + GID + "/GebruikersRecht/LoginMaken/" + GPid;
        $.post(url);
    });

    $('#gbrToekennen').click(function () {
        var url = "/" + GID + "/GebruikersRecht/AanGpToekennen/" + GPid;
        $.post(url);
    });
    // Categoriën verwijderen / Toevoegen

    $('#catVerw').click(function (e) {
        e.preventDefault();
        var catID = $('#catID').val();
        var url = "/" + GID + "/Personen/VerwijderenCategorie";

        $.post(url, { categorieID: catID, gelieerdePersoonID: GPid, groepID: GID }, function () {
            success:
            {
                location.reload();
            }
        });

    });

    $('#catToev').click(function () {
        var url = "/" + GID + "/Personen/ToevoegenAanCategorie";
        $.post(url, { gelieerdePersoonID: GPid }, function (data) {
            //TODO: Oplossen want dieje nest is verkeerd. Hele pagina wordt geladen
            $('#extraInfoDialog').load("/" + GID + "/Personen/CategorieToevoegenPartialView")
                .dialog({
                    modal: true,
                    width: 600,
                    title: "Toevoegen aan categorie"
                });

        });

    });

    //=Html.ActionLink("[verwijderen]", "VerwijderenCategorie", new { categorieID = info.ID, gelieerdePersoonID = ViewData.Model.PersoonLidInfo.PersoonDetail.GelieerdePersoonID })%>

    //Info over verzekering tegen loonsverlies
    $('#loonVerlies').click(function () {
        var url = "/Handleiding/VerzekeringLoonverlies #kort";
        $('#extraInfoDialog').load(url, function (response, status, xhr) {
            if (status == "success") {
                $('#extraInfoDialog').append(
                    "(Kostprijs: € 2,38) <br />" +
                    "<a href=\"/Handleiding/VerzekeringLoonverlies\" target=\"_new\" >Meer</a>");
            }
        });

        $('#extraInfoDialog').dialog({
            modal: true,
            width: 500,
            show: {
                effect: "drop"
            },
            hide: {
                effect: "drop"
            },
            title: "Verzekering tegen loonverlies"
        });

    });
    // Einde verzeking loonsverlies



    // Extra info weergeven
    $('#vkAdres').click(function () {
        toonInfo('#VK-ADRINFO', "Voorkeurs adres", "#extraInfoDialog");
    });

    $('#verhuizen').click(function () {
        var url = "/Handleiding/Verhuizen #verhuizen";
        $('#extraInfoDialog').load(url, function (response, status, xhr) {
        });

        $('#extraInfoDialog').dialog({
            modal: true,
            width: 500,
            show: {
                effect: "drop"
            },
            hide: {
                effect: "drop"
            },
            title: "Verhuizen"
        });
    });

    //-------------------------------------------------------------------------
    //functie om de gemeente op te zoeken
    //-------------------------------------------------------------------------
    function toonGemeenten(postcode) {
        //Groep ID wordt uit een verborgen veld op de pagina gehaald
        var url = "/" + GID + "/Adressen/WoonPlaatsenOphalen";
        $.getJSON(url, { postNummer: postcode }, function (data) {
            var options = '';
            for (var i = 0; i < data.length; i++) {
                options += '<option value="' + data[i].Naam + '">' + data[i].Naam + '</option>';
            }
            $("#gemeente").html(options);
        });
    }
    //-------------------------------------------------------------------------
});