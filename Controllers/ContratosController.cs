using System.Diagnostics;
using Inmobiliaria.Models;
using Inmobiliaria.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers;

// Controlador para gestionar los inquilinos
public class ContratosController : Controller
{
    // Logger para registrar eventos y errores
    private readonly ILogger<ContratosController> _logger;

    // Repositorio para interactuar con la base de datos
    private readonly RepositorioContratos repositorio;

    // Constructor para inyectar dependencias
    public ContratosController(ILogger<ContratosController> logger)
    {
        _logger = logger;
        repositorio = new RepositorioContratos();
    }

    // Metodo para obtener la lista de contratos
    [HttpGet]
    public IActionResult ListadoContratos()
    {
        //Enviar la lista de Inmuebles Disponibles
        RepositorioInmuebles repoInmueble = new RepositorioInmuebles();
        var listaInmueblesDisponibles = repoInmueble.ListarTodosInmuebles();
        ViewBag.inmueblesDisponibles = listaInmueblesDisponibles;

        // Enviar la lista de Inquilinos
        RepositorioInquilinos repoInquilino = new RepositorioInquilinos();
        var listaInquilinos = repoInquilino.ListarInquilinos();
        ViewBag.inquilinos = listaInquilinos;

        //Enviar la lista de Contratos
        var lista = repositorio.ListarContratos();
        return View(lista);
    }

    // Metodo para buscar un contrato
    [HttpGet]
    public IActionResult BuscarContrato(int IdContrato, String? NombreInquilino)
    {
        //Enviar la lista de Inmuebles Disponibles
        RepositorioInmuebles repoInmueble = new RepositorioInmuebles();
        var listaInmueblesDisponibles = repoInmueble.ListarTodosInmuebles();
        ViewBag.inmueblesDisponibles = listaInmueblesDisponibles;

        // Enviar la lista de Inquilinos
        RepositorioInquilinos repoInquilino = new RepositorioInquilinos();
        var listaInquilinos = repoInquilino.ListarInquilinos();
        ViewBag.inquilinos = listaInquilinos;

        //Enviar la lista de Contratos
        var ListarContratos = repositorio.ListarContratos();
        ViewBag.contratos = ListarContratos;
        return View();
    }

    // Metodo para crear un nuevo contrato
    [HttpGet]
    public IActionResult CrearContrato()
    {
        //Enviar la lista de Inmuebles Disponibles
        RepositorioInmuebles repoInmueble = new RepositorioInmuebles();
        var listaInmueblesDisponibles = repoInmueble.ListarInmueblesDisponibles();
        ViewBag.inmueblesDisponibles = listaInmueblesDisponibles;

        // Enviar la lista de Inquilinos
        RepositorioInquilinos repoInquilino = new RepositorioInquilinos();
        var listaInquilinos = repoInquilino.ListarInquilinos();
        ViewBag.inquilinos = listaInquilinos;

        return View();
    }

    // Metodo para crear un nuevo contrato por id
    [HttpGet]
    public IActionResult CrearContratoId(int id)
    {
        //Enviar la lista de Inmuebles Disponibles
        RepositorioInmuebles repoInmueble = new RepositorioInmuebles();
        var inmueble = repoInmueble.ObtenerInmueble(id);
        ViewBag.inmueble = inmueble;

        // Enviar la lista de Inquilinos
        RepositorioInquilinos repoInquilino = new RepositorioInquilinos();
        var listaInquilinos = repoInquilino.ListarInquilinos();
        ViewBag.inquilinos = listaInquilinos;

        return View();
    }

    // Metodo para guardar un nuevo contrato
    [HttpPost]
    public IActionResult GuardarContrato(Contrato contrato)
    {
        if (ModelState.IsValid)
        {
            repositorio.GuardarNuevo(contrato);

            // Cambiar el estado del inmueble
            RepositorioInmuebles repoinmueble = new RepositorioInmuebles();
            repoinmueble.CambiarEstadoInmueble(contrato.Id_inmueble);

            return RedirectToAction("ListadoInmueblesAlquilados", "Inmueble");
        }
        return View("CrearContrato", contrato);
    }

    // Metodo para editar un contrato
    [HttpGet]
    public IActionResult EditarContrato(int id)
    {
        if (id > 0)
        {
            // Obtener el contrato específico según el ID
            var contrato = repositorio.ObtenerContrato(id);

            // Verificar si el contrato existe
            if (contrato != null)
            {
                //Enviar el Inmueble
                RepositorioInmuebles repoinmueble = new RepositorioInmuebles();
                var inmueble = repoinmueble.ObtenerInmueble(contrato.Id_inmueble);
                ViewBag.inmueble = inmueble;

                //Enviar el Inquilino
                RepositorioInquilinos repoInquil = new RepositorioInquilinos();
                var inquilino = repoInquil.ObtenerInquilino(contrato.Id_inquilino);
                ViewBag.inquilino = inquilino;

                return View(contrato);
            }
            else
            {
                // Manejar el caso en que el contrato no existe
                return NotFound();
            }
        }
        else
        {
            // Manejar el caso en que el ID no es válido
            return BadRequest();
        }
    }

    // Metodo para modificar un contrato
    [HttpPut]
    public IActionResult ModificarContrato(Contrato contrato)
    {
        if (ModelState.IsValid)
        {
            // Actualizar el contrato
            repositorio.ActualizarContrato(contrato);
            return RedirectToAction(nameof(ListadoContratos));
        }
        return View("EditarContrato", contrato);
    }

    // Metodo para eliminar un contrato
    [HttpDelete]
    public IActionResult EliminarContrato(int id)
    {
        repositorio.EliminarContrato(id);
        return RedirectToAction(nameof(ListadoContratos));
    }
} 

