/*
Frontend javascript (JQuery)
GAP, Chirojeugd Vlaanderen
2013
*/
//variabelen voor het maken van een nieuw lid
var volledigeNaam, voornaam, naam, geboortedatum, geslacht;
var postcode, gemeente, straat, nummer, bus;
var land = 'België';
var persoonID, lidID, werkjaar,leiding;

var telefoonnummer;
var emailadres;

var straten = [];
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

var zoekterm;
var errors;
var url;
var fout = false;
//--------------------------------------------------------------------------------
//Document ready functie
//--------------------------------------------------------------------------------
$(function () {

   
    //werkjaar invullen op groepen op te halen
    werkjaar = $('#np_werkJaarID').val();

    //url = root + "Adressen/LandenVoorstellen";
    url = link("Adressen", "LandenVoorstellen");
    $.getJSON(url, function(data) {
        $.each(data, function(index, value) {
            $('#landSelect').append('<option id="' + data[index].ID +'" value="' +data[index].Naam +'">' + data[index].Naam +'</option>');
        });
    });
    $('#landSelect').selectedIndex = 2;
    //------------------------------------------------------------------------
    // afdelingsinfo binnenhalen
    //actieve afdelingen met hun (speciale) namen ophalen
    //url = root + "Afdelingen/AfdelingsInfo";
    url = link("Afdelingen", "AfdelingsInfo");
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
        maxDate: "-6y"

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
        chiroleeftijd = $(this).val();
    });
    //------------------------------------------------------------------------------------------------------------
    $('#landSelect').change(function() {
        land = $(this).val();
        if (land != 'België') {
            $('#np_gemeente').hide();
            $('#buitenlandseGemeente, #postNr').show();
        } else {
            $('#np_gemeente').show();
            $('#buitenlandseGemeente, #postNr').hide();
            postcode = $('#np_postCode').val();
            toonGemeenten(postcode, '#np_gemeente');
        }
    });
    //------------------------------------------------------------------------------------------------------------
    //
    $('#np_postCode').keyup(function () {
        postcode = $(this).val();
        if (land == 'België') {
            toonGemeenten(postcode, '#np_gemeente');
        }
    });
  
    //------------------------------------------------------------------------
    //url = root +"Adressen/StratenVoorstellen";
    url = link("Adressen", "StratenVoorstellen");
    $('#np_straat').keyup(function() {
        var straten = [];
        zoekterm = $(this).val();
        if (zoekterm.length > 2) {
            $.getJSON(url, { q: zoekterm, postNummer: postcode }, function(data) {
                $.each(data, function(index, value) {
                    var waarde = data[index].toString();
                    straten.push(waarde);
                });
                $('#np_straat').autocomplete({
                        source: straten
                });
            });
        }
    });
    
        /* $('#np_straat').autocomplete({
         source:function(request, response) {
            //AJAX
            $.ajax({
               url: url,
               dataType: 'jsonp',
               data: {
                   postNummer: postcode,
                   featureClass: "P",
                   style: "full",
                   maxRows: 12,
                   q: request.term
               },
               success: function(data) {
                   alert(JSON.stringify(data));
                   response($.map(data, function(item) {
                       alert(item);
                        return {
                            label: item,
                            value: item
                        }
                   }));
               }
            });
            //AJAX EINDE
        },
        select: function(event, ui) {
            alert(ui.item.label);
        },
        open: function () {
            $( this ).removeClass( "ui-corner-all" ).addClass( "ui-corner-top" );
        },
        close: function() {
            $( this ).removeClass( "ui-corner-top" ).addClass( "ui-corner-all" );
        },
        minLength: 2
        
    }); */
    
      /*  if (land == 'België') {
            var term = $(this).val();
            postcode = $('#np_postCode').val();
            
            $.getJSON(url, {q: term, PostNummer: postcode}, function(data) {
                if (data == '') {
                    straten = 'Geen resulaten gevonden';
                } else {
                     $.each(data, function(index, value) {
                        straten.push(data[index]);
                    });
                }
            });
            
        }*/
    
    
    
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
            var voortgang = $('#balk');
            var progressLabel = $('.progress-label');
            voortgang.progressbar({
                change: function() {
                    progressLabel.text( voortgang.progressbar( "value" ) + "%" );
                },
                complete: function() {
                    progressLabel.text( "Klaar!" );
                }
            });
            var val = voortgang.progressbar('value') || 0;
            
            $('#progress').dialog({
                modal: true,
                height: 100,
                title: 'Verwerking bezig',
                dialogClass: 'noclose'
            });
            
            //url = root + "Personen/Nieuw";
            url = link("Personen", "Nieuw");
            $.post(url, {
                    "HuidigePersoon.VoorNaam": voornaam,
                    "HuidigePersoon.Naam": naam,
                    "HuidigePersoon.GeboorteDatum": geboortedatum,
                    "HuidigePersoon.Geslacht": geslacht
                }, function(data) {
                    success:
                    {
                        voortgang.progressbar('value', val += 10);
                        //als 'data[0]' undefined is zit het niet in het JSON antwoord en is er dus geen dezelfde persoon
                        if (typeof(data[0]) === "undefined") {
                            np_gpID = data.GelieerdePersoonID; 
                            url = link("Personen", "NieuweCommVorm");
                            //----------------------------------------------------------------------------------------
                            if (tel) {
                                $.post(url, {
                                    gelieerdePersoonID: np_gpID,
                                    "NieuweCommVorm.CommunicatieTypeID": 1,
                                    "NieuweCommVorm.Nummer": telefoonnummer,
                                    "NieuweCommVorm.Voorkeur": true
                                });
                                voortgang.progressbar('value', val += 10);
                            }
                             //----------------------------------------------------------------------------------------
                            if (email) {
                                $.post(url, {
                                    gelieerdePersoonID: np_gpID,
                                    "NieuweCommVorm.CommunicatieTypeID": 3,
                                    "NieuweCommVorm.Nummer": emailadres ,
                                    "NieuweCommVorm.Voorkeur": true
                                });
                                voortgang.progressbar('value', val += 10);
                            }
                            //----------------------------------------------------------------------------------------
                            //Persoon inschrijven
                            if (inschrijven) {
                                    //url = root +"Leden/LedenMaken";
                                    url = link("Leden", "LedenMaken");
                                    $.ajax({
                                        url: url,
                                        traditional: true,
                                        type: 'POST',
                                        data: {
                                            "PersoonEnLidInfos[0].GelieerdePersoonID": np_gpID,
                                            "PersoonEnLidInfos[0].InTeSchrijven": true,
                                            "PersoonEnLidInfos[0].LeidingMaken": leiding,
                                            "PersoonEnLidInfos[0].VolledigeNaam": volledigeNaam,
                                            "PersoonEnLidInfos[0].AfdelingsJaarIDs": afdelingsJaarIDs[0],
                                            "BeschikbareAfdelingen": beschikbareAfdelingen,
                                            checkall: false,
                                        }
                                    }).done(function() {
                                        voortgang.progressbar('value', val += 20);
                                        //Alle gegevens van de net ingeschreven persoon ophalen
                                        //url = root +"Personen/PersoonsGegevensOphalenJson";
                                            url = link("Personen", "PersoonsGegevensOphalenJson");
                                        $.getJSON(url, { gelieerdePersoonId: np_gpID }, function(res) {
                                            persoonID = res.HuidigePersoon.PersoonID;
                                            werkjaar = res.HuidigWerkJaar;
                                            lidID = res.HuidigePersoon.LidID;
                                            volledigeNaam = res.HuidigePersoon.volledigeNaam;
                                            voortgang.progressbar('value', val += 5);
                                        }).done(function() {
                                            $.each($('#afdelingSelectie input:checked'), function(index, value) {
                                                var waarde = parseInt($(this).val());
                                                geselecteerdeAfdelingen.push(waarde);
                                            });
                                            //'beschikbare afdelingen' uit het model halen
                                            //url = root +"Leden/AfdelingBewerken";
                                            url = link("Leden", "AfdelingBewerken");
                                            $.getJSON(url, { lidID: lidID }, function(antwoord) {
                                                beschikbareAfdelingen.push(antwoord.BeschikbareAfdelingen);
                                            }).done(function() {
                                                voortgang.progressbar('value', val += 20);
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
                                                    voortgang.progressbar('value', 100);
                                                    //url = root + "Personen/EditRest/" + np_gpID;
                                                    url = link("Personen", "EditRest");
                                                    url += "/" + np_gpID;
                                                    window.location = url;
                                                });
                                            });
                                        });

                                  
                                });
                            }
                        
                    

        //----------------------------------------------------------------------------------------
                        //adres toekennen
                        //url = root + "Personen/NieuwAdres/" + np_gpID;
                            url = link("Personen", "NieuwAdres");
                            url += "/" + np_gpID;
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
                        }).done(function() {
                            if (!inschrijven) {      
                                //url = root + "Personen/EditRest/" + np_gpID;
                                url = link("Personen", "EditRest");
                                url += "/" + np_gpID;
                                window.location = url;
                            }
                            voortgang.progressbar('value', val += 20);
                        });
                        //----------------------------------------------------------------------------------------
                        

                    } else { //'data[0]' is gezet, dus er is al een persoon met dezelfde naam 
                        alert("Er is al zo'n persoon");
                        $(".ui-dialog-content").dialog("close");
                            window.location = "/" + GID + "/Personen/Nieuw";
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
        
        //url = root + "Handleiding/ZusBroer #help";
        url = link("Handleiding", "ZusBroer");
        url += " #help";
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
    if ($('#leiding').is(':checked')) {
        type = "Leiding";
        leiding = true;
    } else {
        type = 'Lid';
        leiding = false;
    }
    
}
//------------------------------------------------------------------------------------------------------------
function controle() {
    var nu = new Date();
    var jaar = nu.getFullYear();
    jaar = parseInt(jaar);

    errors = "";
    fout = false;
    var geboortejaar = geboortedatum.substring(6);
    
    geboortejaar = parseInt(geboortejaar);
    var leeftijd = jaar - geboortejaar;

    if (leeftijd < 15 && leiding) {
        errors += "<li>Deze persoon is te jong om als leiding ingeschreven te worden</li>";
        fout = true;
    }
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

