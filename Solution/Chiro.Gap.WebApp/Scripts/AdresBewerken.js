/*
* Copyright 2013, Arno Soontjens
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

function AdresBewerken() {
    $('#uitlegBinnenland').show();
    var land = 'België';

    // We doen dit initieel, zodat de autocomplete al werkt alvorens
    // het postnummer wordt aangepast. Daarna doen we dat opnieuw iedere
    // keer er een relevante wijziging is.
    BinnenlandBuitenland();
    AdresAutocomplete();

    $('#Land').on('change', function () {
        BinnenlandBuitenland();
    });
    $('#PostNr').on('change', function () {
        AdresAutocomplete();
    });
}

// Vult drop down met beschikbare adressen
// en zet straat-autocomplete in gang.
function AdresAutocomplete() {
    var land = $('#Land').val();
    var pc = $("#PostNr").val();
    if (land.substring(0, 5) == 'Belgi') {
        toonGemeenten(pc, '#WoonPlaatsNaam');
    }

    var stratenCache = {};
    var lastXhr;

    $("input#StraatNaamNaam").autocomplete({
        minLength: 3,
        source: function (request, response) {
            var term = request.term;
            if (term in stratenCache) {
                response(stratenCache[term]);
                return;
            }
            var url2 = link('Adressen', 'StratenVoorstellen');
            lastXhr = $.getJSON(url2, { q: term, postNummer: pc }, function (data, status, xhr) {
                stratenCache[term] = data;
                if (xhr === lastXhr) {
                    response(data);
                }
            });
        }
    });
}

function BinnenlandBuitenland() {
    land = $("#Land").val();
    // Belgi is genoeg, want met die accentjes heb je niets dan 
    // miserie.
    if (land.substring(0, 5) != 'Belgi') {
        //show gegevens voor buitenland en gewone gegevens
        $('#uitlegBuitenland').show();
        $('#uitlegBinnenland').hide();
        $('#tabel').show();
        $('#postCode').show();
        $('#woonplaatsBuitenland').show();
        //hide gegevens voor binnenland
        $('#woonplaatsBinnenland').hide();
    } else {
        $('#woonplaatsBinnenland').show();
        $('#uitlegBuitenland').hide();
        $('#uitlegBinnenland').show();
        $('#tabel').show();
        $('#postCode').hide();
        $('#woonplaatsBuitenland').hide();
    }
}