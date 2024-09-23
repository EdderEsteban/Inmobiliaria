console.log("Ingreso a BuscarContratos.js");

// -----------------------------------------------------------------Funcion para los botones ------------------------------------------------------------------

document.addEventListener("DOMContentLoaded", function () {
  const btnVigentes = document.getElementById("btnVigentes");
  const btnTerminados = document.getElementById("btnTerminados");

  // Función para hacer la solicitud y actualizar la tabla
  function cargarInmuebles(url) {
    fetch(url, {
      method: "GET",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
    })
      .then((response) => {
        if (!response.ok) {
          throw new Error("Error en la respuesta del servidor");
        }
        return response.json();
      })
      .then((result) => {
        if (result.success) {
          const contratos = result.data;
          const tableBody = document.querySelector("#tblBodyContratos");
          tableBody.innerHTML = ""; // Limpiar el contenido actual
          // Iterar sobre los contratos recibidos
          contratos.forEach((contrato) => {
            const row = document.createElement("tr");
            row.innerHTML = `
            <td>${contrato.Id_contrato}</td>
              <td>${contrato.Inquilino.Nombre} ${contrato.Inquilino.Apellido}</td>
              <td>${contrato.Inmueble.Direccion}</td>
              <td>${new Date(contrato.Fecha_inicio).toLocaleDateString("es-ES")}</td>
              <td>${new Date(contrato.Fecha_fin).toLocaleDateString("es-ES")}</td>
              <td>${contrato.Inmueble.Precio}</td>
              <td>${contrato.Monto}</td>
              <td>${contrato.Vigencia ? 'Vigente' : 'No Vigente'}</td>
            <td>
                    <a href="/Inmuebles/EditarInmueble/${inmueble.id_inmueble}" class="material-symbols-outlined" title="Editar Inmueble">edit_note</a>
                    <a href="#" class="material-symbols-outlined btnBorrar" title="Borrar Inmueble" data-id="${inmueble.id_inmueble}" style="cursor: pointer;">delete</a>
                    <a href="/Inmuebles/DetallesInmueble/${inmueble.id_inmueble}" class="material-symbols-outlined" title="Detalles Inmueble">visibility</a>
                </td>
              `;
            tableBody.appendChild(row);
          });
        } else {
          Swal.fire({
            title: "Lo siento",
            text: "No se encontraron contratos",
            icon: "info",
          });
        }
      })
      .catch((error) => {
        console.error("Error:", error);
        Swal.fire("Ocurrió un error al cargar los contratos");
      });
  }

  // Listeners para los botones
  btnVigentes.addEventListener("click", function () {
    cargarInmuebles("/Inmuebles/ListadoInmueblesAlquilados"); //cambiar
  });

  btnTerminados.addEventListener("click", function () {
    cargarInmuebles("/Inmuebles/ListadoInmueblesDisponibles"); //cambiar
  });
});
