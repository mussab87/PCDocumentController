//var lang = "";
var empty = false;
/* START: Hide/Show Loading on whole page */
function loading(action) {
    /*if (!ele) {
        ele = '';
    } else {
        $(ele).append('<div class="load_inr"><div><i class=" fa fa-refresh fa-spin"></i> <p>Loading</p></div></div>')
    };*/
    if (action == 'show') {
        $('.loading_box').show();
    } else {
        setTimeout(function () {
            $('.loading_box').hide();
        }, 600);
    };
};

function loadingFullScreen(action) {
 
    if (action == 'show') {
        $('.full_screen_loading_box').show();
    } else {
        setTimeout(function () {
            $('.full_screen_loading_box').hide();
        }, 600);
    };
};

/* END: Hide/Show Loading on whole page */

/* START: SIde Bar Script */
$(function () {
    $('.navbar-toggle').click(function () {
        $('.navbar-nav').toggleClass('slide-in');
        $('.side-body').toggleClass('body-slide-in');
        $('#search').removeClass('in').addClass('collapse').slideUp(200);

        /// uncomment code for absolute positioning tweek see top comment in css
        //$('.absolute-wrapper').toggleClass('slide-in');

    });
    // Remove menu for searching
    $('#search-trigger').click(function () {
        $('.navbar-nav').removeClass('slide-in');
        $('.side-body').removeClass('body-slide-in');

        /// uncomment code for absolute positioning tweek see top comment in css
        //$('.absolute-wrapper').removeClass('slide-in');
    });
    /* Menu Toggle Script */
$("#menu-toggle-bar").click(function (e) {
    e.preventDefault();
    $("#wrapper").toggleClass("toggled");
});
});



//$(document).on('load resize', function () {
//    if ($(window).width() <= 768) {
//        $("#wrapper").addClass("toggled");
//    } else {
//        $("#wrapper").removeClass("toggled");
//    }
//});

if ($(".sidebar-nav").length) {
    $(".sidebar-nav").slimScroll({
        height: '100%',
        width: '220px',
        alwaysVisible: true
    });
}

/** get url parameter **/
function getUrlParameter(sParam) {

    var sPageURL = decodeURIComponent(window.location.search.substring(1)),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : sParameterName[1];
        }
    }
};

function getDecryptedUrlParameter(sPageURL, sParam) {

    var sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : sParameterName[1];
        }
    }
};

/**
 * Redirect Page after parameter encryption
 * @param {String} to - The page where you want to redirect to
 * @param {String} parameters - Parameters want to decrypt
 * @param {String} _blank - Optional parameter, default _self - _blank value for open page in a new tab 
 */
function RedirectToPage(to, parameters, _blank) {
    console.log(parameters);
    var encryptedId = GetEncryptedQueryString(parameters);
    $.when(encryptedId).done(function (response) {
        var toUrl = to + "?" + response.d;
        _blank = typeof _blank !== 'undefined' ? _blank : "_self";
        window.open(toUrl, _blank);
    });
}
/**
 * Only set href after encrypting the id
 * @param {String} to - The page where you want to redirect to
 * @param {String} parameters - Parameters want to decrypt
 */
function setHref(to, parameters) {
    var encryptedId = GetEncryptedQueryString(parameters);
    $.when(encryptedId).done(function (response) {
        var toUrl = to + "?" + response.d;
        id = to.replace(/-/g, "");
        id = id.substring(0, id.indexOf('.'));
        $("#" + id + " a").attr('href', toUrl);
    });
}

function cleanString(chara) {
    return chara.replace(/[^a-zA-Z]/g, "");
};

/* Show error message under field Start */
function err(action, id, msg) {
    if (action == 'show') {
        $('#' + id).after('<label id="my_error_' + id + '" class="error" style="display: block;">' + msg + '</label>');
        if ($('#my_error_' + id).length == 0) {
            $('#' + id).after('<label id="my_error_' + id + '" class="error" style="display: block;">' + msg + '</label>');
        }
    } else {
        $('#my_error_' + id).remove();
    }
}
function autoHide(id, time) {
    setTimeout(function () {
        $('#' + id + ' + label').slideUp(500).delay(500).queue(function () {
            $(this).remove();
        });
    }, time);
};
/* Show error message under field End */


