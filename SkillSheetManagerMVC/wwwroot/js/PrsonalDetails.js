// Function to Handel Worked in Japan Check box
function updateHiddenInput(checkbox) {
    var hiddenInput = document.querySelector('input[name="workedInJapan"]');
    hiddenInput.value = checkbox.checked ? "true" : "false";
}

// Function for Preview of Photo
$(document).ready(function () {
    $('#PhotoPath').change(function (e) {
        var url = $('#PhotoPath').val();
        var ext = url.substring(url.lastIndexOf('.') + 1).toLowerCase();
        if (PhotoPath.files && PhotoPath.files[0] && (ext == "gif" || ext == "jpg" || ext == "jfif" || ext == "png" || ext == "bmp")) {
            var reader = new FileReader();
            reader.onload = function () {
                var output = document.getElementById('PreviewImage');
                output.src = reader.result;
            }
            reader.readAsDataURL(e.target.files[0]);
        }
        else {
            alert("Please select a valid image file (GIF, JPG, JFIF, PNG, BMP).");
            $('#PhotoPath').val('');
        }
    });
});

// Set Timeout for Alert Message
window.setTimeout(function () {
    $(".alert").fadeTo(500, 0).slideUp(500, function () {
        $(this).remove();
    });
}, 2000);

// Reload the current page on reset button click
function resetForm() {
    location.reload();
}

// Ask If User Really Wants to reset
$(document).ready(function () {
    $('button[type="reset"]').click(function () {
        var confirmed = confirm("Do you really want to reset?");

        if (!confirmed) {
            return false;
        }
    });
});

$(document).ready(function () {
    $('form').submit(function (e) {
        var birthDateInput = $('#BirthDate');
        var joiningDateInput = $('#JoiningDate');
        var birthDate = new Date(birthDateInput.val());
        var joiningDate = new Date(joiningDateInput.val());

        // Check if Joining Date is Greater than today's Date
        if (joiningDate && joiningDate > new Date()) {
            joiningDateInput.addClass('is-invalid');
            joiningDateInput.next('.text-danger').text('Invalid joining date').show();
            e.preventDefault();
        } else {
            joiningDateInput.removeClass('is-invalid');
            joiningDateInput.next('.text-danger').hide();
        }

        // Validate Birthdate
        if (birthDate) {
            var age = new Date().getFullYear() - birthDate.getFullYear();

            if (age <= 16 || age > 120) {
                birthDateInput.addClass('is-invalid');
                birthDateInput.next('.text-danger').text('Invalid birthdate').show();
                e.preventDefault();

            }
        } else {
            birthDateInput.removeClass('is-invalid');
            birthDateInput.next('.text-danger').hide();

            // Validate Joining Date
            if (birthDate && joiningDate) {
                var joiningAtAge = Math.abs(joiningDate.getFullYear() - birthDate.getFullYear());
                var isJoiningGreaterToday = joiningDate > new Date();

                if (joiningAtAge < 16 || isJoiningGreaterToday) {
                    joiningDateInput.addClass('is-invalid');
                    joiningDateInput.next('.text-danger').text('Invalid joining date').show();
                    e.preventDefault();
                } else {
                    joiningDateInput.removeClass('is-invalid');
                    joiningDateInput.next('.text-danger').hide();
                }
            }
        }
    });
});