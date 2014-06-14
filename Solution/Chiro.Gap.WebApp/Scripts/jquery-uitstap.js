/*
 * Copyright 2014, Chirojeugd-Vlaanderen vzw
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

//--------------------------------------------------------------------------------
//Document ready functie
//--------------------------------------------------------------------------------
$(function () {
    var vandaag = new Date();

    $('#Uitstap_DatumTot').datepicker({
        showAnim: "slide",
        changeYear: true,
        changeMonth: true,
        minDate: "-0",
        defaultDate: vandaag,
        yearRange: "-0:+5",
        numberOfMonths: 2
    });

    $('#Uitstap_DatumVan').datepicker({
        showAnim: "slide",
        changeYear: true,
        changeMonth: true,
        minDate: "-0",
        defaultDate: vandaag,
        yearRange: "-0:+5",
        onSelect: function () {
            var begindatum = $('#Uitstap_DatumVan').datepicker('getDate');

            if (begindatum) {
                EinddatumboxAanpassen();
            }
        }
    });

    $.datepicker.setDefaults($.datepicker.regional['be']);

    //Disable per ongeluk te vroeg submitten door 'enter' te drukken
    $(this).keydown(function (event) {
        if (event.keyCode == 13) {
            event.preventDefault();
            return false;
        }
    });
});

function EinddatumboxAanpassen() {
    var begindatum = $('#Uitstap_DatumVan').datepicker('getDate');

    $('#Uitstap_DatumTot').datepicker('option', 'minDate', begindatum);
    $('#Uitstap_DatumTot').datepicker('option', 'defaultDate', begindatum);
}