/* Format Local Date Start */
function formatToLocal(date, format) {
    if (date) { //check if date not null or empty
        date = date.replace('/Date(', '');
        date = date.replace(')', '');
        date = date.replace('/', '');

        var d = new Date();
        d.setTime(parseInt(date));

        formattedDate = moment(d).format(format);
        return formattedDate.toString();
    }
    else {
        return '';
    }
};
/* Format Local Date End */

/* Convert Local Date Start */
function ConvertToDateTime(date) {
    if (date != null || date == '') {
        date = date.replace('/Date(', '');
        date = date.replace(')', '');
        date = date.replace('/', '');

        var d = new Date();
        d.setTime(parseInt(date));

        formattedDate = moment(d);
        return formattedDate.toString();
    }
    else {
        return '';
    }
}
/* Convert Local Date Start */

function IsNULL(variable) {
    return variable == null || variable == 'null' ? "" : variable;
}

/* Global Date Picker End  */


/* tooltip str */
$('[data-toggle="tooltip"]').tooltip();
$('[data-toggle="popover"]').popover();
function popReload() {
    $('[data-toggle="popover"]').popover();
}
/* tooltip end */

/* scrolltotop str */
$('a[href^="#"].gotoTopBtn').on('click', function (event) {

    var target = $($(this).attr('href'));
    if (target.length) {
        event.preventDefault();
        $('html, body').animate({
            scrollTop: target.offset().top
        }, 1000);
    }

});
$(window).scroll(checkscroll);
function checkscroll() {
    var top = $(window).scrollTop();
    if (top < 600) {
        $('#bottom').fadeOut('slow');
    } else {
        $('#bottom').fadeIn('slow');
    }
}

checkscroll();
/* scrolltotop end */

/* START: Hide/Show Spinner On Wizard section save click */

function hideSpinner() {
    $('.no-opac').css({ 'opacity': '0' });
    $(".next").removeClass('disabled');
}
function showSpinner() {
    $('.no-opac').css({ 'opacity': '1' });
    $(".next").addClass('disabled');
}
/* END: Hide/Show Spinner On Wizard section save click */

/* Hide loading after page is loaded */
$(window).load(function () {

    if ($.active == 0) {
        loading('hide');
    } else {
        $(document).one("ajaxStop", function () {
            loading('hide');
        });
    }
});

/* Server Error */
function serverError(msg) {
    $(".next").removeClass('disabled');
    hideSpinner();
    var title = "Error";
    var txt = "Oops! Something went wrong. Please try again or contact your administrator.";
    swal({ title: title, type: "error", text: txt });
};
$('body').on('click', '.sweet-cancel', function () {
    hideSpinner();
});
function GetEncryptedQueryString(queryString) {

    console.log(queryString);
    var pageName = decodeURIComponent(window.location.pathname.substring(location.pathname.lastIndexOf("/") + 1));
    return $.ajax({
        type: "POST",
        url: pageName + "/EncryptString",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ 'plainText': queryString }),
        beforeSend: function () {
            //todo: validation or other checking before sending ajax request
        },
        success: function (data, status) {
        },
        error: function (xhr) {
            console.error('EXCEPTION: ' + xhr.statusText + ' | ' + xhr.responseText);
            serverError();
        },
        complete: function () {
            console.info('COMPLETE: ***');
            //todo: always executes doesn't matter success/error
        },
    });

};

function GetDecryptedQueryString() {

    var pageName = decodeURIComponent(window.location.pathname.substring(location.pathname.lastIndexOf("/") + 1));
    var queryString = decodeURIComponent(window.location.search.substring(1));

    return $.ajax({
        type: "POST",
        url: pageName + "/DecryptString",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSON.stringify({ 'cipherText': queryString }),
        beforeSend: function () {
            //todo: validation or other checking before sending ajax request
        },
        success: function (data, status) {
        },
        error: function (xhr) {
            console.error('EXCEPTION: ' + xhr.statusText + ' | ' + xhr.responseText);
            serverError();
        },
        complete: function () {
            console.info('COMPLETE: ***');
            //todo: always executes doesn't matter success/error
        },
    });

};


