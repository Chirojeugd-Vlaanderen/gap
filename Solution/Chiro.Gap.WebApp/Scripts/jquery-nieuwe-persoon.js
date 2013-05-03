/*
Frontend javascript (JQuery)
GAP, Chirojeugd Vlaanderen
2013
*/
//variabelen voor het maken van een nieuw lid
var volledigeNaam, voornaam, naam, geboortedatum, geslacht;
var postcode, gemeente, straat, nummer, bus;
var persoonID, lidID, werkjaar;

var telefoonnummer;
var emailadres;

var chiroleeftijd, type;
var afdelingsNamen = [];
var afdelingsIDs = [];
var afdelingsJaarIDs = [];
var geselecteerdeAfdelingen = [];
var beschikbareAfdelingen = [];
var np_gpID = 0;
var inschrijven=false;

var tel =false;
var email = false;

var errors;
var url;
var fout = false;
//--------------------------------------------------------------------------------
//Document ready functie
//--------------------------------------------------------------------------------
$(function () {
    //werkjaar invullen op groepen op te halen
    werkjaar = $('#np_werkJaarID').val();
    
    //------------------------------------------------------------------------
    // afdelingsinfo binnenhalen
    //actieve afdelingen met hun (speciale) namen ophalen
    url = "/" + GID + "/Afdelingen/AfdelingsInfo";
    $.post(url, {groepsWerkJaarID: werkjaar},function(antw) {
        $.each(antw.Actief, function(index, value) {
            var naam = antw.Actief[index].AfdelingNaam;
            var id = antw.Actief[index].AfdelingID;
            var afdJrId = antw.Actief[index].AfdelingsJaarID;
            afdelingsNamen.push(naam);
            afdelingsIDs.push(id);
            afdelingsJaarIDs.push(afdJrId);
        });
    }).done(function() {
        $.each(afdelingsNamen, function(index, value) {
            $('#afdelingSelectie')
                .append(
                    '<input ' +
                        'type="radio" ' +
                        'name="radioAfd" ' +
                        'id="' + afdelingsNamen[index]+ 'Check" ' +
                        'value="' + afdelingsJaarIDs[index] + 
                     '"/>' +
                     '<label ' +
                        'for="'+afdelingsNamen[index]+'Check">'+ afdelingsNamen[index] +'</label>');
        });
        alert(afdelingsJaarIDs);
        // deze knoppen krijgen hier het JQuery UI uitzicht, anders wordt de code uitgevoerd voordat alles geïmporteerd is, 
        // en werkt die layout niet
        $('#afdelingSelectie').buttonset();
    });

    //JQuery - UI: knoppen en buttonsets aanmaken (is vooral voor het uitzicht)
    $('#ja').button({
        icons: {
            primary: 'ui-icon-check'   
        }
    });
    
    $('#nee').button({
         icons: {
            primary: 'ui-icon-closethick'   
        }
    });
    $('#tochNiet').button({
         icons: {
            primary: 'ui-icon-closethick'   
        }
    });

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

    $('#geslacht').buttonset().find('label').width(99);
    
    $('#type').buttonset().find('label').width(99);

    $('#andereGegevens input').attr('enabled', false);


    //Disable perongeluk te vroeg submitten door 'enter' te drukken
    $(this).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });
    //------------------------------------------------------------------------------------------------------------
    //Form resetten
    $('#knopReset').click(function (e) {
        e.preventDefault();
        $('#confirmDialog').dialog({
            modal: true,
            buttons: {
                "Ja, verwijder alles": function () {
                    $('fieldset').find('input').val('');
                    $('fieldset').find('select').val('');
                    $('fieldset').find('label').removeClass('ui-state-active');
                    $('fieldset').find('input').attr('checked', false);
                    $('.validation-summary-errors, .field-validation-error').hide();
                    $('#errorfield').hide();
                    location.reload();
                    $(this).dialog("close");
                },
                "Annuleren": function () {
                    $(this).dialog("close");
                }
            }
        });
    });
    /*------------------------------------------------------------------------------------------------------------

    $('#np_adresToevoegen').click(function () {
    adresTeller++;
    $("<tr><td><strong>Adres " + (adresTeller + 1) + "</strong></td></tr>" +
    "<tr class='np_adres' > " +
    "<td>Adres:</td>" +
    "<td>" +
    'Postcode: <input type="text" class="np_postCode" size="6"/>' +
    ' Gemeente: <select id="np_gemeente"></select>' +
    '<br/>' +
    '<br/>' +
    'Straat: <input type="text" id="np_straat"/>' +
    ' Nr: <input type="text" size="6" id="np_nummer"/> Bus: <input type="text" id="np_bus" size="5"/>' +
    '</td>' +
    '</tr>').insertAfter('.np_adres:last');
    });*/
    //------------------------------------------------------------------------------------------------------------
    //Persoon inschrijven of niet?
    $('#ja').click(function() {
        $('#chiroGegevens p, #tochInschrijven').hide();
        $('#chiroGegevens table, #tochNiet').show();
        inschrijven = true;
    });
    
    $('#nee').click(function() {
        $('#chiroGegevens').hide();
        $('#tochInschrijven').button();
        $('#tochInschrijven').show();
        inschrijven = false;
    });

    $('#tochInschrijven').click(function() {
        $(this).hide();
        $('#chiroGegevens, #chiroGegevens table, #tochNiet').show();
        $('#chiroGegevens p, #tochInschrijven').hide();
        inschrijven = true;
    });

    $('#tochNiet').click(function() {
        $('#chiroGegevens').hide();
        $('#tochInschrijven').button();
        $('#tochInschrijven').show();
        inschrijven = false;
    });
    
    //------------------------------------------------------------------------------------------------------------
    $('#np_telToevoegen').click(function () {
        $('#eersteTel').show();
        tel= true;
        $(this).parent().parent().hide();
    });
    //------------------------------------------------------------------------------------------------------------

    $('#np_emailToevoegen').click(function () {
        $('#eersteEmail').show();
        email = true;
        $(this).parent().parent().hide();
    });
    //------------------------------------------------------------------------------------------------------------
    //Layout en attributen van de inputvelden
    $('#HuidigePersoon_VoorNaam').attr('required', true).attr('type', "text");
    $('#HuidigePersoon_Naam').attr('required', true).attr('type', "text");
    $('#HuidigePersoon_GeboorteDatum').attr('required', true).attr('type', 'date');
    $('#HuidigePersoon_GeboorteDatum').width(196);

    //------------------------------------------------------------------------------------------------------------
    //chiroleeftijd veranderen
    $('#select_chiroleeftijd').change(function () {
        chiroleeftijd = $(this).val(this);
    });

    //------------------------------------------------------------------------------------------------------------
    //
    $('#np_postCode').keyup(function () {
        postcode = $(this).val();
        toonGemeenten(postcode, '#np_gemeente');
    });
  
    //------------------------------------------------------------------------
    
    //------------------------------------------------------------------------------------------------------------
    //klik op knop 'BEWAREN'
    //------------------------------------------------------------------------------------------------------------  
    $('#knopBewaren').click(function (e) {
        e.preventDefault();
        var test;
        var doorgaan;

        vulVariabelenIn();
        
        //--------------------------------------------------------------------------------------------------------
        //Controle van de ingevulde variabelen
        //--------------------------------------------------------------------------------------------------------
        test = controle();
        alert(test);
        if (test) {
            $('#errorMessages').html(errors);
            $('#errorfield').show();
        } else {
            $('#errorfield').hide();
            doorgaan = true;
        }
        
        //--------------------------------------------------------------------------------------------------------
        //Maak de nieuwe persoon aan (als er geen fouten zijn)
        //--------------------------------------------------------------------------------------------------------
        if (doorgaan) {
            var url = "/" + GID + "/Personen/Nieuw";

            $.post(url, {
                    "HuidigePersoon.VoorNaam": voornaam,
                    "HuidigePersoon.Naam": naam,
                    "HuidigePersoon.GeboorteDatum": geboortedatum,
                    "HuidigePersoon.Geslacht": geslacht
                }, function(data) {
                    success:
                    {
                        //als 'data[0]' undefined is zit het niet in het JSON antwoord en is er dus geen dezelfde persoon
                        if (typeof(data[0]) === "undefined") {
                            np_gpID = data.GelieerdePersoonID;
                            //----------------------------------------------------------------------------------------
                            if (tel) {
                                url = "/" + GID + "/Personen/NieuweCommVorm";
                                $.post(url, {
                                    gelieerdePersoonID: np_gpID,
                                    "NieuweCommVorm.CommunicatieTypeID": 1,
                                    "NieuweCommVorm.Nummer": telefoonnummer,
                                    "NieuweCommVorm.Voorkeur": true
                                });
                            }
                             //----------------------------------------------------------------------------------------
                            if (email) {
                                url = "/" + GID + "/Personen/NieuweCommVorm";
                                $.post(url, {
                                    gelieerdePersoonID: np_gpID,
                                    "NieuweCommVorm.CommunicatieTypeID": 3,
                                    "NieuweCommVorm.Nummer": emailadres ,
                                    "NieuweCommVorm.Voorkeur": true
                                });
                            }
                            //----------------------------------------------------------------------------------------
                            //Persoon inschrijven
                            if (inschrijven) {
                                    url = "/" + GID + "/Leden/LedenMaken";
                                    $.ajax({
                                        url: url,
                                        traditional: true,
                                        type: 'POST',
                                        data: {
                                            "PersoonEnLidInfos[0].GelieerdePersoonID": np_gpID,
                                            "PersoonEnLidInfos[0].InTeSchrijven": true,
                                            "PersoonEnLidInfos[0].LeidingMaken": true,
                                            "PersoonEnLidInfos[0].VolledigeNaam": volledigeNaam,
                                            "PersoonEnLidInfos[0].AfdelingsJaarIDs": afdelingsJaarIDs,
                                            "BeschikbareAfdelingen": beschikbareAfdelingen,
                                            checkall: false,
                                        }
                                    }).done(function() {
                                    //Alle gegevens van de net ingeschreven persoon ophalen
                                    url = "/" + GID + "/Personen/PersoonsGegevensOphalenJson";
                                    $.getJSON(url, { gelieerdePersoonId: np_gpID }, function(res) {
                                        persoonID = res.HuidigePersoon.PersoonID;
                                        werkjaar = res.HuidigWerkJaar;
                                        lidID = res.HuidigePersoon.LidID;
                                        volledigeNaam = res.HuidigePersoon.volledigeNaam;
                                    }).done(function() {
                                        $.each($('#afdelingSelectie input:checked'), function(index, value) {
                                            var waarde = parseInt($(this).val());
                                            geselecteerdeAfdelingen.push(waarde);
                                        });
                                        //'beschikbare afdelingen' uit het model halen
                                        url = "/" + GID + "/Leden/AfdelingBewerken";
                                        $.getJSON(url, { lidId: lidID }, function(antwoord) {
                                            beschikbareAfdelingen.push(antwoord.BeschikbareAfdelingen);
                                        }).done(function() {
                                            url = "/" + GID + "/Leden/AfdelingBewerken";
                                            $.ajax({
                                                url: url,
                                                type: 'POST',
                                                traditional: true,
                                                data: {
                                                    "groepsWerkJaarID": werkjaar,
                                                     lidID: lidID,
                                                     "Info.AfdelingsJaarIDs": geselecteerdeAfdelingen
                                                }
                                            }).done(function() {
                                                url = "/" + GID + "/Personen/EditRest/" + np_gpID;
                                                window.location = url;
                                            });
                                        });
                                    });

                                  
                                });
                            }
                        
                    

        //----------------------------------------------------------------------------------------
                        //adres toekennen
                        url = "/" + GID + "/Personen/NieuwAdres/" + np_gpID;
                        $.post(url, {
                            action:'Bewaren',
                            GelieerdePersoonIDs:np_gpID,
                            "PersoonsAdresInfo.AdresType":'Thuis',
                            Land:'België',
                            PostNr:postcode,
                            Straat:straat,
                            HuisNr:nummer,
                            Bus:bus,
                            WoonPlaats:gemeente,
                            Voorkeur:true,
                            AanvragerID:np_gpID
                        });
                        //----------------------------------------------------------------------------------------
                        

                    } else { //'data[0]' is gezet, dus er is al een persoon met dezelfde naam 
                        alert("Er is al zo'n persoon");
                    }
                }
            });
        }
        
        //--------------------------------------------------------------------------------------------------------


        //------------------------------------------------------------------------------------------------------------
    }); //EINDE BEWAREN
    //------------------------------------------------------------------------------------------------------------
    

    //------------------------------------------------------------------------------------------------------------
    //switcht tussen een radiobutton of checkbox bij het veranderen van lid naar leiding &
    //maakt alle eerder gecheckte boxen of radiobuttons leeg
    $('#type').change(function () {
        
        $('#leiding').is(':checked') ? type = 'Leiding' : type = 'Lid';
        if (type == 'Lid') {
            $('#wrap_chiroleeftijd').show();
            $('#afdelingSelectie input').attr('checked', false);
            $('#afdelingSelectie label').attr('aria-pressed', false);
            $('#afdelingSelectie label').removeClass('ui-state-active');
            $('#afdelingSelectie input').attr('type', 'radio');
        } else {
            $('#wrap_chiroleeftijd').hide();
            $('#afdelingSelectie input').attr('checked', false);
            $('#afdelingSelectie label').attr('aria-pressed', false);
            $('#afdelingSelectie label').removeClass('ui-state-active');
            $('#afdelingSelectie input').attr('type', 'checkbox');
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
// functies die aangeroepen worden tijdens het bewaren van de nieuwe persoon
//------------------------------------------------------------------------------------------------------------
function vulVariabelenIn() {

    //persoonlijke gegevens
    voornaam = $('#HuidigePersoon_VoorNaam').val();
    naam = $('#HuidigePersoon_Naam').val();
    geboortedatum = $('#HuidigePersoon_GeboorteDatum').val();
    $('#vrouw').is(':checked') ? geslacht = "vrouw" : geslacht = "man";

    //adresgegevens
    postcode = $('#np_postCode').val();
    gemeente = $('#np_gemeente').val();
    straat = $('#np_straat').val();
    nummer = $('#np_nummer').val();
    bus = $('#np_bus').val();
    
    //communicatie
    telefoonnummer = $('#np_telefoonnummer').val();
    emailadres = $('#np_emailadres').val();

    //chirogegevens
    $('#leiding').is(':checked') ? type = "Leiding" : type = "Lid";
    
}
//------------------------------------------------------------------------------------------------------------
function controle() {
    errors = "";
    fout = false;
    if(voornaam.length <= 2) {
        errors += "<li>De ingegeven voornaam is te kort</li>";
        fout = true;
    }
    if (naam.length <= 2) {
        errors += "<li>De ingegeven naam is te kort</li>";
        fout = true;
    }
    if (geboortedatum.length <= 8) {
        errors += "<li>De ingegeven geboortedatum is te kort</li>";
        fout = true;
    }
    var antwoord;
    if (emailadres.length >= 1) {
        antwoord = controleerEmail(emailadres);
        if (antwoord != "") {
            errors += "<li>" + antwoord + "</li>";
            fout = true;
        }
    }
    if (telefoonnummer.length >= 1) {
        antwoord = controleerTel(telefoonnummer);
        if (antwoord != "") {
            errors += "<li>" + antwoord + "</li>";
            fout = true;
        }
    }
    return fout;
}

//------------------------------------------------------------------------------------------------------------

