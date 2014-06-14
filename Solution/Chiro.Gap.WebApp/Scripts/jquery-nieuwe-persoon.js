/*
 * Copyright 2013, Arno Soontjens
 * Cleanup: copyright 2013, Chirojeugd-Vlaanderen vzw
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

var inschrijven=false;
var type = "Leiding";

//--------------------------------------------------------------------------------
//Document ready functie
//--------------------------------------------------------------------------------
$(function () {

    // Activeer adres-autocomplete
    AdresBewerken();

    // initieel zijn de rijen voor afdeling en Chiroleeftijd nog onzichtbaar
    $('#rij_afdeling').hide();
    $('#rij_chiroleeftijd').hide();

    $('#NieuwePersoon_GeboorteDatum').datepicker({
        showAnim: "slide",
        changeYear: true,
        changeMonth: true,
        maxDate: "-0y",
        defaultDate: "-6y", // ik doe maar iets
        yearRange: "-80:+0"
    });

    $.datepicker.setDefaults($.datepicker.regional['be']);

    //Disable per ongeluk te vroeg submitten door 'enter' te drukken
    $(this).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });

    //------------------------------------------------------------------------------------------------------------
    //Persoon inschrijven of niet?

    // We doen dit initieel
    ToonVerbergLidInfo($(":radio[name='InschrijvenAls'][checked='checked']").val());

    // .. en iedere keer er een radio dink geklikt wordt.
    // (wat niet helemaal juist is, want dit gebeurt ook bij wijzigen van geslacht.
    // Niet mooi, maar kan ook geen kwaad.)
    $(':radio').change(function () {
        ToonVerbergLidInfo($(this).val());
    });
});

// Verbergt of toont lidinfo alnaargelang welk lidtype geselecteerd is.
function ToonVerbergLidInfo(selectedValue) {
    if (selectedValue === "Geen") {
        // niet inschrijven
        $('#rij_afdeling').hide();
        $('#rij_chiroleeftijd').hide();
    } else if (selectedValue === "Kind") {
        $('#rij_afdeling').show();
        $('#rij_chiroleeftijd').show();
        $('#AfdelingsJaarIDs').find('option[value="0"]').remove();
    } else if (selectedValue === "Leiding") {
        $('#rij_afdeling').show();
        $('#rij_chiroleeftijd').show();
        if (!$('#AfdelingsJaarIDs').find('option[value="0"]').length) {
            // als de optie 'geen afdeling' nog niet bestaat, voegen we ze toe

            $('#AfdelingsJaarIDs').append('<option value="0">geen</option>');
        }
        $('#AfdelingsJaarIDs').find('option[value="0"]').attr('selected', true);
    }    
}