/* Exam Function */
$('.question-list li a').on('click', function (event) {
    var target = $($(this).attr('href'));
    if (target.length) {
        event.preventDefault();
        $('html, body').animate({
            scrollTop: target.offset().top
        }, 1000);
    }
});

/*$(window).scroll(function () {
      if($(window).scrollTop()>=250){
		$('.ques-nav-holder').addClass('ques-fixed');				
	  } else {
		$('.ques-nav-holder').removeClass('ques-fixed');  
		  }
}); */

function emptyChk(id, type) {
    var count = 0;
    var msg = '';
    if (type == 'multi') {
        $('#' + id + ' input[type=checkbox]').each(function () {
            if (this.checked === true) {
                count = count + 1;
            }
        });
        msg = lang == "ar" ? 'الرجاء اختيار قيد واحد على الاقل' : 'Please select atleast one record.'
    } else {
        $('#' + id + ' tr').each(function () {
            if ($(this).hasClass('selected')) {
                count = count + 1;
            } else {
                empty = false;
            };
        });
        msg = lang == "ar" ? 'الرجاء تحديد سجل واحد على الاقل' : 'Please select a record to take an action.'
    };
    if (count === 0) {
        if (lang == 'ar') {
            swal('خطأ!', msg, 'info');
        }
        else {
            swal('Warning!', msg, 'info');
        }
        empty = true;
    } else {
        empty = false;
    };
};
function disableBtn() {
    $('form input,form select, form textarea').attr('disabled', true);
    $('.next').addClass('hide');
    $('.btn-file').addClass('disabled');
};

var latitude, longitude;

function getLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(showPosition);
    }
    else {
        console.log("Geolocation is not supported by this browser.");
    }
}

function showPosition(position) {

    console.log(position.coords.latitude + ',' + position.coords.longitude);

    latitude = position.coords.latitude;
    longitude = position.coords.longitude;
}

/* Clear form if the modal is closed. */
$('.clear_form').on('click', function () {
    id = '#' + $(this).parents('form').attr('id');
    var validator = $(id).validate();
    $(id).trigger("reset");
    validator.resetForm();
});

// console temporary code
function aa(msg) {
    console.log(msg);
}


var tableToExcel = (function () {
    var uri = 'data:application/vnd.ms-excel;base64,'
      , template = '<html xmlns:o="urn:schemas-microsoft-com:office:office" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns="http://www.w3.org/TR/REC-html40"><head><!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet><x:Name>{worksheet}</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions></x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]--><meta http-equiv="content-type" content="text/plain; charset=UTF-8"/></head><body><table>{table}</table></body></html>'
      , base64 = function (s) { return window.btoa(unescape(encodeURIComponent(s))) }
      , format = function (s, c) { return s.replace(/{(\w+)}/g, function (m, p) { return c[p]; }) }
    return function (table, name) {
        if (!table.nodeType) table = document.getElementById(table)
        var ctx = { worksheet: name || 'Worksheet', table: table.innerHTML }
        var blob = new Blob([format(template, ctx)]);
        var blobURL = window.URL.createObjectURL(blob);
        if (navigator.msSaveBlob) { // IE 10+
            return blob;
        } else {
            return blobURL;
        };
    }
})();

