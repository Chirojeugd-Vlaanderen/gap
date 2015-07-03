/*
 * Copyright 2013, Arno Soontjens
 * Cleanup en refactoring met module pattern: Copyright 2015 Sam Segers
   _____                         _          
  / ____|                       (_)         
 | (___   __ _ _ __ _____      ___ ___  ___ 
  \___ \ / _` | '_ ` _ \ \ /\ / / / __|/ _ \
  ____) | (_| | | | | | \ V  V /| \__ \  __/
 |_____/ \__,_|_| |_| |_|\_/\_/ |_|___/\___|
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

var PersoonModule = function () {
    // Verbergt of toont lidinfo alnaargelang welk lidtype geselecteerd is.
    var toonVerbergLidInfo = function(selectedValue) {
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
    return {
        InitVoorNieuw: function() {
            // Activeer adres-autocomplete
            AdresModule.Init();

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
            preventSubmitOnEnter();
            // changeListener: Persoon inschrijven of niet?
            toonVerbergLidInfo($(":radio[name='InschrijvenAls'][checked='checked']").val());      
            $('#chiroGegevens :radio').change(function () {
                toonVerbergLidInfo($(this).val());
            });

        }
    }
}();
