document.addEventListener("DOMContentLoaded", function () {
    const successMessage = '@TempData["SuccessMessage"]';
    const errorMessage = '@TempData["ErrorMessage"]';

    if (successMessage) {
        Swal.fire({
            icon: 'success',
            title: 'Deleted!',
            text: successMessage,
            confirmButtonColor: '#3085d6',
        });
    } else if (errorMessage) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: errorMessage,
            confirmButtonColor: '#d33',
        });
    }
});