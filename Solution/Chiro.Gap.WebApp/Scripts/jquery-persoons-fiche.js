/*
Frontend javascript (JQuery)
GAP, Chirojeugd Vlaanderen
2013
*/

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
    $('#adressen td:last-child, #adressenLeeg td:last-child')
        .append("<div class=\"adrToev ui-icon ui-icon-circle-plus\" title=\"Toevoegen\" style=\"cursor: pointer\"></div>");
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
    var geboortedatum = $('#gdInfo').val().trim();
    var geslacht = $('#geslachtsInfo').text().trim();
    var chiroleeftijd = $('#chiroleeftijdInfo').text().trim();
    if (chiroleeftijd == "") {
        chiroleeftijd = 0;
    }
    var versiestring = $('#versieString').val().trim();
    var groepswerkJaar = $('#gwJaar').val().trim();
    var lidType = $('#lidType').val().trim();
    //------------------------------------------------------------------------------------------
    //Algemenen Pagina instellingen
    //------------------------------------------------------------------------------------------
    //buttons maken
    $('#print').button({
        icons: {
            primary: "ui-icon-print"
        }
    });

    $('#terug').button({
        icons: {
            primary: "ui-icon-arrowreturnthick-1-w"
        }
    });

    $('#terug').click(function () {
        window.location = $('#terug a').attr('href');
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
    $('#btn_inschrijven').button({
        icons: {
            primary: "ui-icon-circle-plus"
        }
    });
    $('#btn_inschrijven').click(function () {
        var url = "/" + GID + "/Personen/Inschrijven?gelieerdePersoonID=" + GPid + " #main";
        $('#extraInfoDialog').dialog();
        $('#extraInfoDialog').load(url, function () {
            gedeeltelijkTonen('#extraInfoDialog');
            $(this).find('fieldset').css('width', '100%');
            $('#extraInfoDialog').dialog({
                title: "Inschrijven",
                width: 600,
                buttons: {
                    'Schrijf in': function () {
                        $('#bewaar').click();
                    },
                    'Annuleren': function () {
                        $(this).dialog('destroy');
                        $(this).dialog('close');
                    }
                }
            });
        });
        clearDialog();
    });

    //defaults
    $.fn.editable.defaults.mode = 'inline';
    $.fn.editable.defaults.clear = true;
    $.fn.editable.defaults.toggle = 'manual';

    //$('#gdInfo').hide();
    $('#gdTekst').text(geboortedatum);
    //------------------------------------------------------------------------------------------
    //inline editing
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
        $('#gdInput').show();
        $('#gdTekst').hide();
        $('#gdInfo').attr('disabled', false);
        $('#gdInfo').datepicker({
            changeMonth: true,
            changeYear: true,
            onSelect: function () {
                geboortedatum = $('#gdInfo').val();
                bewaarGegevens('geboortedatum', geboortedatum);
                $('#gdInput').hide();
                $('#gdTekst').text(geboortedatum).show();
            },
            onClose: function () {
                $('#gdInput').hide();
                $('#gdTekst').text(geboortedatum).show();
            }
        });
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
                var antwoord;

                var type = $(this).parent().attr('id');

                if (type == 'tel') {
                    antwoord = controleerTel(value);
                    return antwoord;
                } else {
                    antwoord = controleerEmail(value);
                    return antwoord;
                }
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
        clearDialog();
        var comID = $(this).parent().parent().find('td input').val();
        $('#extraInfoDialog').html("Ben je zeker dat je deze communicatievorm wil verwijderen?")
            .dialog({
                modal: true,
                title: "Bevestiging",
                buttons: {
                    'Ja': function () {
                        var url = "/" + GID + "/Personen/VerwijderenCommVorm?commvormID=" + comID;
                        $.get(url);
                        $('input[value=' + comID + ']').parent().parent().hide();
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
                        $.get(url);
                        //verbergt de rij in de tabel zodat de pagina niet herladen hoeft te worden.
                        $('input[value=' + comID + ']').parent().parent().hide();
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
    //oude pagina die geladen wordt bij het aanmaken van een nieuwe communicatievorm (van uit: 'communicatie toevoegen')
    $('#laadNieuweCom .comToev').click(function () {
        var waarde;
        $('#extraInfoDialog').dialog();
        var url = "/" + GID + "/Personen/NieuweCommVorm?gelieerdePersoonID=" + GPid + " #main";
        $('#extraInfoDialog').load(url, function () {

            gedeeltelijkTonen('#extraInfoDialog');
            var attri = '000-00 00 00';
            var antwoord;
            $('#NieuweCommVorm_Nummer').attr('placeholder', attri);
            $('#NieuweCommVorm_Nummer').attr('required', true);
            $('#NieuweCommVorm_Nummer').attr('type', 'tel');


            $('#NieuweCommVorm_CommunicatieTypeID').on('change', function () {
                $('#verbRij').hide();
                waarde = $(this).val();
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
                waarde = $('#NieuweCommVorm_CommunicatieTypeID').val();
                $('#extraInfoDialog').dialog({
                    title: "Nieuwe communicatievorm",
                    modal: true,
                    width: 550,
                    buttons: {
                        'Bewaren': function () {
                            if (waarde == 1 || waarde == 2) {
                                antwoord = controleerTel($('#NieuweCommVorm_Nummer').val());

                                if (antwoord == "") {
                                    $('#bewaarComm').click();
                                } else {
                                    $('#fouten').css('background-color', 'red').html(antwoord);
                                    $('#verbRij').show();
                                }
                            } else {
                                $('#bewaarComm').click();
                            }
                        },
                        'Annuleren': function () {
                            $(this).dialog('close');
                            return false;
                        }
                    }
                });
            }
        });
        clearDialog();
    });

    //Eigen form + post voor email en tel
    $('.comToev').not($('#laadNieuweCom .comToev')).click(function () {
        var cId = $(this).parent().parent().attr('id');
        var nummer;
        var voorkeur;
        var snelleber;
        var gezincom;
        var nota;
        var gaVerder = true;
        var type;
        var antwoord
        
        $('#commDialog #voorkeur, #commDialog #snel').attr('checked', false);
        $('#commDialog #adresNota').val('');
        if (cId == 'email') {
            $('#commDialog select').html('<option>Email</option>');
            $('#nummer').attr("type", "email");
            $('#sb').show();
            type = 3;
        } else if (cId == 'tel') {
            $('#commDialog select').html('<option>Tel</option>');
            $('#nummer').attr("type", "tel");
            type = 1;
        }

        var url = "/" + GID + "/Personen/NieuweCommVorm?gelieerdePersoonID=" + GPid;
        $('#commDialog').dialog({
            modal: true,
            width: 510,
            title: "Communicatievorm toevoegen",
            buttons: {
                'Bewaren': function () {
                    gaVerder = true;
                    nummer = $('#nummer').val();
                    if (type == 1) {
                        antwoord = controleerTel(nummer);
                        if (antwoord != "") {
                            $('#errorTekst').html(antwoord);
                            $('#error').show();
                            gaVerder = false;
                        }
                    }
                    if (type == 3) {
                        antwoord = controleerEmail(nummer);
                        if (antwoord != "") {
                            $('#errorTekst').html(antwoord);
                            $('#error').show();
                            gaVerder = false;
                        }
                    }

                    $('#voorkeurCheck').is(':checked') ? voorkeur = true : voorkeur = false;
                    $('#snelCheck').is(':checked') ? snelleber = true : snelleber = false;
                    $('#gezinCheck').is(':checked') ? gezincom = true : gezincom = false;
                    nota = $('#adresNota').val();
                    if (gaVerder) {
                        $.ajax({
                            url: url,
                            type: 'POST',
                            traditional: true,
                            data: {
                                "NieuweCommVorm.CommunicatieTypeID": type,
                                "NieuweCommVorm.Nummer": nummer,
                                "NieuweCommVorm.IsVoorOptIn": snelleber,
                                "NieuweCommVorm.Voorkeur": voorkeur,
                                "NieuweCommVorm.IsGezinsGebonden": gezincom,
                                "NieuweCommVorm.Nota": nota
                            }
                        }).done(function () {
                            location.reload();
                            $(this).dialog('close');
                        });
                        clearDialog();
                        bezig();
                    }
                },
                'Annuleren': function () {
                    $(this).dialog('close');
                }
            }
        });
    });

    //------------------------------------------------------------------------------------------
    //Adres
    //verster wordt getoond bij het wijzigen van een adres, gegevens worden automatisch ingevuld
    // (uitgelezen uit de hiddenfields op de pagina)
    //--------------------------------------------------------------------
    $('.bewerkAdres').click(function (e) {
        //TODO: Checkboxen voor de gelieerde personen die mee moeten 'verhuizen' 
        //TODO:(id's worden al ingelezen, dus enkel nog naam ophalen, in een checkbox zetten en enkel de gechekte in de lijst plaatsen)
        //TODO: automatisch aanvullen van straatnamen (zie code in comm. hierboven). Geeft in JSON de straatnamen terug maar nog niet als lijst in de inputbox
        //TODO: Fout (in antwoord op post) opvangen en weergeven.
        e.preventDefault();
        var gpidList = [];
        var straatnaam;
        var huisnummer;
        var postcode;
        var bus;
        var gemeente;
        var oudAdresId;
        var type;
        var land;
        var postnr;
        var woonplaatsbuitenland;

        var adresID = $(this).parent().parent().find('td input#persoonsAdresID').val();

        //Haal het model op
        url = "/" + GID + "/Personen/Verhuizen/" + adresID;

        $.getJSON(url, { aanvragerID: GPid }, function (data) {
            //info uit model invullen in variablen
            straatnaam = data.Straat;
            postnr = data.PostNr;
            huisnummer = data.HuisNr;
            bus = data.Bus;

            //lijst met gelieerde personen aanmaken (hier ook best de checkboxes aanmaken)
            $.each(data.GelieerdePersoonIDs, function (index, value) {
                var waarde = parseInt(value);
                gpidList.push(waarde);
            });
            //invullen in de velden
            $('#straatnaam').val(straatnaam);
            $('#huisnr').val(huisnummer);
            $('#postcode').val(postnr);
            $('#bus').val(bus);

            oudAdresId = data.OudAdresID;
            type = data.PersoonsAdresInfo.AdresType;
            land = data.PersoonsAdresInfo.LandNaam;
            postcode = data.PersoonsAdresInfo.PostCode;
            woonplaatsbuitenland = data.WoonPlaatsBuitenLand;

            success:
            {
                toonGemeenten(postnr, '#gemeente');
            }
        });


        $('#postcode').change(function () {
            var pc = $(this).val();
            toonGemeenten(pc, '#gemeente');
        });

        $('#adresDialog').dialog({
            modal: true,
            title: "Adres wijzigen",
            buttons: {
                "Wijzig adres": function () {

                    straatnaam = $('#straatnaam').val();
                    huisnummer = $('#huisnr').val();
                    postnr = $('#postcode').val();
                    bus = $('#bus').val();
                    gemeente = $('#gemeente').val();

                    url = "/" + GID + "/Personen/verhuizen/" + adresID;

                    $.ajax({
                        url: url,
                        type: 'POST',
                        traditional: true,
                        data: {
                            action: "Bewaren",
                            "PersoonsAdresInfo.AdresType": type,
                            Land: land,
                            PostCode: postcode,
                            PostNr: postnr,
                            Straat: straatnaam,
                            HuisNr: huisnummer,
                            Bus: bus,
                            WoonPlaats: gemeente,
                            WoonPlaatsBuitenLand: woonplaatsbuitenland,
                            AanvragerID: GPid,
                            OudAdresID: oudAdresId,
                            GelieerdePersoonIDs: gpidList
                        }
                    });
                    $(this).dialog("close");
                },
                "Annuleren": function () {
                    $(this).dialog("close");
                }
            }
        });
    });

    //Adres verwijderen
    $('.adresverw').click(function (e) {
        e.preventDefault();
        var adresID = $(this).parent().parent().find('td input').val();
        $('#extraInfoDialog').dialog();
        var url = "/" + GID + "/Personen/Adresverwijderen/" + adresID + "?gelieerdePersoonId=" + GPid + ' #main';
        $('#extraInfoDialog').load(url, function () {
            gedeeltelijkTonen('#extraInfoDialog');
            success:
            {
                $("#extraInfoDialog").dialog({
                    modal: true,
                    title: "Adres verwijderen",
                    buttons: {
                        'Verwijderen': function () {
                            $('#extraInfoDialog #verwijderAdres').click();
                            $(this).dialog('close');
                        },
                        'Annuleren': function () {
                            $(this).dialog('destroy');
                            $(this).dialog('close');
                        }
                    },
                    width: 500
                });
            }
        });
        clearDialog();
    });

    //Adres Toevoegen
    $('.adrToev').click(function () {
        adresToevoegen(GID, GPid);
        clearDialog();
    });

    //voorkeursadres maken
    $('.voorkeursAdresMaken').click(function () {
        $('#extraInfoDialog').dialog();
        var voorkeursadresID = $(this).parent().parent().find('td input#voorkeursadresID').val();
        var url = "/" + GID + "/Personen/VoorkeurAdresMaken";
        bezig();
        $.get(url, { persoonsAdresID: voorkeursadresID, gelieerdePersoonID: GPid }, function () {
            success:
            {
                location.reload();
            }
        });
        clearDialog();
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
      });
    //------------------------------------------------------------------------------------------
    //Ingeschreven als (lid/ leiding)

    $('#bewerkLidInfo').click(function (e) {
        e.stopPropagation();
        e.preventDefault();
        $('#lidInfoInfo').editable('toggle');
    });

    $('#lidInfoInfo').editable({
        source: [
                { value: 0, text: 'Leiding' },
                { value: 1, text: 'Lid' }
            ]
    })
        .on('save', function (e, params) {
            e.preventDefault();
            bezig();
            var url = "/" + GID + "/Leden/TypeToggle/" + id;
            $.post(url, { id: id, groepID: GID }, function () {
                success:
                {
                    location.reload();
                }
            }).fail(function () {
                alert("Fout: Er ging iets mis bij het veranderen van de status van deze persoon");
                location.reload();
            });
        });

    //------------------------------------------------------------------------------------------
    // afdeling bewerken

    $('#bewerkAfdeling').click(function (e) {
        var groep = [];
        var naam;
        var tekst = "";
        var waarde;
        var teller = 0;
        e.stopPropagation();
        e.preventDefault();
        var url = "/" + GID + "/Leden/AfdelingBewerken";

        $.getJSON(url, { groepsWerkJaarID: groepswerkJaar, lidID: id }, function (data) {

            $.each(data.BeschikbareAfdelingen, function (index, value) {
                naam = data.BeschikbareAfdelingen[teller].Naam;
                waarde = data.BeschikbareAfdelingen[teller].AfdelingsJaarID;
                if (lidType == 'Leiding') {
                    $('#afdelingenDialog fieldset').append('<input type="checkbox" id="' + naam + '" value="' + waarde + '">' + naam + "</input><br/>");
                } else {
                    $('#afdelingenDialog fieldset').append('<input type="radio" id="' + naam + '" name="afdeling" value="' + waarde + '">' + naam + "</input><br/>");
                }
                teller++;
            });

            success:
            {
                $('#afdelingenDialog').dialog({
                    title: "Afdeling(en) toekennen",
                    modal: true,
                    buttons: {
                        'Bewaren': function () {
                            $.each($('#afdelingenDialog fieldset input:checked'), function (index, value) {
                                waarde = $(this).val();
                                tekst += $(this).attr('id') + " ";
                                groep.push(waarde);
                            });

                            url = "/" + GID + "/Leden/AfdelingBewerken";
                            $.ajax({
                                url: url,
                                type: 'POST',
                                traditional: true,
                                data: {
                                    "groepsWerkJaarID": groepswerkJaar,
                                    lidID: id,
                                    "Info.AfdelingsJaarIDs": groep
                                }
                            });
                            $('#afdelingInfo').text(tekst);
                            $(this).dialog('close');
                            $('#afdelingenDialog fieldset').html('');
                        },
                        'Annuleren': function () {
                            $(this).dialog('close');
                            $('#afdelingenDialog fieldset').html('');
                        }
                    }
                });
            }
        });
    });

    //------------------------------------------------------------------------------------------
    //lidgeld Toggle

    $('#bewerkLidgeld').click(function (e) {
        e.preventDefault();
        var url = "/" + GID + "/Leden/LidGeldToggle/" + id;
        var g = $('#lidgeldInfo b').text().trim();
        if (g == 'Nog niet betaald') {
            $('#lidgeldInfo b').text('Betaald');
        } else {
            $('#lidgeldInfo b').text('Nog niet betaald');
        }

        $.post(url, { id: id, groepID: GID });

    });
    //------------------------------------------------------------------------------------------
    //functies bewerken
    $('#bewerkFuncties').click(function (e) {
        e.preventDefault();
        $('#extraInfoDialog').dialog();
        var url = "/" + GID + "/Leden/FunctiesToekennen/" + id + ' #main';
        $('#extraInfoDialog').load(url, function () {
            gedeeltelijkTonen('#extraInfoDialog');
            $(this).find('a').hide();
            success:
            {
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
            }
        });
        clearDialog();
    });

    //------------------------------------------------------------------------------------------
    // verzekering tegen loonverlies
    $('#bewerkVerzekering').click(function (e) {
        e.preventDefault();
        var url = "/" + GID + "/Leden/LoonVerliesVerzekeren/" + id + " #ver";
        $('#bewerkVerzekeringDialog').load(url);

        $('#bewerkVerzekeringDialog').dialog({
            modal: true,
            title: "Verzekeren tegen loonverlies",
            width: 500,
            buttons: {
                "Bevestigen": function () {
                    url = "/" + GID + "/Leden/LoonVerliesVerzekeren/" + id;
                    $(this).dialog('close');
                    bezig();
                    $.post(url, { id: id, groepID: GID }, function () {

                        success:
                        {
                            location.reload();
                        }
                    });

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
        bezig();
        var url = "/" + GID + "/GebruikersRecht/LoginMaken/" + GPid;
        $.post(url).fail(function () {
            alert("Fout! Er is iets misgelopen");
            location.reload();
        });
    });

    $('#gbrToekennen').click(function () {
        bezig();
        var url = "/" + GID + "/GebruikersRecht/AanGpToekennen/" + GPid;
        $.post(url).fail(function () {
            alert("Fout! Er is iets misgelopen");
            location.reload();
        });
    });
    //TODO: Buttons maken van de links om gebruikersrecht te verlengen of af te nemen + functies daarvoor hier schrijven.
    //------------------------------------------------------------------------------------------
    // Categoriën verwijderen / Toevoegen
    //TODO: verander deze functie naar een eigen form die data post ipv het binnenhalen en tonen van een hele pagina (analoog met afdelingen bewerken, adres bewerken)
    $('.catVerw').click(function (e) {
        bezig();
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

    $('#toevoegenAanCat').click(function () {
        var url = "/" + GID + "/Personen/ToevoegenAanCategorie?gelieerdePersoonID=" + GPid + " #main";
        $('#extraInfoDialog').dialog();
        $('#extraInfoDialog').load(url, function () {
            gedeeltelijkTonen('#extraInfoDialog');
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
                            $(this).dialog('destroy');
                            $(this).dialog('close');
                        }
                    }
                });
            }
        });
        clearDialog();
    });
    //------------------------------------------------------------------------------------------
    // EXTRA INFO
    //------------------------------------------------------------------------------------------
    //Info over verzekering tegen loonsverlies
    $('#loonVerlies').click(function () {
        $('#extraInfoDialog').dialog();
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
            title: "Verzekering tegen loonverlies",
            buttons: {}
        });
    });
    //------------------------------------------------------------------------------------------
    // Extra info weergeven
    $('#vkAdres').click(function () {
        toonInfo('#VK-ADRINFO', "Voorkeurs adres", "#extraInfoDialog");
    });

    $('#verhuizen').click(function () {
        $('#extraInfoDialog').dialog();
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
            title: "Verhuizen",
            buttons: {}
        });
        clearDialog();
    });


    //------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------
    // EIGEN FUNCTIEs
    //------------------------------------------------------------------------------------------
    //functie die de veranderde gegevens post
    //parameters: 
    //      teVeranderen:   waarde veranderd moet worden 
    //                      ('voornaam', 'achternaam', 'geboortedatum', 'geslacht' of 'chiroleeftijd')
    //      nieuweWaarde:   ingegeven waarde die op de plaats van de oude komt

    function bewaarGegevens(teVeranderen, nieuweWaarde, GID, GPid) {
        var url = "/" + GID + "/Personen/EditGegevens/" + GPid;
        var n = achternaam;
        var vn = voornaam;
        var gb = geboortedatum;
        var g = geslacht;
        var cl = chiroleeftijd;
        var fout = false;

        switch (teVeranderen) {
            case 'voornaam':
                vn = nieuweWaarde;
                break;
            case 'achternaam':
                n = nieuweWaarde;
                break;
            case 'geboortedatum':
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
    };
});  
//------------------------------------------------------------------------------------------
// EINDE EIGEN FUNCTIES
//----------------------------------------------------------------------------------------------
// EINDE JQUERY CODE
// CHIROJEUGDVLAANDEREN 2013
//----------------------------------------------------------------------------------------------