/*
Frontend javascript (JQuery)
GAP, Chirojeugd Vlaanderen
2013
*/

//--------------------------------------------------------------------------------
//Document ready functie
//--------------------------------------------------------------------------------
$(function () {
    var GID = $('#np_groepID').val();
    var gpId = 0;
    var url;
    var doorgaan = false;
    var chiroleeftijd;

    $('#knopBewaren').button({
        icons: {
            primary: 'ui-icon-disk'
        }
    });
    $('#knopReset').button({
        icons: {
            primary: 'ui-icon-arrowrefresh-1-s'
        }
    });

    // Door deze code kunnen users de form niet submitten met 'enter'
    $(this).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });
    $('#andereGegevens').hide();
    $('#btn_verder').button();
    $('#btn_verder').attr('disabled', false);

    //Datepicker op het veld met geboortedatum
    $('#HuidigePersoon_GeboorteDatum').datepicker({
        showAnim: "slide",
        changeYear: true,
        changeMonth: true,
        maxDate: "-5y"
    });
    $.datepicker.setDefaults($.datepicker.regional['be']);

    // Maak van de radiobuttons een JQuery UI buttonset en geeft een vaste breedte.
    // De breedte staat op het label, zie structuur van de nieuwe JQuery UI radiobuttons.
    $('#geslacht').buttonset().find('label').width(99);
    $('#type').buttonset().find('label').width(99);

    $('#andereGegevens input').attr('enabled', false);

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

    //Layout en attributen van de inputvelden
    $('#HuidigePersoon_VoorNaam').attr('required', true).attr('type', "text");
    $('#HuidigePersoon_Naam').attr('required', true).attr('type', "text");
    $('#HuidigePersoon_GeboorteDatum').attr('required', true).attr('type', 'date');
    $('#HuidigePersoon_GeboorteDatum').width(196);

    //variabelen voor het maken van een nieuw lid
    var voornaam, vnCheck;
    var naam, nmCheck;
    var geboortedatum, gdCheck;
    var geslacht;
    var errors;

    //------------------------------------------------------------------------------------------------------------
    //klik op 'ga verder'
    //------------------------------------------------------------------------------------------------------------
    $('#btn_verder').click(function (e) {
        e.preventDefault();
        //'bezig met verwerking tonen
        verwerkNieuwePersoon();

        //Variabelen invullen
        voornaam = $('#HuidigePersoon_VoorNaam').val();
        voornaam.length >= 2 ? vnCheck = true : vnCheck = false;
        naam = $('#HuidigePersoon_Naam').val();
        naam.length >= 2 ? nmCheck = true : nmCheck = false;
        geboortedatum = $('#HuidigePersoon_GeboorteDatum').val();
        geboortedatum.length >= 8 ? gdCheck = true : gdCheck = false;
        $('#vrouw').is(':checked') ? geslacht = "vrouw" : geslacht = "man";

        //------------------------------------------------------------------------------------------------------------
        //check of alle nodige velden ingevuld zijn (geslacht niet ingevuld = man)
        if (vnCheck && nmCheck && gdCheck) {
            url = "/" + GID + "/Personen/Nieuw";
            $.post(url, {
                "HuidigePersoon.VoorNaam": voornaam,
                "HuidigePersoon.Naam": naam,
                "HuidigePersoon.GeboorteDatum": geboortedatum,
                "HuidigePersoon.Geslacht": geslacht
            }, function (data) {
                success:
                {
                    //------------------------------------------------------------------------------------------------------------
                    //als 'data[0]' undefined is zit het niet in het JSON antwoord en is er dus geen dezelfde persoon
                    if (typeof (data[0]) === "undefined") {
                        gpId = data.GelieerdePersoonID;
                        doorgaan = true;
                        clearDialog();
                        initialiseerAndereVelden(gpId);

                    } else { //'data[0]' is gezet, dus er is al een persoon met dezelfde naam 
                        errors =
                            "Pas op! Je nieuwe persoon <b>(" + data[0].VolledigeNaam + ")</b> lijkt verdacht veel op iemand die al gekend is in " +
                                "de Chiroadministratie. Als je zeker bent dat je niemand dubbel toevoegt, klik " +
                                "dan opnieuw op 'Bewaren'";
                        $('#extraInfoDialog').html(errors);
                        //vraag of je deze persoon wil toevoegen
                        $('#extraInfoDialog').dialog({
                            modal: true,
                            title: "Gelijkaardige persoon",
                            height: 200,
                            buttons: {
                                //ja --> voeg toe (forceer = true)
                                'Bewaren': function () {
                                    $.post(url, {
                                        "HuidigePersoon.VoorNaam": voornaam,
                                        "HuidigePersoon.Naam": naam,
                                        "HuidigePersoon.GeboorteDatum": geboortedatum,
                                        "HuidigePersoon.Geslacht": geslacht,
                                        Forceer: true
                                    }, function (resultaat) {
                                        if (resultaat.GelieerdePersoonID != "") {
                                            $(this).dialog('close');
                                            errors = "De wijzigingen zijn opgeslagen";
                                            $('#errorfield').html(error).show();
                                        }
                                    });
                                    gpId = data.GelieerdePersoonID;
                                    doorgaan = true;
                                    clearDialog();
                                    initialiseerAndereVelden(gpId);

                                },
                                // nee --> velden leegmaken en pagina herladen
                                'Nee': function () {
                                    $('#HuidigePersoon_VoorNaam').val('');
                                    $('#HuidigePersoon_Naam').val('');
                                    $('#HuidigePersoon_GeboorteDatum').val('');
                                    $('#HuidigePersoon_GeboorteDatum').val('');
                                    location.reload();
                                    $(this).dialog('destroy');
                                    $(this).dialog('close');
                                }
                            }
                        });
                    }
                    //------------------------------------------------------------------------------------------------------------
                }
            });
            // een of meerdere van de nodige velden werden niet ingevuld --> errors weergeven
        } else {
            $(".ui-dialog-content").dialog("close");
            errors = '<img src="/Content/images/Exclamation.png" />';
            if (voornaam == "") {
                errors += "<li>U probeert een persoon toe te voegen zonder voornaam.</li>";
            }
            if (naam == "") {
                errors += "<li>U probeert een persoon toe te voegen zonder naam.</li>";
            }
            if (geboortedatum == "") {
                errors += "<li>U probeert een persoon toe te voegen zonder geboortedatum.</li>";
            }
            if (geslacht == "") {
                errors += "<li>U probeert een persoon toe te voegen zonder geslacht.</li>";
            }
            $('#errorfield').html('<b>Fouten:</b> <ul>' + errors + '</ul>');
            $('#errorfield').show();
        }
    });

    //------------------------------------------------------------------------------------------------------------

    //Lid inschrijven en toekennen van type wanneer Lid/leiding wordt aangeklikt
    $('#type').change(function () {
        if (doorgaan) {
            url = "/" + GID + "/Personen/Inschrijven?gelieerdePersoonID=" + gpId + " #main";
            $('#extraInfoDialog').dialog();
            $('#extraInfoDialog').load(url, function () {
                gedeeltelijkTonen();
                $(this).find('fieldset').css('width', '100%');
                $('#extraInfoDialog').dialog({
                    title: "Inschrijven",
                    width: 600,
                    height: 500,
                    buttons: {
                        'Schrijf in': function () {
                            $('#bewaar').click(function (e) {
                                e.preventDefault();
                            });
                        },
                        'Annuleren': function () {
                            $(this).dialog('destroy');
                            $(this).dialog('close');
                        }
                    }
                });
            });
            clearDialog();
        }
    });

    //chiroleeftijd veranderen
    $('#select_chiroleeftijd').change(function () {
        if (doorgaan) {
            chiroleeftijd = $(this).val();
            alert(chiroleeftijd);
        }
    });

    //toon de adres dialog bij het klikken in de input
    $('#np_adres').focus(function () {
        if (doorgaan) {
            adresToevoegen(GID, gpId);
        }
    });
    //------------------------------------------------------------------------------------------------------------
    //functie om het dialoogvenster tijdens de verwerking te tonen en mededelingen te verbergen.
    function verwerkNieuwePersoon() {
        $('#extraInfoDialog').dialog({
            title: 'Verwerking bezig',
            modal: true,
            height: 150,
            closeOnEscape: false,
            draggable: false,
            resizable: false,
            buttons: {},
            dialogClass: "noclose"
        });
        $('.mededelingen').hide();
    }
    //------------------------------------------------------------------------------------------------------------
    // MEER INFORMATIE TONEN ( via dialog)

    //extra info over gezinsgebonden communicatievormen
    $('#gezinsCom').click(function () {
        toonInfo('#GB-COMINFO', "Gezinsgebonden communicatievormen", "#extraInfoDialog");
    });

    //extra info over broer/zus maken
    $('#zusBroer').click(function () {
        var url = "/Handleiding/ZusBroer #help";
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
    //------------------------------------------------------------------------------------------------------------

});
//------------------------------------------------------------------------------------------------------------
function initialiseerAndereVelden(gpId) {
    $(".ui-dialog-content").dialog("close");
    $('#btn_verder').attr('enabled', true).hide();
    $('#andereGegevens').show();
    $('#andereGegevens').find(':input').attr('disabled', false);
    $('#knopReset').attr('disabled', true);
    $('#knopReset').hide();
    $('#knopBewaren').show();
    $('#errorfield').hide();
    haalGegevens(gpId);
}
//------------------------------------------------------------------------------------------------------------
function haalGegevens(gelieerdePersoon) {
    //Alle gegevens van de net aangemaakte persoon ophalen
    var url = "/" + GID + "/Personen/PersoonsGegevensOphalenJson";
    var persoonId, werkjaar;
    $.getJSON(url, { gelieerdePersoonId: gelieerdePersoon }, function (data) {
        persoonId = data.HuidigePersoon.PersoonID;
        werkjaar = data.HuidigWerkJaar;
    });
    return (persoonId, werkjaar);
}
//------------------------------------------------------------------------------------------------------------