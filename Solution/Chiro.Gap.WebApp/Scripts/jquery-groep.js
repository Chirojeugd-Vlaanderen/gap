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

$(function () {
    //Variabelen
    var functieId;
    var categorieId;
    var afdelingId;
    var doorgaan = false;
    var url;
    //tabel opmaak
    $('table tr:even').css("background-color", "lightGray");

    //knoppen
    $('#groep_functies_toev_verw').button({
        icons: {
            primary: "ui-icon-circle-plus"
        }
    });

    $('#groep_categorieën_Toevoegen').button({
        icons: {
            primary: "ui-icon-circle-plus"
        }
    });

    $('#groep_afdelingen_aanpassen_knop').button({
        icons:
        {
            primary: "ui-icon-wrench"
        }
    });
    //---------------------------------------------------------------------------

    //inline editing
    //---------------------------------------------------------------------------
    //  click events
    //      pen icoon: groepsnaam bewerken
    $('#bewerkGroepsNaam').click(function (e) {
        e.stopPropagation();
        e.preventDefault();
        $('#groepsNaam').editable('toggle');
    });
    //      Categorie toevoegen
    $('#groep_categorieën_Toevoegen').click(function () {
        url = link("Categorieen", "Index");
        url += ' #main';
        $('#extraInfoDialog').load(url, function () {
            success:
            {
                gedeeltelijkTonen('#extraInfoDialog');
                $('#categorieen_bestaand').hide();
                $('input[type="submit"]').hide();
                $(this).dialog({
                    modal: true,
                    width: 450,
                    height: 280,
                    title: "Categorie toevoegen",
                    buttons: {
                        'Bewaren': function () {
                            $('#categorieToevoegen').click();
                            $(this).dialog('close');
                        },
                        'Annuleren': function () {
                            $(this).dialog('close');
                        }
                    }
                });
            }
        });
        clearDialog();
    });

    $('.categorieVerwijderen').click(function () {
        categorieId = $(this).parent().parent().find('td input').val();
        url = link("Categorieen", "CategorieVerwijderen");
        url += "/" + categorieId;
        $('#extraInfoDialog').html('Ben je zeker dat je deze categorie wil verwijderen?');
        $('#extraInfoDialog').dialog({
            modal: true,
            title: "Categorie verwijderen",
            buttons: {
                'Ja': function () {
                    doorgaan = true;
                    $(this).dialog('close');
                    $.post(url, { CategorieID: categorieId }).done(function () {
                        doorgaan = false;
                        location.reload();
                    });
                },
                'Nee': function () {
                    $(this).dialog('close');
                }
            }
        });
    });

    $('#groep_afdelingen_nieuw').click(function () {
        $('#errors').hide();
        url = link("Afdelingen", "Nieuw");
        url += ' #main';
        $('#extraInfoDialog').load(url, function () {
            success:
                {
                    gedeeltelijkTonen('#extraInfoDialog');
                    $(this).dialog({
                        modal: true,
                        width: 450,
                        height: 300,
                        title: "Een nieuwe afdeling maken",
                        buttons: {
                            'Bewaren': function () {
                                $('#afdelingNieuw_bewaar').click();
                                $(this).dialog('close');
                            },
                            'Annuleren': function () {
                                $(this).dialog('close');
                            }
                        }
                    });
                }
        });
        clearDialog();
    });

    //      Afdelingsverdeling aanpassen
    $('.groep_bewerkAfdeling').click(function () {
        $('#errors').hide();
        $('#extraInfoDialog').dialog();
        afdelingId = $(this).parent().parent().find('td input').val();
        url = link("Afdelingen", "AfdJaarBewerken");
        url += "/" + afdelingId + " #main";
        $('#extraInfoDialog').load(url, function () {
            success:
            {
                gedeeltelijkTonen('#extraInfoDialog');
                $(this).dialog({
                    modal: true,
                    width: 450,
                    height: 400,
                    title: "Een afdeling bewerken",
                    buttons: {
                        'Bewaren': function () {
                            $('#afdelingBewerken_bewaar').click();
                        },
                        'Annuleren': function () {
                            $(this).dialog('close');
                        }
                    }
                });
            }
        });
        clearDialog();
    });

    // een afdeling activeren
    $('.afdActiveren').click(function () {
        $('#errors').hide();
        afdelingId = $(this).parent().parent().find('td input').val();
        url = link("Afdelingen", "Activeren");
        url += "/" + afdelingId + " #main";
        $('#extraInfoDialog').load(url, function () {
            success:
                {
                    gedeeltelijkTonen("#extraInfoDialog");
                    $('#afdelingBewerken_bewaar').hide();
                    $('#extraInfoDialog').dialog({
                        modal: true,
                        width: 450,
                        height: 400,
                        title: "Afdeling Activeren",
                        buttons: {
                            'Bevestigen': function () {
                                $('#afdelingBewerken_bewaar').click();
                                $(this).dialog('close');
                            },
                            'Annuleren': function () {
                                $(this).dialog('close');
                            }
                        }
                    });
                }
        });
    });

    // een afdeling VERWIJDEREN
    $('.afdelingVerwijderen').click(function () {
        $('#errors').hide();
        afdelingId = $(this).parent().parent().find('td input').val();
        url = link("Afdelingen", "Verwijderen") + '/' + afdelingId;
        $('#extraInfoDialog').html('Ben je zeker dat je deze afdeling wil verwijderen?');
        $('#extraInfoDialog').dialog({
            modal: true,
            title: "Afdeling verwijderen",
            buttons: {
                'Ja': function () {
                    $(this).dialog('close');
                    $.getJSON(url, { }, function (data) {
                        if (data[0] == "gelukt") {
                            location.reload();
                        } else {
                            $('#errors').html('Afdeling kon niet verwijderd worden want ze is actief in sommige jaren');
                            $('#errors').show();
                        }
                    });
                },
                'Nee': function () {
                    $(this).dialog('close');
                }
            }
        });
    });

    // een afdelingjaar VERWIJDEREN
    $('.afdelingjaarVerwijderen').click(function () {
        $('#errors').hide();
        afdelingId = $(this).parent().parent().find('td input').val();
        url = link("Afdelingen", "VerwijderenVanWerkjaar") + '/' + afdelingId;;
        $('#extraInfoDialog').html('Ben je zeker dat je deze afdeling wil verwijderen voor dit jaar (kan enkel als er geen leden in zitten)?');
        $('#extraInfoDialog').dialog({
            modal: true,
            title: "Afdeling verwijderen uit werkjaar",
            buttons: {
                'Ja': function () {
                    $(this).dialog('close');
                    $.getJSON(url, { }, function (data) {
                        if (data[0] == "gelukt") {
                            location.reload();
                        } else {
                            $('#errors').html('Afdeling kon niet verwijderd worden want ze heeft nog leden');
                            $('#errors').show();
                        }
                    });
                },
                'Nee': function () {
                    $(this).dialog('close');
                }
            }
        });
    });

    //      knop functies toevoegen
    $('#groep_functies_toev_verw').click(function () {
        url = link("Functies", "Index");
        url += " #main";
        $('#extraInfoDialog').dialog({
            modal: true,
            width: 420,
            heigth: 450
        });
        $('#extraInfoDialog').load(url, function () {
            success:
            {
                gedeeltelijkTonen('#extraInfoDialog');
                $('#bewerken').hide();
                $('#functies_bestaand').hide();
                $('#extraInfoDialog').dialog({
                    modal: true,
                    width: 450,
                    height: 550,
                    title: 'Functie toevoegen', //Macheert nog nie, bekijken!
                    buttons: {
                        'Bevestigen': function () {
                            $('#functieToevoegen').click();
                            $(this).dialog('close');
                        },
                        'Annuleren': function () {
                            $(this).dialog('close');
                        }
                    }
                });
            }
        });
        clearDialog();
    });
    //      een functie bewerken
    $('.functieBewerken').click(function () {

        // die clearDialog stond oorspronkelijk achter de 'load'. Maar
        // alnaargelang hoe lang de load duurt, wordt die meestal toch
        // uitgevoerd voor de load afgelopen is, en de inhoud wordt
        // getoond. Het zou beter en alleszins duidelijker zijn, moest
        // die clearDialog overal naar voren verhuisd worden.
        clearDialog();

        functieId = $(this).parent().parent().find('td input').val();
        url = link("Functies", "Bewerken");
        url += "/" + functieId + " #main";
        $('#extraInfoDialog').load(url, function () {
            $(this).dialog();
            success:
            {
                gedeeltelijkTonen("#extraInfoDialog");
                $('#bewaarFunctie').hide();
                $('#extraInfoDialog').dialog({
                    modal: true,
                    width: 550,
                    height: 350,
                    title: "Functie bewerken",
                    buttons: {
                        'Bevestigen': function () {
                            $('#bewaarFunctie').click();
                            $(this).dialog('close');
                        },
                        'Annuleren': function () {
                            $(this).dialog('close');
                        }
                    }
                });
            }
        });
    });

    //      een functie VERWIJDEREN
    $('.functieVerwijderen').click(function () {
        functieId = $(this).parent().parent().find('td input').val();
        url = link("Functies", "Verwijderen");
        $('#extraInfoDialog').html('Ben je zeker dat je deze functie wil verwijderen?');
        $('#extraInfoDialog').dialog({
            modal: true,
            title: "Functie verwijderen",
            buttons: {
                'Ja': function () {
                    $(this).dialog('close');
                    $.post(url, { FunctieID: functieId }).done(function () {
                        location.reload();
                    });
                },
                'Nee': function () {
                    $(this).dialog('close');
                }
            }
        });
    });

    //---------------------------------------------------------------------------
    //  editable events
    //      veranderen GROEPSNAAM
    $('#groepsNaam').editable({
        validate: function (value) {
            if ($.trim(value) == '') return 'Dit veld mag niet leeg zijn!';
        }
    }).on('save', function (e, params) {
        e.preventDefault();
        var idVanDeGroep;
        var plaatsVanDeGroep;
        var stamNummerVanDeGroep;

        url = link("Groep", "NaamWijzigen");
        $.get(url, {}, function (data) {
            idVanDeGroep = data.Info.ID;
            plaatsVanDeGroep = data.Info.Plaats;
            stamNummerVanDeGroep = data.Info.StamNummer;
        }).done(function () {
            var nieuweNaam = params.newValue;
            $.post(url, {
                "Info.Naam": nieuweNaam,
                "Info.ID": idVanDeGroep,
                "Info.Plaats": plaatsVanDeGroep,
                "Info.StamNummer": stamNummerVanDeGroep
            }, function () {
                success: location.reload();
            });
            $('#groepsNaam').text(params.newValue);
        });
    });
});