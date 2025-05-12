$(document).ready(function () {
    var successMessage = $("#successMessage").val();
    var errorMessage = $("#errorMessage").val();

    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: 'Success',
            html: successMessage
        });
    }

    if (errorMessage) {
        // Convert `<br/>` separated messages into a list format for better readability
        var formattedErrors = errorMessage.replace(/<br\/?>/g, "\n");

        Swal.fire({
            icon: 'error',
            title: 'Error',
            html: formattedErrors.replace(/\n/g, "<br/>")
        });
    }
});
