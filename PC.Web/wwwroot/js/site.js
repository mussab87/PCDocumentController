// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    $("#loaderbody").addClass('hide');

    $(document).bind('ajaxStart', function () {
        $("#loaderbody").removeClass('hide');
    }).bind('ajaxStop', function () {
        $("#loaderbody").addClass('hide');
    });
});

showInPopup = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#form-modal .modal-body').html(res);
            $('#form-modal .modal-title').html(title);
            $('#form-modal').modal('show');
            // to make popup draggable
            $('.modal-dialog').draggable({
                handle: ".modal-header"
            });
        }
    })
}

jQueryAjaxPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    $('#view-all').html(res.html)
                    $('#form-modal .modal-body').html('');
                    $('#form-modal .modal-title').html('');
                    $('#form-modal').modal('hide');
                }
                else
                    $('#form-modal .modal-body').html(res.html);
            },
            error: function (err) {
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}

jQueryAjaxDelete = form => {
    Swal.fire({
        title: 'هل أنت متأكد من عملية الحذف ؟',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        cancelButtonText: 'الغاء',
        confirmButtonText: 'نعم'
    }).then((result) => {
        if (result.isConfirmed) {
            try {
                $.ajax({
                    type: 'POST',
                    url: form.action,
                    data: new FormData(form),
                    contentType: false,
                    processData: false,
                    success: function (res) {
                        Swal.fire({
                            title: "تم الحذف بنجاح",
                            icon: 'success',
                            //confirmButtonText: 'موافق',
                            showConfirmButton: false,
                            showCloseButton: false
                        })
                        location.reload();
                    },
                    error: function (err) {
                        console.log(err)
                    }
                })
            } catch (ex) {
                console.log(ex)
            }
        }

    })


    //prevent default form submit event
    return false;
}

//Manage Roles******************************************

$(function () {
    var placeHolderElement = $('#PlaceHolderHere');

    $('button[data-toggle="ajax-modal"').click(function (event) {
        //get url action from data-url from the button tag
        var url = $(this).data('url');
        var decodedUrl = decodeURIComponent(url);
        $.get(decodedUrl).done(function (data) {
            placeHolderElement.html(data);
            placeHolderElement.find('.modal').modal('show');
        })

    })

    placeHolderElement.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.modal').find('form');

        var actionName = form.attr('action');
        var controllerName = form.attr('controller');

        var actionUrl = "/" + controllerName + "/" + actionName;

        var sendData = form.serialize();
        var userId = document.getElementById("SelecteduserId").value;


        $.post(actionUrl, sendData + "&userId=" + userId).done(function (data) {

            window.location.reload();
            placeHolderElement.find('.modal').modal('hide');
        })
    })
})

//*******************************************************//Manage Users******************************************
$(function () {
    var placeHolderElement = $('#PlaceHolderHereUser');

    $('button[data-toggle="ajax-modal"').click(function (event) {
        //get url action from data-url from the button tag
        var url = $(this).data('url');
        var decodedUrl = decodeURIComponent(url);
        $.get(decodedUrl).done(function (data) {
            placeHolderElement.html(data);
            placeHolderElement.find('.modal').modal('show');
        })

    })

    placeHolderElement.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.modal').find('form');

        var actionName = form.attr('action');
        var controllerName = form.attr('controller');

        var actionUrl = "/" + controllerName + "/" + actionName;

        var sendData = form.serialize();

        var roleId = document.getElementById("SelectedRoleId").value;


        $.post(actionUrl, sendData + "&roleId=" + roleId).done(function (data) {
            alert("Roles have been added to selected user")
            window.location.reload();
            placeHolderElement.find('.modal').modal('hide');
        })


    })
})

//****************************************************************************************************************

//*******************************************************//Activity and details******************************************
function getActivity() {

    var Selected_MainCategory = $('#MainCategoryId').val(); //document.getElementById("MainCategoryId").value;

    $.post("/CategoryHeader/GetActivity",
        { selectedMainCategory: Selected_MainCategory },
        function (data, status) {
            //alert("Data: " + data + "\nStatus: " + status);

            if (data == "[]" || data == false) {
                swal({
                    title: "Sorry",
                    text: "No Activity Added into this Main Category",
                    type: "warning",
                    confirmButtonText: "Ok",
                });
            }
            $("#ActivityId").empty();

            $('#ActivityId').append($('<option>', {
                value: 0,
                text: 'Please select Activity'
            }));

            $.each(data, function (index, i) {

                $('#ActivityId').append($('<option>', {
                    value: i.activityId,
                    text: i.name
                }));
            });
        });
    //getDetails();

}

function getDetails() {

    var Selected_Activity = $('#ActivityId').val(); //document.getElementById("ActivityId").value;

    $.post("/CategoryHeader/GetDetails",
        { Selected_Activity: Selected_Activity },
        function (data, status) {

            if (data == "[]" || data == false) {
                swal({
                    title: "Sorry",
                    text: "No Details Added into this Activity",
                    type: "warning",
                    confirmButtonText: "Ok",
                });
            }
            $("#DetailsId").empty();

            $('#DetailsId').append($('<option>', {
                value: 0,
                text: 'Please select Details'
            }));

            $.each(data, function (index, i) {

                $('#DetailsId').append($('<option>', {
                    value: i.detailsId,
                    text: i.name
                }));
            });
        });
}

//*******************************************************//Activity and details******************************************


//Manage UserLevelRole******************************************

$(function () {
    var placeHolderElementLevel = $('#PlaceHolderLevelRole');

    $('button[data-toggle="ajax-modal"').click(function (event) {
        //get url action from data-url from the button tag
        var url = $(this).data('url');
        var decodedUrl = decodeURIComponent(url);
        $.get(decodedUrl).done(function (data) {
            placeHolderElementLevel.html(data);
            placeHolderElementLevel.find('.modal').modal('show');
        })

    })

    placeHolderElementLevel.on('click', '[data-save="modalLevel"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.modal').find('form');

        var actionName = "ManageUserLevelRole";
        var controllerName = "CategoryHeader";

        var actionUrl = "/" + controllerName + "/" + actionName;

        var sendData = form.serialize();
        var LevelId = document.getElementById("LevelId").value;
        var OldroleId = document.getElementById("UserOldRoleId").value;
        var NewroleId = document.getElementById("UserNewRoleId").value;

        $.post(actionUrl, sendData + "&LevelId=" + LevelId + "&OldroleId=" + OldroleId + "&NewroleId=" + NewroleId).done(function (data) {

            window.location.reload();
            placeHolderElementLevel.find('.modal').modal('hide');
        })
    })
})

//*******************************************************//Manage UserLevelRole******************************************