function exportPDF(e) {
    var create_date = (lang == 'ar') ? ":تاريخ الإنشاء" : "Created on:";
    var align = (lang == 'ar') ? "right" : "left";
    var todaysDate = moment().format('DD-MM-YYYY');
    $('#divTableDataHolder').prepend('<table class="removetbl"><tr><td colspan="2"><img class="logo_xcl" src="dist/images/pscod-logo.png" height="54" width="250" /></td><td><strong>' + create_date + '</strong> ' + todaysDate + '</td></tr></table><p></p><p></p><p></p><p></p><p></p><p></p><p></p>').append('<div id="tbl_data" class="removetbl2"></div>');
    $('#tbl_data').append($('#divTableDataHolder').html());
    $('.logo_xcl').attr('src', $('.logo_xcl')[0].src);
    $('#tbl_data th').attr('bgcolor', '#015a82').css({ 'color': '#ffffff' });
    $('.border, #tbl_data .dataTable').attr('border', '1');
    $('#tbl_data table td, #tbl_data table th').attr('align', align);
    if ($(e).attr('id') == 'myButtonControlID') { /* For Excel */
        var blobURL = tableToExcel('tbl_data', 'PSS Report');
        if (navigator.msSaveBlob) { // IE 10+
            window.navigator.msSaveBlob(blobURL, $('.top-fixed .title').text() + ' - ' + todaysDate + '.xls')
        } else {
            $(e).attr('download', $('.top-fixed .title').text() + ' - ' + todaysDate + '.xls');
            $(e).attr('href', blobURL);
        };
    }
    setTimeout(function () {
        $('#tbl_data').html('');
        $('.removetbl, .removetbl2').remove();
    }, 1500);

};
function exportXcl(e) {
    console.log('exportXcl');
    var filterUsed = (lang == 'ar') ? "معايير التصفية المستخدمة" : "Filters Used";
    $('#divTableDataHolder').prepend('<table class="removetbl"><tr><td ><img class="logo_xcl" src="dist/images/pscod-logo.png" height="54" width="250" /></td></tr><tr><td>&nbsp;</td></tr><tr><td>&nbsp;</td></tr><tr><td><strong>' + filterUsed + '</strong></td></tr></table>').append('<table class="removetbl"><tr><td>&nbsp;</td></tr><tr><td>&nbsp;</td></tr><tr><td style="font-size:14px;"><strong>' + $('.top-fixed .title').text() + '</strong></td></tr></table><div id="tbl_data" class="removetbl2"></div><table class="removetbl"><tr><td>&nbsp;</td></tr><tr><td></td></tr></table>');
    $('.logo_xcl').attr('src', $('.logo_xcl')[0].src);
    console.log($('.dataTables_wrapper').html());
    $('#tbl_data').html('').append($('.dataTables_wrapper').html());
    $('#divTableDataHolder th').attr('bgcolor', '#015a82').css({ 'color': '#ffffff' });
    $('.border, #tbl_data .dataTable').attr('border', '1');
    $('#tbl_data .dataTable td').css('text-align', 'left');
    $('#tbl_data .row:first-child, #tbl_data .row:last-child ').remove();
    var todaysDate = moment().format('DD-MM-YYYY');
    $('.input-xs').remove();
    if ($(e).attr('id') == 'myButtonControlID') { /* For Excel */
        $('.input-xs').remove();
        var blobURL = tableToExcel('divTableDataHolder', 'PSS Report');
        if (navigator.msSaveBlob) { // IE 10+
            window.navigator.msSaveBlob(blobURL, $('.top-fixed .title').text() + ' - ' + todaysDate + '.xls')
        } else {
            $(e).attr('download', $('.top-fixed .title').text() + ' - ' + todaysDate + '.xls');
            $(e).attr('href', blobURL);
        };
    } else { /* For PDF */
        //$('#divTableDataHolder').prepend('<img class="logoforpdf removetbl2" width="150" style="text-align:center; margin-left:600px;" src="dist/images/logo-moi.png" />');
        //$('.logoforpdf').attr('src', $('.logoforpdf')[0].src);
        $('.removetbl').remove();
        $('.input-xs').remove();
        $('#tblval').css({ 'margin-bottom': '30px', 'margin-top': '30px' });
        $('#hdnhtml').val('').val($('#divTableDataHolder').html());
    }
    setTimeout(function () {
        $('#tbl_data').html('');
        $('.removetbl, .removetbl2').remove();
    }, 1500);


};

