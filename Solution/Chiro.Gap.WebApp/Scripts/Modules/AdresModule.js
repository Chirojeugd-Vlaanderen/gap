/*
* Copyright 2013, Arno Soontjens
* Cleanup en refactoring met module pattern: Copyright 2015 Sam Segers
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


// MODULE Adres
var AdresModule = (function () {
    var postNummerElement;
    var landElement;
    var straatNaamElement;

    // Helper functie om de gemeente op te zoeken op postcode
    // Hangt de resultaten aan het opgegeven veld
    var postNrCache = {};
    var laadGemeenten = function (postcode, veld) {
        if (postcode.length < 4 || !isBelgieGeselecteerd()) {
            toonGemeenten('', veld);
        }
        else if (postcode in postNrCache) {
            toonGemeenten(postNrCache[postcode], veld);
        } else {
            //Groep ID wordt uit een verborgen veld op de pagina gehaald
            var url = link("Adressen", "WoonPlaatsenOphalen");
            $.getJSON(url, { postNummer: postcode }, function (data) {
                postNrCache[postcode] = data;
                toonGemeenten(data, veld);
            });
        }
    }
    // tonen in dropdown
    var toonGemeenten = function (data, veld) {
        var options = '';
        if (data == '') {
            options = '<option>Geen resultaten gevonden</option>';
        } else {
            for (var i = 0; i < data.length; i++) {
                options += '<option value="' + data[i].Naam + '">' + data[i].Naam + '</option>';
            }
        }

        $(veld).html(options);
    }

    // handle event: postnummer gewijzigd
    // => laad overeenkomstige gemeentes
    // => init autocomplete straat met postnr
    var postnummerGewijzigd = function () {
        var postNr = $(postNummerElement).val();
        if (postNr.length < 4 || !isBelgieGeselecteerd()) return; // niets doen
        laadGemeenten(postNr, '#WoonPlaatsNaam');
        autocomplete.InitForPostNr(postNr);
    }

    // initialiseer autocomplete 
    //  - (PostNr, StraatnaamDeel) => Straatnaam
    var autocomplete = {
        InitForPostNr : function (postnr) {
            var lastXhr;
            var stratenCache = {};
            $(straatNaamElement).autocomplete({
                minLength: 3,
                source: function (request, response) {
                    if (!isBelgieGeselecteerd()) return; // negeren indien niet belgie
                    var term = request.term;
                    if (term in stratenCache) {
                        response(stratenCache[term]);
                        return;
                    }
                    var url = link('Adressen', 'StratenVoorstellen');
                    lastXhr = $.getJSON(url, { q: term, postNummer: postnr }, function (data, status, xhr) {
                        stratenCache[term] = data;
                        if (xhr === lastXhr) { // counter concurrency
                            response(data);
                        }
                    });
                }
            });
        }
    }
    // Toon de juiste velden adhv geselecteerd land
    // indien land == Belgie ? Binnenland : Buitenland
    var binnenlandBuitenland = function () {
        $('#adrestabel').show();
        if (!isBelgieGeselecteerd()) {
            //show gegevens voor buitenland en gewone gegevens
            $('#postNrLabel').hide();
            $('#postCode').show();
            $('#uitlegBuitenland').show();
            $('#woonplaatsBuitenland').show();
            $('#uitlegBinnenland').hide();
            $('#woonplaatsBinnenland').hide();
        } else {
            $('#postNrLabel').show();
            $('#postCode').hide();
            $('#uitlegBuitenland').hide();
            $('#woonplaatsBuitenland').hide();
            $('#uitlegBinnenland').show();
            $('#woonplaatsBinnenland').show();

            // Events die niet zijn afgegaan tijdens dat het land niet belgie was, 
            // moeten mogelijks gecorrigeerd worden
            var postNr = $(postNummerElement).val();
            laadGemeenten(postNr, '#WoonPlaatsNaam');
            autocomplete.InitForPostNr(postNr);
        }
    };
    // Belgi is genoeg, want met die accentjes heb je niets dan miserie.
    var isBelgieGeselecteerd = function () {
        return $(landElement).val().substring(0, 5) == 'Belgi';
    };

    // public methods
    return {
        // Init event and change listeners
        Init: function () {
            // init often used html elements, to limit useless jQuery selector calls
            postNummerElement = document.getElementById('PostNr');
            straatNaamElement = document.getElementById('StraatNaamNaam');
            landElement = document.getElementById('Land');
            // Event listener op het wijzigen van land
            // We tonen andere data afhankelijk van binnen of buitenland
            binnenlandBuitenland();
            $(landElement).on('change', function () {
                binnenlandBuitenland();
            });
            postnummerGewijzigd();
            $(postNummerElement).on('change', function () {
                postnummerGewijzigd();
            });
        },
        // open a dialog to change/add an adress
        OpenDialog : function(url, title){
            clearDialog();
            $('#extraInfoDialog').load(url, function () {
                gedeeltelijkTonen('#extraInfoDialog');
                $('#adrestabel').show();
                // Door deze code kunnen users de form niet submitten met 'enter' (gaf een fout over de postcode)
                $(this).keydown(function (event) {
                    if (event.keyCode == 13) {
                        event.preventDefault();
                        return false;
                    }
                    return true;
                });
                AdresModule.Init(); // init adresfeatures in loaded window
                success:
                {
                    $('#extraInfoDialog fieldset').css('width', '600px');
                    $(this).dialog({
                        title: title,
                        modal: true,
                        width: 700,
                        height: 600,
                        resizable: true,
                        buttons: {
                            'Bewaren': function () {
                                $('#extraInfoDialog #bewaarAdres').click();
                                $(this).dialog('close');
                            },
                            'Annuleren': function () {
                                $(this).dialog('close');
                            }
                        }
                    });
                }
            });
        },
        OpenDialogVerwijderen: function (url) {
            clearDialog();
            $('#extraInfoDialog').load(url, function () {
                gedeeltelijkTonen('#extraInfoDialog');
                success:
                {
                    $("#extraInfoDialog").dialog({
                        modal: true,
                        title: "Adres verwijderen",
                        height: 250,
                        buttons: {
                            'Verwijderen': function () {
                                $('#extraInfoDialog #verwijderAdres').click();
                                $(this).dialog('close');
                            },
                            'Annuleren': function () {
                                $(this).dialog('close');
                            }
                        },
                        width: 500
                    });
                }
            });
        },
        OpenDialogVoorkeurAdres: function (voorkeursadresID, GPid) {
            bezig();
            url = link("Personen", "VoorkeurAdresMaken");
            $.get(url, { persoonsAdresID: voorkeursadresID, gelieerdePersoonID: GPid }, function () {
                success:
                    {
                        location.reload();
                    }
            });
        }
    }
}());
