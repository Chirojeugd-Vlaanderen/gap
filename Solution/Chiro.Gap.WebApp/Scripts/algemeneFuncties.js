$(function() {
//defaults
$.fn.editable.defaults.mode = 'inline';
$.fn.editable.defaults.clear = true;
$.fn.editable.defaults.toggle = 'manual';

    var GID = $('#MGID').val();

    //mededelingen
    $('.mededelingen').click(function () {
        var url = "/" + GID + "/GavTaken";
        window.location = url;
    });
});    
//--------------------------------------------------------------------------------
// functie om extra informatie te tonen in een dialog
// Maak een <div id="eenID"></div> aan in de pagina waar je de dialog wil
// doen verschijnen en geef het id mee in de functie.
//--------------------------------------------------------------------------------
function toonInfo(id, titel, dialogId) {

    var url = "/Handleiding/Trefwoorden " + id;
    $(dialogId).load(url, function () {
        success: {
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
        }
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
//Straatnamen aanvullen


//--------------------------------------------------------------------------------
// MEER INFORMATIE TONEN ( via dialog)
// Deze functies kunnen in dit algemene bestand opgenomen worden omdat ze in enkele paginas
// op exact dezelfde manier voorkomen. De ID's op de links en de <div> tag 
// waarin de info geladen wordt moeten dan wel op alle pagina's dezelfde naam hebben.
//--------------------------------------------------------------------------------
// extra info over AD-nummer

/*$('#Ad-info').click(function () {
    toonInfo('#ADINFO', "AD-nummer", "#extraInfoDialog");
});

$('#clInfo').click(function () {
    toonInfo('#CLINFO', "Chiroleeftijd", "#extraInfoDialog");
});*/

