console.log("Ingreso a BuscarInquilinos.js");

// Funcionalidad Boton Buscar
document.addEventListener("DOMContentLoaded", function () {
  document.getElementById("btnBuscar").addEventListener("click", function () {
    buscarInquilinos();
  });
});
// Funcion para buscar
function buscarInquilinos() {
  console.log("Ingreso a buscarInquilinos");

  const Nombre = document.getElementById("Nombre").value;
  const Apellido = document.getElementById("Apellido").value;
  const Dni = document.getElementById("Dni").value;
  const form = document.getElementById("formSearch");

  if (Nombre === "") {
    if (Apellido === "") {
      if (Dni === "") {
        Swal.fire({
          title: "Sin datos",
          text: "Debe completar al menos uno de los campos",
          icon: "question",
        });
        return;
      }
    }
  }

  const JsonSearch = { Nombre: Nombre, Apellido: Apellido, Dni: Dni };

  fetch("/Inquilinos/BuscarInq", {
    method: "POST",
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
    },
    body: JSON.stringify(JsonSearch),
  })
    .then((response) => response.json())
    .then((data) => {
      
      if (data.length === 0) {
        Swal.fire("No existen resultados a su busqueda.");
        form.reset();
      } else {
        // Actualizar el contenido de la tabla con los datos recibidos
        const tableBody = document.querySelector("#tbodyInquilinos");
        tableBody.innerHTML = ""; // Limpiar el contenido actual

        data.forEach((propietario) => {
          const row = document.createElement("tr");
          row.innerHTML = `
            <td>${propietario.nombre}</td>
            <td>${propietario.apellido}</td>
            <td>${propietario.dni}</td>
            <td>${propietario.direccion}</td>
            <td>${propietario.telefono}</td>
            <td>${propietario.correo}</td>
            <td>
              <a href="/Propietarios/EditarPropietario/${propietario.id_Propietario}" 
                title="Editar" class="material-symbols-outlined">edit_note</a>
              <a href="/Propietarios/EliminarPropietario/${propietario.id_Propietario}" 
               title="Eliminar" class="material-symbols-outlined">delete</a>
            </td>
          `;
          tableBody.appendChild(row);
        });
        form.reset();
      }
    })
    .catch((error) => {
      console.error("Error:", error);
      Swal.fire("Ocurrio un error al buscar el inquilino");
    });
}
