/*
 * Copyright 2013, Arno Soontjens
 * Copyright 2014, the GAP developers. See the NOTICE file at the
 * top-level directory of this distribution, and at
 * https://develop.chiro.be/gap/wiki/copyright
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

var GID;
$(function () {

    //defaults
    $.fn.editable.defaults.mode = 'inline';
    $.fn.editable.defaults.clear = true;
    $.fn.editable.defaults.toggle = 'manual';

    GID = $('#MGID').text();
    var url;

    $('.ui-dialog').on('dialogbeforeclose', function () {
        $('#extraInfoDialog').html('<img src="' + root + 'Content/images/loading.gif"/>');
    });

    //mededelingen
    $('.mededelingen').click(function () {
        url = $(this).find("a").attr("href");
        window.location = url;
    });
    //--------------------------------------------------------------------------------
    // MEER INFORMATIE TONEN ( via dialog)
    // Deze functies kunnen in dit algemene bestand opgenomen worden omdat ze in enkele paginas
    // op exact dezelfde manier voorkomen. De ID's op de links en de <div> tag 
    // waarin de info geladen wordt moeten dan wel op alle pagina's dezelfde naam hebben.
    //--------------------------------------------------------------------------------
    // extra info over AD-nummer
    //TODO: Volgens mij moet dit efficienter kunnen
    $('#clInfo').click(function () {
        $('#extraInfoDialog').dialog();
        toonInfo('#CLINFO', "Chiroleeftijd", "#extraInfoDialog");
    });

    $('#Ad-info').click(function () {
        $('#extraInfoDialog').dialog();
        toonInfo('#ADINFO', 'AD-nummer', '#extraInfoDialog');
    });

    $('#instapperiodeInfo').click(function () {
        $('#extraInfoDialog').dialog();
        toonInfo('#INSINFO', 'Instapperiode', '#extraInfoDialog');
    });

    $('#clInfo').click(function () {
        $('#extraInfoDialog').dialog();
        toonInfo('#CLINFO', 'Chiroleeftijd', '#extraInfoDialog');
    });

    $('#snelBerichtInfo').click(function () {
        $('#extraInfoDialog').dialog();
        toonInfo('#SBLINFO', 'Snelleberichtenlijsten', '#extraInfoDialog');
    });

    $('#print').click(function () {
        window.print();
    });

    $('#lidgeldInfo').click(function () {
        $('#extraInfoDialog').dialog();
        toonInfo('#LGINFO', "Lidgeld", "#extraInfoDialog");
    });
}); 
//--------------------------------------------------------------------------------
// functie om extra informatie te tonen in een dialog
// Maak een <div id="eenID"></div> aan in de pagina waar je de dialog wil
// doen verschijnen en geef het id mee in de functie.
//--------------------------------------------------------------------------------
function toonInfo(id, titel, dialogId) {
    clearDialog();
    url = link("Handleiding", "Trefwoorden");
    url = url + ' ' + id; 
    $(dialogId).load(url, function () {
            $(dialogId).dialog({
                modal: true,
                width: 500,
                show: {
                    effect: "drop"
                },
                hide: {
                    effect: "drop"
                },
                title: titel,
                buttons: {}
            });
        });
}

//--------------------------------------------------------------------------------
//default waarden voor de datePicker, zorgt ervoor dat hij in het nederlands staat.
//--------------------------------------------------------------------------------
$.datepicker.regional['be'] = {
    closeText: 'Sluiten',
    prevText: '←',
    nextText: '→',
    currentText: 'Vandaag',
    monthNames: ['Januari', 'Februari', 'Maart', 'April', 'Mei', 'Juni',
                'Juli', 'Augustus', 'September', 'Oktober', 'November', 'December'],
    monthNamesShort: ['Jan', 'Feb', 'Maa', 'Apr', 'Mei', 'Jun',
                'Jul', 'Aug', 'Sep', 'Okt', 'Nov', 'Dec'],
    dayNames: ['Zondag', 'Maandag', 'Dinsdag', 'Woensdag', 'Donderdag', 'Vrijdag', 'Zaterdag'],
    dayNamesShort: ['Zon', 'Maa', 'Din', 'Woe', 'Don', 'Vri', 'Zat'],
    dayNamesMin: ['zo', 'ma', 'di', 'wo', 'do', 'vr', 'za'],
    weekHeader: 'Wk',
    dateFormat: 'dd/mm/yy',
    firstDay: 1,
    isRTL: false,
    showMonthAfterYear: false,
    yearSuffix: ''
};
//-------------------------------------------------------------------------
//Functie 'bezig met verwerking'

function bezig() {
    $('#extraInfoDialog').dialog({
        title: 'Verwerking bezig',
        modal: true,
        height: 150,
        closeOnEscape: false,
        draggable: false,
        resizable: false,
        buttons: {}
    });
}

// TODO (#1812): Standaardmanier van feedback.

//-------------------------------------------------------------------------
//Functie 'Klaar met success'
function klaarMetSucces(boodschap) {
    $('#extraInfoDialog').html("<p>" + boodschap + "</p>").dialog({
        title: "Succesvol verwerkt",
        modal: true,
        height: 150,
        closeOnEscape: true,
        draggable: false,
        resizable: false,
        buttons: {}
    });
}
//-------------------------------------------------------------------------
//Functie 'Klaar met fout'
function klaarMetFout(boodschap) {
    $('#extraInfoDialog').html("<p>" + boodschap + "</p>").dialog({
        title: "Fout",
        modal: true,
        height: 150,
        closeOnEscape: true,
        draggable: false,
        resizable: false,
        buttons: {}
    });
}
//-------------------------------------------------------------------------
function clearDialog() {
    $('#extraInfoDialog').html('<img src="' + root +'Content/images/loading.gif"/>')
            .dialog({
                title: "Laden...",
                buttons: {},
                modal: true,
                height: 200,
                width: 250,
                show: {
                    effect: "drop"
                },
                hide: {
                    effect: "drop"
                },
                position: {
                    my: "center",
                    at: "center",
                    of: window
                },
                dialogClass: {}
            });
}

//------------------------------------------------------------------------------------------
function gedeeltelijkTonen(container) {
    // Arno gebruikt dit script om zaken te verbergen als hij 'oude' pagina's laadt in
    // een dialoog in de nieuwe frontend.
    // Let op dat je niet de hele oude pagina mag laden, want het 'gedeeltelijk tonen' 
    // verbergt wel wat er staat, maar verhindert niet dat scripts dubbel ingelezen en
    // uitgevoerd worden.
    // jquery.load() ondersteunt het enkel inlezen van een bepaalde div; doe dat om 
    // miserie te vermijden
    
    $(container).find('#header, #footer, .mededelingen, legend, h2, #acties').hide();
    $(container + ' fieldset').css({ 'width': '350px' });
}
//-------------------------------------------------------------------------
//functie om de gemeente op te zoeken

function toonGemeenten(postcode, veld) {
    //Groep ID wordt uit een verborgen veld op de pagina gehaald
    url = link("Adressen", "WoonPlaatsenOphalen");
    var options = '';
    $.getJSON(url, { postNummer: postcode }, function (data) {
        if (data == '') {
            options = '<option>Geen resultaten gevonden</option>';
        } else {
            for (var i = 0; i < data.length; i++) {
                options += '<option value="' + data[i].Naam + '">' + data[i].Naam + '</option>';
            }
        }

        $(veld).html(options);
    });

}
//-------------------------------------------------------------------------
// email valideren
function controleerEmail(email) {
    var emailReg = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    if (!emailReg.test(email)) {
        return "Het ingegeven e-mail adres is ongeldig. Gebruik de vorm: 'voorbeeld@voorbeeld.xxx'";
    }
    return "";
}
//------------------------------------------------------------------------------------------
// telefoonnummer/ Fax /msn valideren
function controleerTel(tel) {
    var telReg = /^(^0[0-9]{1,2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$|^04[0-9]{2}\-[0-9]{2,3}\s?[0-9]{2}\s?[0-9]{2}$)|^\+[0-9]*$/;
    if (!telReg.test(tel)) {
        return "Het ingegeven telefoonnummer heeft een ongeldig formaat. Voorbeelden: 02-345 67 89, 014-56 78 89, +323456789";
    }
    return "";
}

function link(controller, functie) {
    var link;
    if (controller != "") {
        link = root + GID + "/" + controller + "/" + functie;
    } else {
        link = root + GID + "/" + functie;
    }
    return link;
}

//check het procotol ('http:' of 'https:')
function checkProtocol() {
    return window.location.protocol;
}

// Url parameter opvragen
// Credits: http://stackoverflow.com/questions/901115/how-can-i-get-query-string-values/901144#901144
function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results == null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}