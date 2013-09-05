// Code om datetimes in stringvorm deftig te sorteren. Belangrijk: gaan uit van formaat dd/mm/yyyy!
function trim(str) {
    str = str.replace(/^\s+/, '');
    for (var i = str.length - 1; i >= 0; i--) {
        if (/\S/.test(str.charAt(i))) {
            str = str.substring(0, i + 1);
            break;
        }
    }
    return str;
}

function dateHeight(dateStr) {
    var trimmed = trim(dateStr);
    if (trimmed != '') {
        var frDate = trimmed.split('/');
        var day = parseInt(frDate[0]);
        var month = parseInt(frDate[1]) * 31;
        var year = parseInt(frDate[2]) * 366;
        return day + month + year;
    } else {
        return 9999999999; //GoHorse!
    }
}

jQuery.fn.dataTableExt.oSort['date-euro-asc'] = function (a, b) {
    var x = dateHeight(a);
    var y = dateHeight(b);
    return ((x < y) ? -1 : ((x > y) ? 1 : 0));
};

jQuery.fn.dataTableExt.oSort['date-euro-desc'] = function (a, b) {
    var x = dateHeight(a);
    var y = dateHeight(b);
    return ((x < y) ? 1 : ((x > y) ? -1 : 0));
};