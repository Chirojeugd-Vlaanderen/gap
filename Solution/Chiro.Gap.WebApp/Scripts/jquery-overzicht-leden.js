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
    
    $('#filter').hide();
    $('#kiesActie').hide();
    $("#AfdelingID").change(function () {
        $('#filter').click();
    });
    $("#FunctieID").change(function () {
        $('#filter').click();
    });
    $("#SpecialeLijst").change(function () {
            $('#filter').click();
    });
    $("#GekozenActie").change(function () {
        $('#kiesActie').click();
    });


    //check of uncheck alle checkboxen in de lijst
    $("#checkall").change(function () {
        var bool = $("#checkall").is(':checked');
        $('input[type="checkbox"]').prop('checked', bool);
    });
    
    //hier maak ik de juiste url naar de .swf file om de pdf te maken
    var protocol = checkProtocol();
    var url =protocol + "//" + document.location.host + root;
    url += "Content/media/copy_csv_xls_pdf.swf";
    
    
    
    //instellingen voor datatable
    $('#ledenOverzichtsTabel').dataTable({
        "bJQueryUI": true,
        "sPaginationType": "full_numbers",
        "iDisplayLength": 25,
        "aaSorting": [[2, "asc"]],
        "aLengthMenu": [[25, 50, 100, 150, -1], [25, 50, 100, 150, "Alle"]],
        "aoColumnDefs": [
             { "bSortable": false, "aTargets": [ 0 ] },
             { "sWidth": "15px", "aTargets": [ 0,1,4,5,6,7 ] },
             { "sWidth": "150px", "aTargets": [ 2 ] },
             { "sWidth": "50px", "aTargets": [ 3 ] },
             { "bSearchable": false, "aTargets": [0,1,4,5,6,7,8,10,11]},
            { "sType": "html", "aTargets": [2, 6, 7, 10, 11] },
            { "bVisible": false, "aTargets": [4] }
        ],
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
            "sSearch": "<strong>Zoeken in tabel:</strong>",
            "oPaginate": {
                "sFirst": "Eerste",
                "sLast": "Laatste",
                "sNext": "Volgende",
                "sPrevious": "Vorige"
            }
        },
        "sDom": '<"H"Tlfr>t<"F"ip>',
        "oTableTools": {
            "sSwfPath": url ,
            "aButtons": [
                    {
                        "sExtends": "pdf",
                        "sFileName": '*.pdf',
                        "mColumns": [ 1,2,3,6,9,10 ],
                        "sButtonText": "<strong>Opslaan als pdf</strong>",
                        "sPdfOrientation": "landscape",
                        "sPdfMessage": "Geïmporteerd uit het GroepsAdministratieProgramma (GAP).",
                        "sPdfSize": "A4",
                    },
				    {
				        "sExtends": "xls",
				        "sButtonText": "<strong>Opslaan als Excel</strong>",
				        "mColumns": [ 1,2,3,4,5,6,7,9,10,11 ]
				    }
            ]
        }
    });
    
    // UGLY: Als verjaardagslijst gekozen, sorteer dan op verjaardag.
    if (getParameterByName("sortering") == "Verjaardag") {
        $('#ledenOverzichtsTabel').dataTable().fnSort([[4, "asc"]]);
    }
    
    //css eigenschappen toevoegen aan/verwijderen voor deze tabel
    $('#ledenOverzichtsTabel_wrapper').find('.DTTT_container').attr('id', 'knoppenBalk');
    $('#ledenOverzichtsTabel_wrapper').css('float', 'none');
    $('#knoppenBalk').css({
        'margin-right': '0px',
        'max-width': '400px', 
        'float': 'left',
    });
    $('#ledenOverzichtsTabel_paginate, #ledenOverzichtsTabel_paginate span a').removeClass('.ui-button, ui-buttonset');
    $('#acties').css({ 'float': 'none', 'display': 'inline'});
    $('#ToolTables_ledenOverzichtsTabel_0, #ToolTables_ledenOverzichtsTabel_1').css({
        'border-radius': '5px',
        'padding': '5px', 
        'margin-right' : '3px'
    });
    $('.fg-toolbar').css('padding-bottom', '0px');
});