using System.Diagnostics;
using Inmobiliaria.Models;
using Inmobiliaria.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    public class PagoController : Controller
    {
        private readonly ILogger<PagoController> _logger;
        private readonly RepositorioPago repositorio;

        public PagoController(ILogger<PagoController> logger)
        {
            _logger = logger;
            repositorio = new RepositorioPago();
        }

        [HttpGet]
        public IActionResult ListadoPagos()
        {
            var lista = repositorio.ListarPagos();
            ViewBag.inmuebles = new RepositorioInmuebles().ListarTodosInmuebles();
            ViewBag.inquilinos = new RepositorioInquilinos().ListarInquilinos();
            ViewBag.contratos = new RepositorioContratos().ListarContratos();

            return View(lista);
        }

        [HttpGet]
        public IActionResult BuscarInquilinoPago()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CrearPago(int id)
        {
            Console.WriteLine($"este es el id en controller: {id}");
            // Obtener el inmueble por ID
            var inmueblexId = new RepositorioInmuebles().ObtenerInmueble(id);
            if (inmueblexId == null)
            {
                return NotFound(); // Retorna un 404 si no se encuentra el inmueble
            }

            ViewBag.Inmueble = inmueblexId; // Pasar el inmueble al ViewBag

            // Obtener el contrato asociado al inmueble
            var contratoxId = new RepositorioContratos().ObtenerContratoInmueble(id);
            if (contratoxId == null)
            {
                return NotFound(); // Retorna un 404 si no se encuentra el contrato
            }

            ViewBag.Contrato = contratoxId; // Pasar el contrato al ViewBag

            // Obtener el inquilino asociado al contrato
            var inquilinoxId = new RepositorioInquilinos().ObtenerInquilino(
                contratoxId.Id_inquilino
            );
            if (inquilinoxId != null)
            {
                ViewBag.Inquilino = inquilinoxId; // Pasar el inquilino al ViewBag si se encuentra
            }

            return View();
        }

        [HttpPost]
        public IActionResult GuardarPago(Pago pago)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repositorio.GuardarNuevo(pago);
                    TempData["SuccessMessage"] = "Pago guardado exitosamente.";
                    return RedirectToAction(nameof(ListadoPagos));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al guardar el pago.");
                    ModelState.AddModelError(
                        "",
                        "Ocurrió un error al guardar el pago. Intente nuevamente."
                    );
                }
            }
            return View("CrearPago", pago);
        }

        [HttpGet]
        public IActionResult EditarPago(int id)
        {
            var pago = repositorio.ObtenerPago(id);
            if (pago == null)
            {
                return NotFound(); // Retorna un 404 si no se encuentra el pago
            }

            ViewBag.contratos = new RepositorioContratos().ListarContratos();
            ViewBag.inquilinos = new RepositorioInquilinos().ListarInquilinos();
            return View(pago);
        }

        [HttpPost]
        public IActionResult ActualizarPago(Pago pago)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repositorio.ActualizarPago(pago);
                    TempData["SuccessMessage"] = "Pago actualizado exitosamente.";
                    return RedirectToAction(nameof(ListadoPagos));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar el pago.");
                    ModelState.AddModelError(
                        "",
                        "Ocurrió un error al actualizar el pago. Intente nuevamente."
                    );
                }
            }
            return View("EditarPago", pago);
        }

        [HttpPost]
        public IActionResult EliminarPago(int id)
        {
            try
            {
                repositorio.EliminarPago(id);
                TempData["SuccessMessage"] = "Pago eliminado exitosamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el pago.");
                TempData["ErrorMessage"] =
                    "Ocurrió un error al eliminar el pago. Intente nuevamente.";
            }
            return RedirectToAction(nameof(ListadoPagos));
        }

        [HttpGet]
        public IActionResult DetallesPago(int id)
        {
            var pago = repositorio.ObtenerPago(id);
            if (pago == null)
            {
                return NotFound(); // Retorna un 404 si no se encuentra el pago
            }
            return View(pago);
        }

        [HttpPost]
        public IActionResult BuscarInqPago([FromBody] BusquedaInquilinos busqueda)
        {
            // Realiza la búsqueda de los inquilinos
            var inquilinos = new RepositorioInquilinos().BuscarInquilinos(busqueda);

            // Itera sobre los inquilinos encontrados y busca el contrato e inmueble asociado para cada uno
            var resultadoConContratosEInmuebles = inquilinos
                .Select(i =>
                {
                    // Obtener el contrato del inquilino
                    var contrato = new RepositorioContratos().ObtenerContratoxInquilino(
                        i.Id_inquilino
                    );

                    // Si el contrato existe, obtener el inmueble asociado
                    var inmueble =
                        contrato != null
                            ? new RepositorioInmuebles().ObtenerInmueble(contrato.Id_inmueble)
                            : null;

                    // Retorna un objeto que contiene la información del inquilino, contrato e inmueble
                    return new
                    {
                        Id_Inquilino = i.Id_inquilino,
                        Nombre = i.Nombre,
                        Apellido = i.Apellido,
                        Dni = i.Dni,
                        Contrato = contrato != null
                            ? new
                            {
                                Id_Contrato = contrato.Id_contrato,
                                Id_Inmueble = contrato.Id_inmueble,
                                Fecha_Inicio = contrato.Fecha_inicio,
                                Fecha_Fin = contrato.Fecha_fin,
                                Inmueble = inmueble != null
                                    ? new
                                    {
                                        Id_Inmueble = inmueble.Id_inmueble,
                                        inmueble.Direccion,
                                    }
                                    : null // Si no hay inmueble, será null
                            }
                            : null // Si no hay contrato, será null
                    };
                })
                .Where(x => x.Contrato != null) // Filtra solo los que tienen contrato
                .ToList();

            // Devuelve los resultados como JSON, incluyendo solo inquilinos con contrato e inmueble
            return Json(resultadoConContratosEInmuebles);
        }
    }
}
