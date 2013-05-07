
var GID;
$(function () {
    //defaults
    $.fn.editable.defaults.mode = 'inline';
    $.fn.editable.defaults.clear = true;
    $.fn.editable.defaults.toggle = 'manual';

    GID = $('#MGID').val();
    var url;
    
    //mededelingen
    $('.mededelingen').click(function () {
        url = link("", 'GavTaken');
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
        clearDialog();
    });

    $('#Ad-info').click(function () {
        $('#extraInfoDialog').dialog();
        toonInfo('#ADINFO', 'AD-nummer', '#extraInfoDialog');
        clearDialog();
    });

    $('#instapperiodeInfo').click(function () {
        $('#extraInfoDialog').dialog();
        toonInfo('#INSINFO', 'Instapperiode', '#extraInfoDialog');
        clearDialog();
    });

    $('#clInfo').click(function () {
        $('#extraInfoDialog').dialog();
        toonInfo('#CLINFO', 'Chiroleeftijd', '#extraInfoDialog');
        clearDialog();
    });

    $('#print').click(function () {
        window.print();
    });

    $('#lidgeldInfo').click(function () {
        $('#extraInfoDialog').dialog();
        toonInfo('#LGINFO', "Lidgeld", "#extraInfoDialog");
        clearDialog();
    });
}); 
//--------------------------------------------------------------------------------
// functie om extra informatie te tonen in een dialog
// Maak een <div id="eenID"></div> aan in de pagina waar je de dialog wil
// doen verschijnen en geef het id mee in de functie.
//--------------------------------------------------------------------------------
function toonInfo(id, titel, dialogId) {

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
//-------------------------------------------------------------------------
function clearDialog() {
    $('#extraInfoDialog').html('<img src="/Content/images/loading.gif"/>')
            .dialog({
                title: "Laden...",
                buttons: {},
                modal: true,
                width: 300,
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


function adresToevoegen(GID, GPid) {
    $('#extraInfoDialog').dialog();

    url = link("Personen", "NieuwAdres");
    url = url + "/" + GPid + " #main";
    
    $('#extraInfoDialog').load(url, function () {
        gedeeltelijkTonen('#extraInfoDialog');
        $('#tabel').show();
        $('#uitlegBinnenland').show();
        var land = 'België';

        // Door deze code kunnen users de form niet submitten met 'enter' (gaf een fout over de postcode)
        $(this).keydown(function (event) {
            if (event.keyCode == 13) {
                event.preventDefault();
                return false;
            }
        });

        $('#Land').on('change', function () {
            land = $(this).val();
            if (land != 'België') {
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
        });
        $('#PostNr').on('change', function () {
            var pc = $(this).val();
            if (land == 'België') {
                toonGemeenten(pc, '#WoonPlaats');
            }
        });

        success:
        {
            $('#extraInfoDialog fieldset').css('width', '600px');
            $(this).dialog({
                title: "Adres Toevoegen",
                modal: true,
                width: 700,
                height: 700,
                resizable: true,
                buttons: {
                    'Bewaren': function () {
                        $('#extraInfoDialog #bewaarAdres').click();
                        $(this).dialog('close');
                    },
                    'Annuleren': function () {
                        $(this).dialog('destroy');
                        $(this).dialog('close');
                    }
                }
            });
        }
    });
}

//------------------------------------------------------------------------------------------
function gedeeltelijkTonen(container) {
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
        return "Het ingegeven telefoonnummer is ongeldig. Gebruik de vorm: '000-00 00 00' of '0000-00 00 00'";
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