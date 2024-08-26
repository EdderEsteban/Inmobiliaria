using System.Diagnostics;
using Inmobiliaria.Models;
using Inmobiliaria.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers;

// Controlador para gestionar los inquilinos
public class InquilinosController : Controller
{
    // Logger para registrar eventos y errores
    private readonly ILogger<InquilinosController> _logger;

    // Repositorio para interactuar con la base de datos
    private readonly RepositorioInquilinos repositorio;

    // Constructor para inyectar dependencias
    public InquilinosController(ILogger<InquilinosController> logger)
    {
        _logger = logger;
        repositorio = new RepositorioInquilinos();
    }

    public IActionResult ListadoInquilinos()
    {
        var lista = repositorio.ListarInquilinos();
        return View(lista);
    }

    public IActionResult CrearInquilino()
    {
        return View();
    }

    public IActionResult GuardarInquilino(Inquilinos inquilino)
    {
        if (ModelState.IsValid) //Asegurarse q es valido el modelo
        {
            repositorio.GuardarNuevo(inquilino);
            return RedirectToAction(nameof(ListadoInquilinos));
        }
        return View("CrearInquilino", inquilino);
    }

    public IActionResult EditarInquilino(int id)
    {
        if (id > 0)
        {
            var inquilino = repositorio.ObtenerInquilino(id);
            return View(inquilino);
        }
        else
        {
            return View();
        }
    }

    public IActionResult ModificarInquilino(Inquilinos inquilino)
    {
        if (ModelState.IsValid) //Asegurarse q es valido el modelo
        {
            repositorio.ActualizarInquilino(inquilino);
            return RedirectToAction(nameof(ListadoInquilinos));
        }
        return View("EditarInquilino", inquilino);
    }

    // Método para eliminar un inquilino existente
    public IActionResult EliminarInquilino(int id)
    {
        try
        {
            // Intentar eliminar el inquilino desde el repositorio
            repositorio.EliminarInquilino(id);
            // Establecer mensaje de éxito
            TempData["SuccessMessage"] = "Propietario eliminado exitosamente.";
        }
        catch (InvalidOperationException ex)
        {
            // Capturar la excepción específica y establecer el mensaje de error
            TempData["ErrorMessage"] = ex.Message;
        }
        catch (Exception ex)
        {
            // Capturar cualquier otra excepción y establecer un mensaje de error genérico
            TempData["ErrorMessage"] = $"Ocurrió un error al eliminar el propietario. {ex.Message}";
        }

        // Redirigir a la lista de propietarios
        return RedirectToAction(nameof(ListadoInquilinos));
    }

    
        // Método para buscar un inquilino
        public IActionResult BuscarInquilinos()
        {
            return View();
        }
    
        // Método para recibir el formulario Search
        [HttpPost]
        public IActionResult BuscarInq([FromBody] BusquedaInquilinos busqueda)
        {
            Console.WriteLine(
                $"Busqueda en Controller: {busqueda.Nombre}, {busqueda.Apellido}, {busqueda.Dni}");
    
            var resultados = repositorio.BuscarInquilinos(busqueda);
    
            // Devuelve los resultados como JSON
            return Json(resultados);
        }
}
