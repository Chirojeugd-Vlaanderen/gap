/*
* Copyright 2013, Arno Soontjens
* Copyright 2013, Chirojeugd-Vlaanderen (hergebruik adressenscripts)
* 
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
* 
*     http://www.apache.org/licenses/LICENSE-2.0
* 
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

//--------------------------------------------------------------------------------
//Document ready functie
//--------------------------------------------------------------------------------
$(function() {
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
    var werkjaar = $('#werkjaar').val();
    var versiestring = $('#versieString').val().trim();
    var groepswerkJaar = $('#gwJaar').val().trim();
    var lidType = $('#lidType').val().trim();

    var url;
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

    $('#terug').click(function() {
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


    $('#btn_inschrijven').click(function() {
        var geboortejaar = geboortedatum.substr(-4);
        geboortejaar = parseInt(geboortejaar);
        var leeftijd = werkjaar - geboortejaar;
        if (leeftijd < 7) {
            $('#extraInfoDialog').html("Het is niet mogelijk om kleuters in te schrijven!");
            $('#extraInfoDialog').dialog({
                title: "Onmogelijk",
                modal: true,
                buttons: {
                    'Ok': function() {
                        $(this).dialog('close');
                    }
                }
            });
        } else {
            url = link("Personen", "Inschrijven");
            url += "?gelieerdePersoonID=" + GPid + " #main";
            $('#extraInfoDialog').dialog();
            $('#extraInfoDialog').load(url, function() {
                gedeeltelijkTonen('#extraInfoDialog');
                $(this).find('fieldset').css('width', '100%');
                $('#extraInfoDialog').dialog({
                    title: "Inschrijven",
                    width: 600,
                    height: 350,
                    modal: true,
                    buttons: {
                        'Schrijf in': function() {
                            $('#bewaar').click();
                        },
                        'Annuleren': function() {
                            $(this).dialog('close');
                        }
                    }
                });
            });
            clearDialog();
        }
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
        .click(function(e) {
            e.stopPropagation();
            e.preventDefault();
            $('#voornaamInfo').editable('toggle');
        });

    $('#voornaamInfo').editable({
        validate: function(value) {
            if ($.trim(value) == '') return 'Dit veld mag niet leeg zijn!';
        }
    })
        .on('save', function(e, params) {
            e.preventDefault();
            bewaarGegevens('voornaam', params.newValue, GID, GPid);
            voornaam = params.newValue;
            $('#main h2, head title').text(params.newValue + " " + achternaam);
        });
    //------------------------------------------------------------------------------------------
    //Achternaam bewerken
    $('#bewerkAchterNaam')
        .click(function(e) {
            e.stopPropagation();
            e.preventDefault();
            $('#achternaamInfo').editable('toggle');
        });

    $('#achternaamInfo').editable({
        validate: function(value) {
            if ($.trim(value) == '') return 'Dit veld mag niet leeg zijn!';
        }
    })
        .on('save', function(e, params) {
            e.preventDefault();
            bewaarGegevens('achternaam', params.newValue, GID, GPid);
            achternaam = params.newValue;
            $('#main h2, head title').text(voornaam + " " + params.newValue);
        });
    //------------------------------------------------------------------------------------------
    //Geboortedatum bewerken
    $('#gdInfo').attr('disabled', true);
    $.datepicker.setDefaults($.datepicker.regional['be']);

    $('#bewerkGd').click(function() {
        $('#gdInput').show();
        $('#gdTekst').hide();
        $('#gdInfo').attr('disabled', false);
        $('#gdInfo').datepicker({
            changeMonth: true,
            changeYear: true,
            onSelect: function() {
                geboortedatum = $('#gdInfo').val();
                bewaarGegevens('geboortedatum', geboortedatum, GID, GPid);
                $('#gdInput').hide();
                $('#gdTekst').text(geboortedatum).show();
            },
            onClose: function() {
                $('#gdInput').hide();
                $('#gdTekst').text(geboortedatum).show();
            }
        });
    });
    //------------------------------------------------------------------------------------------
    //geslacht bewerken
    $('#bewerkGeslacht').click(function(e) {
        e.preventDefault();
        var g = $('#geslachtsInfo').text().trim();
        if (g == 'Man') {
            $('#geslachtsInfo').text('Vrouw');
        } else {
            $('#geslachtsInfo').text('Man');
        }
        g = $('#geslachtsInfo').text().trim();
        bewaarGegevens('geslacht', g, GID, GPid);
    });
    //------------------------------------------------------------------------------------------
    //contact gegevens (tel/email)
    //------------------------------------------------------------------------------------------
    // Algemeen
    $('.contact')
        .editable({
            validate: function(value) {
                if ($.trim(value) == '') return "Dit veld mag niet leeg zijn! gebruik '-' om te verwijderen";
                var antwoord;

                var type = $(this).attr('id');

                // FIXME: dit is gepruts

                if (type.charAt(0) == 't') {
                    antwoord = controleerTel(value);
                    return antwoord;
                } else {
                    antwoord = controleerEmail(value);
                    return antwoord;
                }
            }
        })
        .on('save', function(e, params) {
            e.preventDefault();
            var rijID = $(this).attr('id');
            // extract communicatievormid uit ID van de rij.
            var pattern = /([0-9]*)$/;
            var cvID = pattern.exec(rijID)[0];

            wijzigCommunicatieNr(cvID, params.newValue);
        });

    $('.contactBewerken').click(function(e) {
        e.stopPropagation();
        e.preventDefault();

        $(this).parent().parent().find('.contact').editable('toggle');
    });
    //------------------------------------------------------------------------------------------
    //email & Telefoonnummer verwijderen
    $('.emailverw, .telverw').click(function(e) {
        e.preventDefault();
        var comID = $(this).parent().parent().find('td input').val();
        $('#extraInfoDialog').html("Ben je zeker dat je deze communicatievorm wil verwijderen?")
            .dialog({
                modal: true,
                title: "Bevestiging",
                buttons: {
                    'Ja': function() {
                        url = link("Personen", "VerwijderenCommVorm");
                        $.get(url, { commvormID: comID });
                        //verbergt de rij in de tabel zodat de pagina niet herladen hoeft te worden.
                        $('input[value=' + comID + ']').parent().parent().hide();
                        $(this).dialog('close');
                    },
                    'Nee': function() {
                        $(this).dialog('close');
                    }
                }
            });
    });
    //------------------------------------------------------------------------------------------
    //Nieuwe communicatievorm toevoegen
    //oude pagina die geladen wordt bij het aanmaken van een nieuwe communicatievorm (van uit: 'communicatie toevoegen')
    $('#laadNieuweCom .comToev').click(function() {
        var waarde;
        $('#extraInfoDialog').dialog();

        url = link("Personen", "NieuweCommVorm");
        url += "?gelieerdePersoonID=" + GPid + " #main";
        $('#extraInfoDialog').load(url, function() {

            gedeeltelijkTonen('#extraInfoDialog');
            var attri = '000-00 00 00';
            var antwoord;
            $('#NieuweCommVorm_Nummer').attr('placeholder', attri);
            $('#NieuweCommVorm_Nummer').attr('required', true);
            $('#NieuweCommVorm_Nummer').attr('type', 'tel');


            $('#NieuweCommVorm_CommunicatieTypeID').on('change', function() {
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
                    height: 530,
                    buttons: {
                        'Bewaren': function() {
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
                        'Annuleren': function() {
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
    $('.comToev').not($('#laadNieuweCom .comToev')).click(function() {
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

        url = link("Personen", "NieuweCommVorm");
        url += "?gelieerdePersoonID=" + GPid;
        $('#commDialog').dialog({
            modal: true,
            width: 510,
            title: "Communicatievorm toevoegen",
            buttons: {
                'Bewaren': function() {
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
                        }).done(function() {
                            location.reload();
                            $(this).dialog('close');
                        });
                        bezig();
                    }
                },
                'Annuleren': function() {
                    $(this).dialog('close');
                }
            }
        });
    });

    //------------------------------------------------------------------------------------------
    //Adres bewerken
    //--------------------------------------------------------------------
    $('.bewerkAdres').click(function(e) {
        $('#extraInfoDialog').dialog();

        // FIXME: dit lijkt me allemaal erg moeilijk te onderhouden, omdat
        // we sterk afhankelijk zijn van hoe alle controller acties heten, e.d.

        // zouden we die url niet ergens gewoon bij in de html kunnen steken, en hem
        // daar oppikken met JQuery? Dat lijkt me ook noodzakelijk moesten we de javascript
        // opnieuw inobtrusive maken.

        var adresID = parseInt($(this).parent().parent().find('td input#persoonsAdresID').val());
        url = link("Personen", "Verhuizen") + "/" + adresID + '?aanvragerID=' + GPid + " #main";

        $('#extraInfoDialog').load(url, function() {
            gedeeltelijkTonen('#extraInfoDialog');
            $('#tabel').show();

            // Door deze code kunnen users de form niet submitten met 'enter' (gaf een fout over de postcode)
            $(this).keydown(function(event) {
                if (event.keyCode == 13) {
                    event.preventDefault();
                    return false;
                }
                return true;
            });

            AdresBewerken();

            success:
            {
                $('#extraInfoDialog fieldset').css('width', '600px');
                $(this).dialog({
                    title: "Verhuizen",
                    modal: true,
                    width: 700,
                    height: 600,
                    resizable: true,
                    buttons: {
                        'Bewaren': function() {
                            $('#extraInfoDialog #bewaarAdres').click();
                            $(this).dialog('close');
                        },
                        'Annuleren': function() {
                            $(this).dialog('close');
                        }
                    }
                });
            }

        });
    });

    //Adres verwijderen
    $('.adresverw').click(function(e) {
        e.preventDefault();
        var adresID = $(this).parent().parent().find('td input').val();
        $('#extraInfoDialog').dialog();

        url = link("Personen", "Adresverwijderen");
        url += "/" + adresID + "?gelieerdePersoonId=" + GPid + ' #main';
        $('#extraInfoDialog').load(url, function() {
            gedeeltelijkTonen('#extraInfoDialog');
            success:
            {
                $("#extraInfoDialog").dialog({
                    modal: true,
                    title: "Adres verwijderen",
                    height: 250,
                    buttons: {
                        'Verwijderen': function() {
                            $('#extraInfoDialog #verwijderAdres').click();
                            $(this).dialog('close');
                        },
                        'Annuleren': function() {
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
    $('.adrToev').click(function() {
        adresToevoegenPersoon();
    });

    //voorkeursadres maken
    $('.voorkeursAdresMaken').click(function() {
        $('#extraInfoDialog').dialog();
        var voorkeursadresID = $(this).parent().parent().find('td input#voorkeursadresID').val();
        url = link("Personen", "VoorkeurAdresMaken");
        bezig();
        $.get(url, { persoonsAdresID: voorkeursadresID, gelieerdePersoonID: GPid }, function() {
            success:
            {
                location.reload();
            }
        });
        clearDialog();
    });

    //------------------------------------------------------------------------------------------
    //Chiroleeftijd
    $('#bewerkCl').click(function(e) {
        e.stopPropagation();
        e.preventDefault();
        $('#chiroleeftijdInfo').editable('toggle');

    });

    $('#chiroleeftijdInfo').editable({
        source: [
            { value: -3, text: '-3' },
            { value: -2, text: '-2' },
            { value: -1, text: '-1' },
            { value: 0, text: '0' },
            { value: 1, text: '1' },
            { value: 2, text: '2' },
            { value: 3, text: '3' }
        ],
        showbuttons: false
    })
        .on('save', function(e, params) {
            e.preventDefault();
            bewaarGegevens('chiroleeftijd', params.newValue, GID, GPid);
        });
    //------------------------------------------------------------------------------------------
    //Ingeschreven als (lid/ leiding)

    $('#bewerkLidInfo').click(function(e) {
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
        .on('save', function(e, params) {
            e.preventDefault();
            bezig();
            url = link("Leden", "TypeToggle");

            $.post(url, { id: id, groepID: GID }, function() {
                success:
                {
                    location.reload();
                }
            }).fail(function() {
                alert("Fout: Er ging iets mis bij het veranderen van de status van deze persoon");
                location.reload();
            });
        });

    //------------------------------------------------------------------------------------------
    // afdeling bewerken

    $('#bewerkAfdeling').click(function(e) {
        var groep = [];
        var naam;
        var tekst = "";
        var waarde;
        var teller = 0;
        e.stopPropagation();
        e.preventDefault();

        url = link("Leden", "AfdelingBewerken");
        $.getJSON(url, { groepsWerkJaarID: groepswerkJaar, lidID: id }, function(data) {

            $.each(data.BeschikbareAfdelingen, function(index, value) {
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
                        'Bewaren': function() {
                            $.each($('#afdelingenDialog fieldset input:checked'), function(index, value) {
                                waarde = $(this).val();
                                tekst += $(this).attr('id') + " ";
                                groep.push(waarde);
                            });

                            url = link("Leden", "AfdelingBewerken");
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
                        'Annuleren': function() {
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

    $('#bewerkLidgeld').click(function(e) {
        e.preventDefault();
        url = link("Leden", "LidGeldToggle");
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
    $('#bewerkFuncties').click(function(e) {
        e.preventDefault();
        $('#extraInfoDialog').dialog();

        url = link("Leden", "FunctiesToekennen");
        url += "/" + id + ' #main';
        $('#extraInfoDialog').load(url, function() {
            gedeeltelijkTonen('#extraInfoDialog');
            $(this).find('a').hide();
            success:
            {
                $('#extraInfoDialog').dialog({
                    modal: true,
                    width: 500,
                    height: 450,
                    title: "Functies bewerken",
                    buttons: {
                        "Bevestigen": function() {
                            $('#extraInfoDialog #bewerkFuncties').click();
                            $(this).dialog('close');
                        },
                        "Annuleren": function() {
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
    $('#bewerkVerzekering').click(function(e) {
        e.preventDefault();

        url = link("Leden", "LoonVerliesVerzekeren");
        url += "/" + id + " #ver";
        $('#bewerkVerzekeringDialog').load(url);

        $('#bewerkVerzekeringDialog').dialog({
            modal: true,
            title: "Verzekeren tegen loonverlies",
            width: 500,
            buttons: {
                "Bevestigen": function() {

                    url = link("Leden", "LoonVerliesVerzekeren");
                    $(this).dialog('close');
                    bezig();
                    $.post(url, { id: id, groepID: GID }, function() {

                        success:
                        {
                            location.reload();
                        }
                    });

                },
                "Annuleren": function() {
                    $(this).dialog('close');
                }
            }
        });
    });
    //------------------------------------------------------------------------------------------
    //EINDE INLINE EDITING
    //------------------------------------------------------------------------------------------


    //------------------------------------------------------------------------------------------
    //PANEEL AAN DE RECHTERZIJDE
    //------------------------------------------------------------------------------------------

    //------------------------------------------------------------------------------------------
    // Categoriën verwijderen / Toevoegen
    $('.catVerw').click(function(e) {
        bezig();
        e.preventDefault();
        var catID = $('#catID').val();
        url = link("Personen", "VerwijderenCategorie");
        $.post(url, { categorieID: catID, gelieerdePersoonID: GPid, groepID: GID }, function() {
            success:
            {
                location.reload();
            }
        });

    });

    $('#toevoegenAanCat').click(function() {
        url = link("Personen", "ToevoegenAanCategorie");
        url += "?gelieerdePersoonID=" + GPid + " #main";
        $('#extraInfoDialog').dialog();
        $('#extraInfoDialog').load(url, function() {
            gedeeltelijkTonen('#extraInfoDialog');
            success:
            {
                $('#extraInfoDialog').dialog({
                    modal: true,
                    title: "Toevoegen aan categorie",
                    width: 480,
                    buttons: {
                        'Bewaren': function() {
                            $('#bewaarCat').click();
                        },
                        'Annuleren': function() {
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
    $('#loonVerlies').click(function() {
        $('#extraInfoDialog').dialog();

        url = link("Handleiding", "VerzekeringLoonverlies");
        url += " #kort";
        $('#extraInfoDialog').load(url, function(response, status, xhr) {
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
        clearDialog();
    });
    //------------------------------------------------------------------------------------------
    // Extra info weergeven
    $('#vkAdres').click(function() {
        toonInfo('#VK-ADRINFO', "Voorkeurs adres", "#extraInfoDialog");
    });

    $('#verhuizen').click(function() {
        $('#extraInfoDialog').dialog();

        url = link("Handleiding", "Verhuizen");
        url += " #verhuizen";
        $('#extraInfoDialog').load(url);

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

        url = link("Personen", "EditGegevens");
        url += "/" + GPid;
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
    // Verandert van de communicatievorm met gegeven CommunicatieVormID het
    // nummer/e-mailadres/...
    //-------------------------------------------------------------------------
    function wijzigCommunicatieNr(cvid, nieuweWaarde) {
                url = link("Personen", "NummerWijzigen");

                $.post(url,
                    {
                        "Waarde": nieuweWaarde,
                        "ID": cvid
                    });
    }

    //-------------------------------------------------------------------------
    // Toont het adressenformulier, en zorgt ervoor dat de user het kan
    // gebruiken om een adres aan de persoon toe te voegen.
    //-------------------------------------------------------------------------
    function adresToevoegenPersoon() {
        $('#extraInfoDialog').dialog();

        url = link("Personen", "NieuwAdres");
        url = url + "/" + GPid + " #main";

        $('#extraInfoDialog').load(url, function() {
            gedeeltelijkTonen('#extraInfoDialog');
            $('#tabel').show();

            // Door deze code kunnen users de form niet submitten met 'enter' (gaf een fout over de postcode)
            $(this).keydown(function(event) {
                if (event.keyCode == 13) {
                    event.preventDefault();
                    return false;
                }
                return true;
            });

            AdresBewerken();
            
            success:
            {
                $('#extraInfoDialog fieldset').css('width', '600px');
                $(this).dialog({
                    title: "Adres Toevoegen",
                    modal: true,
                    width: 700,
                    height: 600,
                    resizable: true,
                    buttons: {
                        'Bewaren': function() {
                            $('#extraInfoDialog #bewaarAdres').click();
                            $(this).dialog('close');
                        },
                        'Annuleren': function() {
                            $(this).dialog('close');
                        }
                    }
                });
            }
        });
        clearDialog();
    }

            //gedeeltelijkTonen("#johan_dialog");
});  
//------------------------------------------------------------------------------------------
// EINDE EIGEN FUNCTIES
//----------------------------------------------------------------------------------------------
// EINDE JQUERY CODE
// CHIROJEUGDVLAANDEREN 2013
//----------------------------------------------------------------------------------------------