function exportXclNoFilter(e) {
    var filterUsed = (lang == 'ar') ? "" : "";
    $('#divTableDataHolder').prepend('<table class="removetbl"><tr><td ><img class="logo_xcl" src="dist/images/pscod-logo.png" height="54" width="250" /></td></tr><tr><td>&nbsp;</td></tr><tr><td>&nbsp;</td></tr><tr><td><strong>' + filterUsed + '</strong></td></tr></table>').append('<table class="removetbl"><tr><td>&nbsp;</td></tr><tr><td>&nbsp;</td></tr><tr><td style="font-size:14px;"><strong>' + $('.top-fixed .title').text() + '</strong></td></tr></table><div id="tbl_data" class="removetbl2"></div><table class="removetbl"><tr><td>&nbsp;</td></tr><tr><td></td></tr></table>');
    $('.logo_xcl').attr('src', $('.logo_xcl')[0].src);

    $('#tbl_data').html('').append($('.dataTables_wrapper').html());
    $('#divTableDataHolder th').attr('bgcolor', '#015a82').css({ 'color': '#ffffff' });
    $('.border, #tbl_data .dataTable').attr('border', '1');
    $('#tbl_data .dataTable td').css('text-align', 'left');
    $('#tbl_data .row:first-child, #tbl_data .row:last-child ').remove();
    var todaysDate = moment().format('DD-MM-YYYY');

    if ($(e).attr('id') == 'myButtonControlID') { /* For Excel */
        var blobURL = tableToExcel('divTableDataHolder', 'PSS Report');
        $(e).attr('download', $('.top-fixed .title').text() + ' - ' + todaysDate + '.xls');
        $(e).attr('href', blobURL);
    } else { /* For PDF */
        //$('#divTableDataHolder').prepend('<img class="logoforpdf removetbl2" width="150" style="text-align:center; margin-left:600px;" src="dist/images/logo-moi.png" />');
        //$('.logoforpdf').attr('src', $('.logoforpdf')[0].src);
        $('.removetbl').remove();
        $('#tblval').css({ 'margin-bottom': '30px', 'margin-top': '30px' });
        $('#hdnhtml').val('').val($('#divTableDataHolder').html());
    }
    setTimeout(function () {
        $('#tbl_data').html('');
        $('.removetbl, .removetbl2').remove();
    }, 1500);
};

function exportXclNOfilter(e) {
    var filterUsed = (lang == 'ar') ? "" : "";
    $('#divTableDataHolder').prepend('<table class="removetbl"><tr><td ><img class="logo_xcl" src="dist/images/pscod-logo.png" height="54" width="250" /></td></tr><tr><td>&nbsp;</td></tr><tr><td>&nbsp;</td></tr><tr><td><strong>' + filterUsed + '</strong></td></tr></table>').append('<table class="removetbl"><tr><td>&nbsp;</td></tr><tr><td>&nbsp;</td></tr><tr><td style="font-size:14px;"><strong>' + $('.top-fixed .title').text() + '</strong></td></tr></table><div id="tbl_data" class="removetbl2"></div><table class="removetbl"><tr><td>&nbsp;</td></tr><tr><td></td></tr></table>');
    $('.logo_xcl').attr('src', $('.logo_xcl')[0].src);

    $('#tbl_data').html('').append($('.dataTables_wrapper').html());
    $('#divTableDataHolder th').attr('bgcolor', '#015a82').css({ 'color': '#ffffff' });
    $('.border, #tbl_data .dataTable').attr('border', '1');
    $('#tbl_data .dataTable td').css('text-align', 'left');
    $('#tbl_data .row:first-child, #tbl_data .row:last-child ').remove();
    var todaysDate = moment().format('DD-MM-YYYY');

    if ($(e).attr('id') == 'myButtonControlID') { /* For Excel */
        var blobURL = tableToExcel('divTableDataHolder', 'PSS Report');
        $(e).attr('download', $('.top-fixed .title').text() + ' - ' + todaysDate + '.xls');
        $(e).attr('href', blobURL);
    } else { /* For PDF */
        //$('#divTableDataHolder').prepend('<img class="logoforpdf removetbl2" width="150" style="text-align:center; margin-left:600px;" src="dist/images/logo-moi.png" />');
        //$('.logoforpdf').attr('src', $('.logoforpdf')[0].src);
        $('.removetbl').remove();
        $('#tblval').css({ 'margin-bottom': '30px', 'margin-top': '30px' });
        $('#hdnhtml').val('').val($('#divTableDataHolder').html());
    }
    setTimeout(function () {
        $('#tbl_data').html('');
        $('.removetbl, .removetbl2').remove();
    }, 1500);
};

//hammad: to make function general, put it here for arabic settings( style, alignments).
function applyArabicSettings() {
    $('head').append('<link href="dist/css/bootstrap-rtl.min.css" rel="stylesheet" />');
}


/* Sticky menu */
$(window).bind('scroll', function () {
    if ($(window).scrollTop() > 10) {
        $('#wrapper').addClass('fxd');
    }
    else {
        $('#wrapper').removeClass('fxd');
    }
});