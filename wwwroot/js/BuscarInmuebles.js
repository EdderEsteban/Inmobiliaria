console.log("Ingreso a BuscarInmuebles.js");

document.addEventListener("DOMContentLoaded", function() {
    const deleteButtons = document.querySelectorAll('.btnBorrar');

    deleteButtons.forEach(button => {
        button.addEventListener('click', function() {
            const inmuebleId = this.getAttribute('data-id');

            // Confirmación con SweetAlert
            Swal.fire({
                title: '¿Estás seguro?',
                text: "¡No podrás revertir esto!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'Sí, bórralo!',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Si el usuario confirma, hacer la petición de borrado
                    fetch(`/Inmuebles/Borrar/${inmuebleId}`, {
                        method: 'DELETE',
                        headers: {
                            'Content-Type': 'application/json',
                            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                        }
                    })
                    .then(response => response.json())
                    .then(data => {
                        if (data.success) {
                            Swal.fire(
                                '¡Borrado!',
                                'El inmueble ha sido borrado.',
                                'success'
                            ).then(() => {
                                // Recargar la página o eliminar la fila de la tabla
                                location.reload();
                            });
                        } else {
                            Swal.fire(
                                'Error',
                                'No se pudo borrar el inmueble.',
                                'error'
                            );
                        }
                    })
                    .catch(error => {
                        Swal.fire(
                            'Error',
                            'Ocurrió un error al intentar borrar el inmueble.',
                            'error'
                        );
                    });
                }
            });
        });
    });
});

document.addEventListener("DOMContentLoaded", function () {
    // Escuchar el botón buscar
    const btnBuscar = document.getElementById("btnBuscar");

    btnBuscar.addEventListener("click", function () {
        // Obtener el valor del input de búsqueda
        const idInmueble = document.getElementById("Id").value;

        // Validar si hay algún valor en el campo
        if (!idInmueble) {
            Swal.fire({
                title: "Sin datos",
                text: "Debe completar el campo Id",
                icon: "question",
              });
              return;
        }

        // Realizar la solicitud fetch
        fetch(`/Inmuebles/BuscarInmueblePorId?id=${idInmueble}`, {
            method: "POST",
            headers: {
              Accept: "application/json",
              "Content-Type": "application/json",
            },
            body: JSON.stringify(idInmueble),
          })
        })
        .then((response) => response.json())
    .then((data) => {
        // Mostrar los datos del inmueble
    })
    .catch((error) => {
      console.error("Error:", error);
      Swal.fire("Ocurrio un error al buscar el inmueble");
    });
})