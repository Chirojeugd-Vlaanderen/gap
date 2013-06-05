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
        url += ' #form0';
        $('#DialogDiv').load(url, function () {
            success:
            {
                gedeeltelijkTonen('#form0');
                $('input[type="submit"]').hide();
                $(this).dialog({
                    modal: true,
                    width: 400,
                    title: "Categorie toevoegen",
                    buttons: {
                        'Bewaren': function () {
                            $('#form0').submit();
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
        categorieId = $(this).parent().parent().find('td input').val();
        alert(categorieId);
        url = link("Categorieen", "CategorieVerwijderen");
        url += "/" + categorieId;
        $.post(url).done(function () {
            location.reload();
        });
    });

    //      knop functies toevoegen
    $('#groep_functies_toev_verw').click(function () {
        url = link("Functies", "Index");
        url += " #toevoegen";
        $('#DialogDiv').load(url, function () {
            success:
            {
                gedeeltelijkTonen('#toevoegen');
                $('#DialogDiv').dialog({
                    modal: true,
                    width: 400,
                    title: 'Functie toevoegen', //Macheert nog nie, bekijken!
                    buttons: {
                        'Bevestigen': function () {
                            $('#toevoegen').submit();
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
    //      een functie bewerken
    $('.functieBewerken').click(function () {
        functieId = $(this).parent().parent().find('td input').val();
        url = link("Functies", "Bewerken");
        url += "/" + functieId + " #bewerken";
        $('#DialogDiv').load(url, function () {
            $(this).dialog();
            success:
            {
                gedeeltelijkTonen("#bewerken");
                $('#bewaarFunctie').hide();
                $('#DialogDiv').dialog({
                    modal: true,
                    title: "Functie bewerken",
                    buttons: {
                        'Bevestigen': function () {
                            $('#bewaarFunctie').click();
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
    });

    //      een functie VERWIJDEREN
    $('.functieVerwijderen').click(function () {
        functieId = $(this).parent().parent().find('td input').val();
        alert(functieId);
        url = link("Functies", "Verwijderen");
        $.post(url, { FunctieID: functieId }).done(function () {
            location.reload();
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