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

//DocumentReady function
$(function () {
    //instellingen voor datatable
    $('#overzichtsTabel').dataTable({
        "bJQueryUI": true,
        "sPaginationType": "full_numbers",
        "iDisplayLength": 25,
        "aaSorting": [[2,"asc"]],
        "aoColumnDefs": [
             { "bSortable": false, "aTargets": [ 0 ] }
        ],
        "aLengthMenu": [[25, 50, 100, 150, -1], [25, 50, 100, 150, "Alle"]],
        "oLanguage": {
            "sLengthMenu": "Toon _MENU_ personen per pagina",
            "sZeroRecords": "Er werden geen overeenkomsten gevonden",
            "sInfo": "Toont _START_ tot _END_ van _TOTAL_ personen",
            "sInfoEmpty": "Toont 0 tot 0 van 0 personen",
            "sInfoFiltered": "(Gefilterd uit _MAX_ personen in totaal)",
            "sEmptyTable": "Geen gegevens gevonden",
            "sInfoPostFix": "",
            "sInfoThousands": ".",
            "sLoadingRecords": "De gegevens worden geladen...",
            "sProcessing": "De gegevens worden verwerkt...",
            "sSearch": "Filter gegevens:",
            "oPaginate": {
                "sFirst": "Eerste",
                "sLast": "Laatste",
                "sNext": "Volgende",
                "sPrevious": "Vorige"
            }
        },
    });

    $('#overzichtsTabel_paginate, #overzichtsTabel_paginate span a').removeClass('.ui-button, ui-buttonset');
    $('#acties').css({ 'float': 'none', 'display': 'inline' });


    //check of uncheck alle checkboxen in de lijst
    $("#checkall").change(function () {
        var bool = $("#checkall").is(':checked');
        $('input[type="checkbox"]').prop('checked', bool);
    });
  
//-----------------------------------------------------------------------------------
});     //Einde DocumentReady