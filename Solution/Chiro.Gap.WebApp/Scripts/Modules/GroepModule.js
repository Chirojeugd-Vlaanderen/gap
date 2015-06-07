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

// Module groep
var GroepModule = (function () {
    var initMenu = function () {
        $('#groep_Menu').menu();
        $('table tr:even').css("background-color", "lightGray");
    }
    return {
        // => groepsnaam kunnen bewerken
        // => Moet adres on the fly kunnen aanpassen
        InitVoorAlgemeen: function () {
            // UI
            initMenu();

            // EVENTS
            $('#bewerkGroepsNaam').click(function (e) {
                e.stopPropagation();
                e.preventDefault();
                $('#groepsNaam').editable('toggle');
            });
            $('#groepsNaam').editable({
                validate: function (value) {
                    if ($.trim(value) == '') return 'Dit veld mag niet leeg zijn!';
                }
            }).on('save', function (e, params) {
                e.preventDefault();
                var idVanDeGroep;
                var plaatsVanDeGroep;
                var stamNummerVanDeGroep;

                var url = link("Groep", "NaamWijzigen");
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
        },

        // => nieuwe afdeling
        // => afdelingsverderling maken
        // => afdeling activeren
        // => afdeling verwijderen
        // => een afdelingjaar verwijderen
        InitVoorAfdelingen: function () {
            // UI
            initMenu();
            $('#groep_afdelingen_aanpassen_knop').button({
                icons:
                {
                    primary: "ui-icon-wrench"
                }
            });
            // Events
            $('#groep_afdelingen_nieuw').click(function () {
                clearDialog();
                $('#errors').hide();
                var url = link("Afdelingen", "Nieuw") + ' #main';
                $('#extraInfoDialog').load(url, function () {
                    success: {
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
            });

            //      Afdelingsverdeling aanpassen
            $('.groep_bewerkAfdeling').click(function () {
                clearDialog();
                $('#errors').hide();
                $('#extraInfoDialog').dialog();
                var afdelingId = $(this).parent().parent().find('td input').val();
                var url = link("Afdelingen", "AfdJaarBewerken") + "/" + afdelingId + " #main";
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
            });

            // een afdeling activeren
            $('.afdActiveren').click(function () {
                clearDialog();
                $('#errors').hide();
                var afdelingId = $(this).parent().parent().find('td input').val();
                var url = link("Afdelingen", "Activeren") + "/" + afdelingId + " #main";
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
                var afdelingId = $(this).parent().parent().find('td input').val();
                var url = link("Afdelingen", "Verwijderen") + '/' + afdelingId;
                $('#extraInfoDialog').html('Ben je zeker dat je deze afdeling wil verwijderen?');
                $('#extraInfoDialog').dialog({
                    modal: true,
                    title: "Afdeling verwijderen",
                    buttons: {
                        'Ja': function () {
                            $(this).dialog('close');
                            $.getJSON(url, {}, function (data) {
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
                clearDialog();
                $('#errors').hide();
                var afdelingId = $(this).parent().parent().find('td input').val();
                var url = link("Afdelingen", "VerwijderenVanWerkjaar") + '/' + afdelingId;;
                $('#extraInfoDialog').html('Ben je zeker dat je deze afdeling wil verwijderen voor dit jaar (kan enkel als er geen leden in zitten)?');
                $('#extraInfoDialog').dialog({
                    modal: true,
                    title: "Afdeling verwijderen uit werkjaar",
                    buttons: {
                        'Ja': function () {
                            $(this).dialog('close');
                            $.getJSON(url, {}, function (data) {
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
        },

        // => Categorie toevoegen
        // => Categorie verwijderen
        InitVoorCategorieen: function () {
            initMenu();
            // UI
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
            // events
            $('#groep_categorieën_Toevoegen').click(function () {
                clearDialog();
                var url = link("Categorieen", "Index") + ' #main';
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
            });

            $('.categorieVerwijderen').click(function () {
                clearDialog();
                var categorieId = $(this).parent().parent().find('td input').val();
                var url = link("Categorieen", "CategorieVerwijderen") + "/" + categorieId;
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

        },

        // => Functies toevoegen
        // => Functies bewerken
        // => Functies verwijderen
        InitVoorFuncties : function(){
            initMenu();

            // toevoegen
            $('#groep_functies_toev_verw').click(function () {
                clearDialog();
                var url = link("Functies", "Index") + " #main";
                $('#extraInfoDialog').load(url, function () {
                    success: {
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
            });

            //  bewerken
            $('.functieBewerken').click(function () {
                clearDialog();
                var functieId = $(this).parent().parent().find('td input').val();
                var url = link("Functies", "Bewerken") + "/" + functieId + " #main";
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

            // Verwijderen
            $('.functieVerwijderen').click(function () {
                $('#extraInfoDialog').html('Ben je zeker dat je deze functie wil verwijderen?');
                var functieId = $(this).parent().parent().find('td input').val();
                var url = link("Functies", "Verwijderen");
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
        }
    }
}());

