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
    
    $('#knopBewaren, #knopReset').button();

   
    //Datepicker op het veld met geboortedatum
    $('#HuidigePersoon_GeboorteDatum').datepicker({
        showAnim: "slide",
        showButtonPanel: true,
        changeYear: true,
        changeMonth: true,
        maxDate: "-6y"
    });
    $.datepicker.setDefaults($.datepicker.regional['be']);

    // Maak van de radiobutton om het geslacht te kiezen een JQuery UI buttonset en geeft een vaste breedte.
    // De breedte staat op het label, zie structuur van de nieuwe JQuery UI radiobuttons.
    $('#geslacht').buttonset().find('label').width(99);
    
    //Form resetten
    $('#knopReset').click(function () {
        $('#confirmDialog').dialog({
            modal: true,
            buttons: {
                "Ja, verwijder alles": function () {
                    $('#form0')[0].reset();
                    $('.validation-summary-errors, .field-validation-error').hide();
                    $(this).dialog("close");

                },
                "Annuleren": function () {
                    $(this).dialog("close");
                }
            }
        });
    });

    // MEER INFORMATIE TONEN ( via dialog)

    //extra info over gezinsgebonden communicatievormen
    $('#gezinsCom').click(function () {
        toonInfo('#GB-COMINFO', "Gezinsgebonden communicatievormen", "#extraInfoDialog");
    });

    //extra info over broer/zus maken
    $('#zusBroer').click(function () {
        var url = "/Handleiding/ZusBroer #help" ;
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
            title: "Broer /Zus"
        });
    });
    
});


