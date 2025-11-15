// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function confirmDelete(id, pages, tipo, fullName) {
    Swal.fire({
        title: '¿Estás seguro?',
        html: `¿Deseas eliminar al ${tipo} <strong>${fullName}</strong>? Esta acción no se puede deshacer.`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar',
        cancelButtonText: 'Cancelar',
        reverseButtons: true,
        customClass: {
            confirmButton: 'btn btn-danger m-2',
            cancelButton: 'btn btn-secondary m-2'
        },
        buttonsStyling: false
    }).then((result) => {
        if (result.isConfirmed) {
            // Crear y enviar formulario POST con token anti-forgery
            const form = document.createElement('form');
            form.method = 'POST';
            form.action = `/${pages}/Delete?id=${id}`;

            // Obtener el token anti-forgery (necesario en Razor Pages)
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            const tokenInput = document.createElement('input');
            tokenInput.type = 'hidden';
            tokenInput.name = '__RequestVerificationToken';
            tokenInput.value = token;
            form.appendChild(tokenInput);

            document.body.appendChild(form);
            form.submit();
        }
    });
}