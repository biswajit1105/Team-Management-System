// Function for Add Button Click
$(function () {
    var placeHolderElement = $('#PlaceHolderHere');
    $('button[data-toggle="ajax-modal-Add"]').click(function (event) {
        var url = $(this).data('url');
        var decodedUrl = decodeURIComponent(url);
        $.get(decodedUrl).done(function (data) {
            placeHolderElement.html(data);
            placeHolderElement.find('.modal').modal('show');
        });
    });
});

// Function for Delete Button Click
$(function () {
    var placeHolderElement = $('#PlaceHolderHere');
    $('button[data-toggle="ajax-modal-Delete"]').click(function (event) {
        var url = $(this).data('url');
        var selectedUsernames = [];

        $('input[type="checkbox"]:checked').each(function () {
            var username = $(this).closest('tr').find('td:eq(1)').text();
            selectedUsernames.push(username);
        });

        if (selectedUsernames.length > 0 && confirm("Do You Really want to Delete User!") == true) {

            // Construct the URL with the selected usernames as a parameter
            var usernamesParam = encodeURIComponent(selectedUsernames.join(','));
            var modifiedUrl = url + '?usernames=' + usernamesParam;

            var decodedUrl = decodeURIComponent(modifiedUrl);
            $.get(decodedUrl).done(function (data, textStatus, xhr) {
                if (xhr.status === 200) {
                    placeHolderElement.html(data);
                    placeHolderElement.find('.modal').modal('show');
                    alert('User Deleted Sucessfull');
                    location.reload();
                } else {
                    alert('Delete User Failed');
                    location.reload();
                }
            });
        }
    });
});

// Function for Edit Button Click
$(function () {
    var placeHolderElement = $('#PlaceHolderHere');
    $('button[data-toggle="ajax-modal-Edit"]').click(function (event) {
        var url = $(this).data('url');
        var selectedUsernames = "";

        $('input[type="checkbox"]:checked').each(function () {
            var username = $(this).closest('tr').find('td:eq(1)').text();
            selectedUsernames += username + ",";
        });

        if (selectedUsernames.length > 0) {
            var usernamesParam = encodeURIComponent(selectedUsernames.slice(0, -1));
            var userName = $(this).data('username');
            var modifiedUrl = url + '?usernames=' + usernamesParam + '&adminName=' + encodeURIComponent(userName);

            var decodedUrl = decodeURIComponent(modifiedUrl);
            $.get(decodedUrl).done(function (data) {
                placeHolderElement.html(data);
                placeHolderElement.find('.modal').modal('show');
            });
        }
    });
});

// Function for Enable and Disable Buttons as per Requirement
$(document).ready(function () {
    var checkboxes = $('input[type="checkbox"]');
    var btnAdd = $('#AddLink');
    var btnEdit = $('#EditLink');
    var btnDelete = $('#DeleteLink');
    checkboxes.change(function () {
        var checkedCheckboxes = checkboxes.filter(':checked');

        if (checkedCheckboxes.length === 0) {
            // No checkboxes checked
            btnAdd.prop('disabled', false);
            btnEdit.prop('disabled', true);
            btnDelete.prop('disabled', true);
        }
        else if (checkedCheckboxes.length === 1) {
            // Only one checkbox checked
            btnAdd.prop('disabled', true);
            btnEdit.prop('disabled', false);
            btnDelete.prop('disabled', false);
        }
        else {
            // More than one checkbox checked
            btnAdd.prop('disabled', true);
            btnEdit.prop('disabled', true);
            btnDelete.prop('disabled', false);
        }
    });
});

// Set Timeout for Alert Message
window.setTimeout(function () {
    $(".alert").fadeTo(500, 0).slideUp(500, function () {
        $(this).remove();
    });
}, 2000);