// Function to submit GroupName, UserName, Password on Login Button Click
$(function () {
    $('#LoginButton').click(function () {
        // Get the selected values
        var selectedGroup = $('#GroupNameDropdown').val();
        var selectedUser = $('#UserNameDropdown').val();
        var password = $('#PasswordInput').val();

        // Perform form submission
        $('form').submit();
    });
});

// Reload the current page on reset button click
function resetForm() {
    location.reload();
}

// Set Timeout for Alert Message
window.setTimeout(function () {
    $(".alert").fadeTo(500, 0).slideUp(500, function () {
        $(this).remove();
    });
}, 2000);

// For toggle Hide and Unhide Passsword
$(function () {
    const togglePassword = document.querySelector('#togglePassword');
    const password = document.querySelector('#password');

    togglePassword.addEventListener('click', function (e) {
        // toggle the type attribute
        const type = password.getAttribute('type') === 'password' ? 'text' : 'password';
        password.setAttribute('type', type);
        // toggle the eye slash icon
        this.classList.toggle('fa-eye-slash')
    });
});