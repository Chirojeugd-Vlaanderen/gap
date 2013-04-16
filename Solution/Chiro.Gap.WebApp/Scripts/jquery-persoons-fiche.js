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


//------------------------------------------------------------------------------------------

//--------------------------------------------------------------------------------
//Document ready functie
//--------------------------------------------------------------------------------
$(function () {
    //Tabel layout
    $('table tr:even').css("background-color", "lightGray");

    //settings voor icoontjes bij adres

    // 'Verwijderen' iconen
    $('#adressen td:last-child')
    .append("<div class=\"adresverw ui-icon ui-icon-circle-minus \" title=\"Verwijderen\" id=\"adrVerw\" style=\"cursor: pointer\"></div>");
    $('#tel td:last-child')
    .append("<div class=\"telverw ui-icon ui-icon-circle-minus \" title=\"Verwijderen\" id=\"telVerw\" style=\"cursor: pointer\"></div>");
    $('#email td:last-child')
    .append("<div class=\"emailverw ui-icon ui-icon-circle-minus\" title=\"Verwijderen\" id=\"emailVerw\" style=\"cursor: pointer\"></div>");

    //'toevoegen' iconen
    $('#adressen td:last-child')
        .append("<div class=\"ui-icon ui-icon-circle-plus\" title=\"Toevoegen\" id=\"adrToev\" style=\"cursor: pointer\"></div>");
    $('#commLeeg td:last-child')
        .append("<div class=\"comToev ui-icon ui-icon-circle-plus\" title=\"Toevoegen\" style=\"cursor: pointer\"></div>");
    $('#tel:last td:last-child , #email:last td:last-child')
        .append("<div class=\"comToev ui-icon ui-icon-circle-plus\" title=\"Toevoegen\" style=\"cursor: pointer\"></div>");

    $('.ui-icon').tooltip();
    $('.contact').tooltip();

    //variabelen

    var GID = $('#groepID').val();
    var id = $('#lidIdH').val();
    var GPid = $('#GPid').val();

    var adnummer = $('#adNummerInfo').text().trim();
    var voornaam = $('#voornaamInfo').text().trim();
    var achternaam = $('#achternaamInfo').text().trim();
    var geboortedatum = $('#gdInfo').text().trim();
    var geslacht = $('#geslachtsInfo').text().trim();
    var chiroleeftijd = $('#chiroleeftijdInfo').text().trim();
    var versiestring = $('#versieString').val().trim();


    //------------------------------------------------------------------------------------------
    //inline editing
    //TODO: Ervoor zorgen dat de ingevulde gegevens ook daadwerkelijk opgeslagen worden.
    //------------------------------------------------------------------------------------------
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
    $('#toevoegenAanCat').button({
        icons: {
            primary: "ui-icon-circle-plus"
        }
    });

    //defaults
    $.fn.editable.defaults.mode = 'inline';
    $.fn.editable.defaults.clear = true;
    $.fn.editable.defaults.toggle = 'manual';
    //------------------------------------------------------------------------------------------

    //Voornaam bewerken
    $('#bewerkVoorNaam')
        .click(function (e) {
            e.stopPropagation();
            e.preventDefault();
            $('#voornaamInfo').editable('toggle');
        });

    $('#voornaamInfo').editable({
        validate: function (value) {
            if ($.trim(value) == '') return 'Dit veld mag niet leeg zijn!';
        }
    })
        .on('save', function (e, params) {
            e.preventDefault();
            bewaarGegevens('voornaam', params.newValue);
            voornaam = params.newValue;
            $('#main h2, head title').text(params.newValue + " " + achternaam);
        });
    //------------------------------------------------------------------------------------------
    //Achternaam bewerken
    $('#bewerkAchterNaam')
        .click(function (e) {
            e.stopPropagation();
            e.preventDefault();
            $('#achternaamInfo').editable('toggle');
        });

    $('#achternaamInfo').editable({
        validate: function (value) {
            if ($.trim(value) == '') return 'Dit veld mag niet leeg zijn!';
        }
    })
        .on('save', function (e, params) {
            e.preventDefault();
            bewaarGegevens('achternaam', params.newValue);
            achternaam = params.newValue;
            $('#main h2, head title').text(voornaam + " " + params.newValue);
        });
    //------------------------------------------------------------------------------------------
    //Geboortedatum bewerken
    $('#gdInfo').attr('disabled', true);
    $.datepicker.setDefaults($.datepicker.regional['be']);

    $('#bewerkGd').click(function () {
        $('#gdInfo').attr('disabled', false);
        $('#gdInfo').datepicker({
            changeMonth: true,
            changeYear: true
        });
        $('#gdInfo').click();
    });
    //------------------------------------------------------------------------------------------
    //geslacht bewerken
    $('#bewerkGeslacht').click(function (e) {
        e.preventDefault();
        var g = $('#geslachtsInfo').text().trim();
        if (g == 'Man') {
            $('#geslachtsInfo').text('Vrouw');
        } else {
            $('#geslachtsInfo').text('Man');
        }
        g = $('#geslachtsInfo').text().trim();
        bewaarGegevens('geslacht', g);
    });
    //------------------------------------------------------------------------------------------
    //contact gegevens (tel/email)
    //------------------------------------------------------------------------------------------
    // Algemeen
    $('.contact')
        .editable({
            // reg ex die zorgt voor de validatie van het e-mail adres
            validate: function (value) {
                if ($.trim(value) == '') return "Dit veld mag niet leeg zijn! gebruik '-' om te verwijderen";

                var emailReg = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;
                var telReg = /^[0 - 9]{2,4}\s?-?([0-9]{2}){3}$/;
                if (!emailReg.test(value) || !telReg.test(value)) {
                    return "De ingegeven waarde is ongeldig";
                };
            }
        })
        .on('save', function (e, params) {
            e.preventDefault();
            var cvID = $(this).parent().find('td:first input').val();

            bewaarContactGegevens(cvID, 'nummer', params.newValue);
        });

    $('.contactBewerken').click(function (e) {
        e.stopPropagation();
        e.preventDefault();

        $(this).parent().parent().find('td:eq(1)').editable('toggle');
    });
    //------------------------------------------------------------------------------------------
    //Telefoonnummer verwijderen
    $('.telverw').click(function (e) {
        e.preventDefault();
        var comID = $(this).parent().parent().find('td input').val();

        $('#extraInfoDialog').html("Ben je zeker dat je deze communicatievorm wil verwijderen?")
            .dialog({
                modal: true,
                title: "Bevestiging",
                buttons: {
                    'Ja': function () {
                        var url = "/" + GID + "/Personen/VerwijderenCommVorm?commvormID=" + comID;
                        $.get(url).done(function () {
                            location.reload();
                        });
                        $('#extraInfoDialog').html("Bezig met verwijderen").dialog();
                        $(this).dialog('close');
                    },
                    'Nee': function () {
                        $(this).dialog('close');
                    }
                }
            });
    });
    //------------------------------------------------------------------------------------------
    //email verwijderen

    $('.emailverw').click(function (e) {
        e.preventDefault();
        var comID = $(this).parent().parent().find('td input').val();

        $('#extraInfoDialog').html("Ben je zeker dat je deze communicatievorm wil verwijderen?")
            .dialog({
                modal: true,
                title: "Bevestiging",
                buttons: {
                    'Ja': function () {
                        var url = "/" + GID + "/Personen/VerwijderenCommVorm?commvormID=" + comID;
                        $.get(url).done(function () {
                            location.reload();
                        });
                        $('#extraInfoDialog').html("Bezig met verwijderen").dialog();
                        $(this).dialog('close');
                    },
                    'Nee': function () {
                        $(this).dialog('close');
                    }
                }
            });
    });
    //------------------------------------------------------------------------------------------
    //Nieuwe communicatievorm toevoegen

    $('.comToev').click(function () {
        var url = "/" + GID + "/Personen/NieuweCommVorm?gelieerdePersoonID=" + GPid;
        $('#extraInfoDialog').load(url, function () {
            gedeeltelijkTonen();
            var attri = '000-00 00 00';
            $('#NieuweCommVorm_Nummer').attr('placeholder', attri);
            $('#NieuweCommVorm_Nummer').attr('required', true);

            $('#NieuweCommVorm_CommunicatieTypeID').on('change', function () {
                var waarde = $(this).val();
                switch (waarde) {
                    case '1':
                        attri = '000-00 00 00';
                        $('#NieuweCommVorm_Nummer').attr('type', 'tel');
                        break;
                    case '2':
                        attri = '000-00 00 00';
                        $('#NieuweCommVorm_Nummer').attr('type', 'tel');
                        break;
                    case '3':
                        attri = 'iemand@voorbeeld.be';
                        $('#NieuweCommVorm_Nummer').attr('type', 'email');
                        break;
                    case '4':
                        $('#NieuweCommVorm_Nummer').attr('type', 'url');
                        attri = 'http://www.voorbeeld.be';
                        break;
                    case '5':
                        $('#NieuweCommVorm_Nummer').attr('type', 'email');
                        attri = 'iemand@voorbeeld.be';
                        break;
                    case '6':
                        attri = 'XMPP';
                        break;
                    case '7':
                        attri = 'Twitteraccount';
                        break;
                    case '8':
                        attri = 'Statusnet';
                        break;
                    default:
                        $('#NieuweCommVorm_Nummer').attr('type', 'tel');
                        attri = '000-00 00 00';
                }
                $('#NieuweCommVorm_Nummer').attr('placeholder', attri);
            });
            success:
            {
                $('#extraInfoDialog').dialog({
                    title: "Nieuwe communicatievorm",
                    modal: true,
                    width: 550,
                    buttons: {
                        "Bewaren": function () {
                            $('#bewaarComm').click();
                        },
                        "Annuleren": function () {
                            $(this).dialog('close');
                        }
                    }
                });
            }
        });
    });

    //------------------------------------------------------------------------------------------
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

    //Adres verwijderen
    $('.adresverw').click(function (e) {
        e.preventDefault();
        var adresID = ""; //TODO: id dat in de url moet uitlezen uit input veld en in url zetten

        $('#extraInfoDialog').html("Ben je zeker dat je dit adres wil verwijderen?")
            .dialog({
                modal: true,
                title: "Bevestiging",
                buttons: {
                    'Ja': function () {
                        var url = "/" + GID + "/Personen/Adresverwijderen/?commvormID="; // nog iet nie juist
                        $.get(url).done(function () {
                            location.reload();
                        });
                        $('#extraInfoDialog').html("Bezig met verwijderen").dialog();
                        $(this).dialog('close');
                    },
                    'Nee': function () {
                        $(this).dialog('close');
                    }
                }
            });
    });
    //------------------------------------------------------------------------------------------
    //Chiroleeftijd
    $('#bewerkCl').click(function (e) {
        e.stopPropagation();
        e.preventDefault();
        $('#chiroleeftijdInfo').editable('toggle');

    });

    $('#chiroleeftijdInfo').editable({
        source: [
                { value: 0, text: '0' },
                { value: 1, text: '1' },
                { value: 2, text: '2' },
                { value: 3, text: '3' }
            ],
        showbuttons: false
    })
      .on('save', function (e, params) {
          e.preventDefault();
          bewaarGegevens('chiroleeftijd', params.newValue);
      }); ;
    //------------------------------------------------------------------------------------------
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
    //------------------------------------------------------------------------------------------
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
    //------------------------------------------------------------------------------------------
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
    //------------------------------------------------------------------------------------------
    //functies bewerken
    $('#bewerkFuncties').click(function (e) {
        e.preventDefault();

        var url = "/" + GID + "/Leden/FunctiesToekennen/" + id;
        $('#extraInfoDialog').load(url, function () {
            $('#extraInfoDialog #acties').hide();
            $('#extraInfoDialog #header, #extraInfoDialog #footer').hide();

        });
        $('#extraInfoDialog').dialog({
            modal: true,
            width: 500,
            title: "Functies bewerken",
            buttons: {
                "Bevestigen": function () {
                    $('#extraInfoDialog #bewerkFuncties').click();
                    $(this).dialog('close');
                },
                "Annuleren": function () {
                    $(this).dialog('close');
                }
            }
        });
    });

    //------------------------------------------------------------------------------------------
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
                    url = "/" + GID + "/Leden/LoonVerliesVerzekeren/" + id;

                    $.post(url, { id: id, groepID: GID });
                },
                "Annuleren": function () {
                    $(this).dialog('close');
                }
            }

        });
    });
    //------------------------------------------------------------------------------------------
    //abboneren op dubbelpunt
    $('#abboneer').click(function () {
        var url = "/" + GID + "/Abonnementen/DubbelPuntAanvragen";
        $.post(url, { id: id, groepID: GID });
    });


    //------------------------------------------------------------------------------------------
    //EINDE INLINE EDITING
    //------------------------------------------------------------------------------------------


    //------------------------------------------------------------------------------------------
    //PANEEL AAN DE RECHTERZIJDE
    //------------------------------------------------------------------------------------------

    //gebruikersrechten
    $('#loginMaken').click(function () {
        var url = "/" + GID + "/GebruikersRecht/LoginMaken/" + GPid;
        $.post(url);
    });

    $('#gbrToekennen').click(function () {
        var url = "/" + GID + "/GebruikersRecht/AanGpToekennen/" + GPid;
        $.post(url);
    });
    //------------------------------------------------------------------------------------------
    // Categoriën verwijderen / Toevoegen

    $('#catVerw').click(function (e) {
        e.preventDefault();
        var catID = $('#catID').val();
        var url = "/" + GID + "/Personen/VerwijderenCategorie";
        $('#extraInfoDialog').dialog();

        $.post(url, { categorieID: catID, gelieerdePersoonID: GPid, groepID: GID }, function () {
            success:
            {
                location.reload();
            }
        });

    });

    $('#toevoegenAanCat').click(function () {
        var url = "/" + GID + "/Personen/ToevoegenAanCategorie?gelieerdePersoonID=" + GPid + "#test";
        $('#extraInfoDialog').load(url, function () {
            gedeeltelijkTonen();
            success:
            {
                $('#extraInfoDialog').dialog({
                    modal: true,
                    title: "Toevoegen aan categorie",
                    width: 480,
                    buttons: {
                        'Bewaren': function () {
                            $('#bewaarCat').click();
                        },
                        'Annuleren': function () {
                            $(this).dialog('close');
                        }
                    }
                });
            }
        });
    });
    //------------------------------------------------------------------------------------------
    // EXTRA INFO
    //------------------------------------------------------------------------------------------
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
    //------------------------------------------------------------------------------------------
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
    //------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------
    // EIGEN FUNCTIEs
    //------------------------------------------------------------------------------------------

    function gedeeltelijkTonen() {
        $('#extraInfoDialog #header, #extraInfoDialog #footer').hide();
        $('#extraInfoDialog h2,#extraInfoDialog legend').hide();
        $('#extraInfoDialog fieldset').css({ 'width': '350px' });
        $('#extraInfoDialog #acties').hide();
    }
    //-------------------------------------------------------------------------
    //functie om de gemeente op te zoeken

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
    //Functie 'bezig met verwerking'

    function bezig() {
        $('#extraInfoDialog').dialog();
    }

    //------------------------------------------------------------------------------------------
    //functie die de veranderde gegevens post
    //parameters: 
    //      teVeranderen:   waarde veranderd moet worden 
    //                      ('voornaam', 'achternaam', 'geboortedatum', 'geslacht' of 'chiroleeftijd')
    //      nieuweWaarde:   ingegeven waarde die op de plaats van de oude komt

    function bewaarGegevens(teVeranderen, nieuweWaarde) {
        var url = "/" + GID + "/Personen/EditGegevens/" + GPid;
        var n = achternaam;
        var vn = voornaam;
        var gb = geboortedatum;
        var g = geslacht;
        var cl = chiroleeftijd;

        switch (teVeranderen) {
            case 'voornaam':
                vn = nieuweWaarde;
                break;
            case 'achternaam':
                n = nieuweWaarde;
                break;
            case 'geboortedatum':
                gb = nieuweWaarde;
                break;
            case 'geslacht':
                g = nieuweWaarde;
                break;
            case 'chiroleeftijd':
                cl = nieuweWaarde;
                break;
            default:
        }
        $.post(url,
            {
                "HuidigePersoon.AdNummer": adnummer,
                "HuidigePersoon.Voornaam": vn,
                "HuidigePersoon.Naam": n,
                "HuidigePersoon.GeboorteDatum": gb,
                "HuidigePersoon.Geslacht": g,
                "HuidigePersoon.ChiroLeefTijd": cl,
                "HuidigePersoon.GelieerdePersoonID": GPid,
                "BroerZusID": 0,
                "HuidigePersoon.VersieString": versiestring
            });
    }
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    // Bewaar telefoonnummers en email adressen
    //-------------------------------------------------------------------------
    function bewaarContactGegevens(cvid, teVeranderen, nieuweWaarde) {

        var nummer = "";
        var snelleBerichten = false;
        var voorkeur1 = true;
        var voorkeur2 = false;
        var GG = false;
        var nota = "";
        var url = "/" + GID + "/Personen/CommVormBewerken";
        $.getJSON(url, { commvormID: cvid, gelieerdePersoonID: GPid }, function (data) {
            nummer = data.NieuweCommVorm.Nummer;
            snelleBerichten = data.NieuweCommVorm.IsVoorOptIn;
            voorkeur1 = data.NieuweCommVorm.Voorkeur;
            GG = data.NieuweCommVorm.IsGezinsGebonden;
            nota = data.NieuweCommVorm.Nota;

            success:
            {
                switch (teVeranderen) {
                    case 'nummer':
                        //data.NieuweCommVorm.Nummer = nieuweWaarde;
                        nummer = nieuweWaarde;
                        break;
                    case 'snelleBerichten':
                        snelleBerichten = nieuweWaarde;
                        break;
                    case 'voorkeur':
                        voorkeur1 = nieuweWaarde;
                        break;
                    case 'gezin':
                        GG = nieuweWaarde;
                        break;
                    case 'nota':
                        nota = nieuweWaarde;
                        alert("hier mag ik nie zijn");
                        break;
                    default:
                }

                url = "/" + GID + "/Personen/CommVormBewerken?commvormID=" + cvid + "&gelieerdePersoonID=" + GPid;

                $.post(url,
                            {
                                "NieuweCommVorm.Nummer": nummer,
                                "NieuweCommVorm.IsVoorOptIn": snelleBerichten,
                                "NieuweCommVorm.Voorkeur": voorkeur1,
                                "NieuweCommVorm.IsGezinsGebonden": GG,
                                "NieuweCommVorm.Nota": nota,
                                "NieuweCommVorm.ID": cvid,
                                "NieuweCommVorm.VersieString": versiestring

                            });
            }
        });
    }
    //------------------------------------------------------------------------------------------
    // Nieuwe communicatie vorm
    /*function nieuweCommunicatie(vorm, waarde, snel, vrkr, gg, opm) {

    var url = "/" + GID + "/Personen/NieuweCommVorm";

    /*$.getJSON(url, { gelieerdePersoonID: GPid }, function (data) {
    //success:
    //{
    $.post(url,
    {
    "NieuweCommVorm.CommunicatieTypeOmschrijving": vorm,
    "NieuweCommVorm.Nummer": waarde,
    "NieuweCommVorm.IsVoorOptIn": snel,
    "NieuweCommVorm.Voorkeur": vrkr,
    "NieuweCommVorm.IsGezinsGebonden": gg,
    "NieuweCommVorm.Nota": opm,
    "NieuweCommVorm.VersieString": versiestring

    });
    //}
    //});
    }*/
    //------------------------------------------------------------------------------------------
});
    
//------------------------------------------------------------------------------------------
// EINDE EIGEN FUNCTIES
//----------------------------------------------------------------------------------------------
// EINDE JQUERY CODE
// CHIROJEUGDVLAANDEREN 2013
//----------------------------------------------------------------------------------------------