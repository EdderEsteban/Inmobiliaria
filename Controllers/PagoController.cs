using System.Diagnostics;
using Inmobiliaria.Models;
using Inmobiliaria.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers;

public class PagoController : Controller
{
    private readonly ILogger<PagoController> _logger;

    // Repositorio para interactuar con la base de datos
    private readonly RepositorioPago repositorio;

    public PagoController(ILogger<PagoController> logger)
    {
        _logger = logger;
        repositorio = new RepositorioPago();
    }

    [HttpGet]
    public IActionResult ListadoPagos()
    {
        //Lista de Pagos
        var lista = repositorio.ListarPagos();

        //Enviar la lista de Inmuebles
        RepositorioInmuebles repoInmueble = new RepositorioInmuebles();
        var listaInmuebles = repoInmueble.ListarTodosInmuebles();
        ViewBag.inmuebles = listaInmuebles;

        // Enviar la lista de Inquilinos
        RepositorioInquilinos repoInquilino = new RepositorioInquilinos();
        var listaInquilinos = repoInquilino.ListarInquilinos();
        ViewBag.inquilinos = listaInquilinos;

        //Enviar la lista de Contratos
        RepositorioContratos repoContratos = new RepositorioContratos();
        var listaContratos = repoContratos.ListarContratos();
        ViewBag.contratos = listaContratos;

        return View(lista);
    }

    [HttpGet]
    public IActionResult CrearPago(int id)
    {
        //Enviar el Inmueble
        RepositorioInmuebles repoInmueble = new RepositorioInmuebles();
        var inmueblexId = repoInmueble.ObtenerInmueble(id);
        ViewBag.inmueble = inmueblexId;

        //Enviar la lista de Contratos
        RepositorioContratos repoContratos = new RepositorioContratos();
        var contratoxId = repoContratos.ObtenerContratoInmueble(id);
        ViewBag.contratos = contratoxId;

        // Enviar la lista de Inquilinos
        RepositorioInquilinos repoInquilino = new RepositorioInquilinos();
        var inquilinoxId = repoInquilino.ObtenerInquilino(contratoxId.Id_inquilino);
        ViewBag.inquilino = inquilinoxId;

        return View(); 
    }

    [HttpPost]
    public IActionResult GuardarPago(Pago pago)
    {
        if (ModelState.IsValid)
        {
            RepositorioPago repo = new RepositorioPago();
            repo.GuardarNuevo(pago);
            return RedirectToAction(nameof(ListadoPagos));
        }
        return View("CrearPago", pago);
    }
}
