using System.Diagnostics;
using Inmobiliaria.Models;
using Inmobiliaria.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers;

// Controlador para gestionar los propietarios
public class PropietariosController : Controller
{
    // Logger para registrar eventos y errores
    private readonly ILogger<PropietariosController> _logger;

    // Repositorio para interactuar con la base de datos
    private readonly RepositorioPropietarios repositorio;

    // Constructor para inyectar dependencias
    public PropietariosController(ILogger<PropietariosController> logger)
    {
        _logger = logger;
        repositorio = new RepositorioPropietarios();
    }

    // Método para obtener la lista de propietarios
    public IActionResult ListadoPropietarios()
    {
        // Obtener la lista de propietarios desde el repositorio
        var lista = repositorio.ListarPropietarios();
        return View(lista);
    }

    // Método para crear un nuevo propietario
    public IActionResult CrearPropietario()
    {
        // Crear un nuevo propietario vacío
        return View(new Propietarios());
    }

    // Método para guardar un nuevo propietario
    [HttpPost]
    public IActionResult GuardarPropietario(Propietarios propietario)
    {
        // Verificar si el modelo es válido
        if (ModelState.IsValid)
        {
            // Guardar el propietario en el repositorio
            repositorio.GuardarNuevo(propietario);
            // Redirección a la lista de propietarios
            return RedirectToAction(nameof(ListadoPropietarios));
        }
        // Si el modelo no es válido, devolver la vista con los errores
        return View("CrearPropietario", propietario);
    }

    // Método para editar un propietario existente
    public IActionResult EditarPropietario(int id)
    {
        // Obtener el propietario desde el repositorio
        var propietario = repositorio.ObtenerPropietario(id);
        return View(propietario);
    }

    // Método para actualizar un propietario existente
    [HttpPost]
    public IActionResult ModificarPropietario(Propietarios propietario)
    {
        // Verificar si el modelo es válido
        if (ModelState.IsValid)
        {
            // Actualizar el propietario en el repositorio
            repositorio.ActualizarPropietario(propietario);
            // Redirección a la lista de propietarios
            return RedirectToAction(nameof(ListadoPropietarios));
        }
        // Si el modelo no es válido, devolver la vista con los errores
        return View("EditarPropietario", propietario);
    }

    // Método para eliminar un propietario existente
    public IActionResult EliminarPropietario(int id)
    {
        try
        {
            // Intentar eliminar el propietario desde el repositorio
            repositorio.EliminarPropietario(id);
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
        return RedirectToAction(nameof(ListadoPropietarios));
    }

    // Método para buscar un propietario
    public IActionResult BuscarPropietarios()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Search(Propietarios busqueda)
    {
        var propietario = repositorio.BuscarPropietario(busqueda);

        return View(propietario);
    }

}
