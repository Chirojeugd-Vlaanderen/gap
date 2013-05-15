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
    var url ="http://" + document.location.host;
    url += "/Content/media/copy_csv_xls_pdf.swf";
    
    //instellingen voor datatable
    $('#ledenOverzichtsTabel').dataTable({
        "bJQueryUI": true,
        "sPaginationType": "full_numbers",
        "iDisplayLength": 25,
        "aaSorting": [[2, "asc"]],
        "aLengthMenu": [[25, 50, 100, 150, -1], [25, 50, 100, 150, "Alle"]],
        "aoColumnDefs": [
             { "bSortable": false, "aTargets": [ 0 ] },
             { "sWidth": "15px", "aTargets": [ 0,1,6,7,8 ] },
             { "sWidth": "150px", "aTargets": [ 3 ] },
             { "sWidth": "80px", "aTargets": [ 4 ] }
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
            "sSearch": "Filter gegevens:",
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
                        "mColumns": [ 1,2,3,4,6,7,8,10,11 ],
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
        'padding': '9px', 
        'margin-right' : '3px'
    });
    $('.fg-toolbar').css('padding-top', '20px');
});