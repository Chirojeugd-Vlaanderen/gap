// jQuery code voor het overzicht van alle personen die ooit bij deze groep in het GAP ingeschreven waren.
// tabblad: 'Iedereen'

//DocumentReady function
$(function () {
    //datatable maken van de tabel en algemene instellingen
    $('#overzichtsTabel').dataTable({
        "bJQueryUI": true,
        "sPaginationType": "full_numbers",
        "iDisplayLength": 25,
        "aLengthMenu": [[20, 50, 100, 150, -1], [20, 50, 100, 150, "Iedereen"]],
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
        "aoColumns": [
                  { "bSearchable": false },
                  null,
                  null,
                  null,
                  { "bSearchable": false },
                  null,
                  null,
                  { "bSearchable": false }
             ]
    });

    $('#overzichtsTabel_paginate, #overzichtsTabel_paginate span a').removeClass('.ui-button, ui-buttonset');
    $('#acties').css({ 'float': 'none', 'display': 'inline' });


    //check of uncheck alle checkboxen in de lijst
    $("#checkall").change(function () {
        var bool = $("#checkall").is(':checked');
        $('.sorting_1 input[type="checkbox"]').prop('checked', bool);
    });
  
    
});     //Einde DocumentReady