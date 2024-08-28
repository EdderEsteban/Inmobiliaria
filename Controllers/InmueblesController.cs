using System.Diagnostics;
using Inmobiliaria.Models;
using Inmobiliaria.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers;

// Controlador para gestionar los propietarios
public class InmueblesController : Controller
{
    // Logger para registrar eventos y errores
    private readonly ILogger<InmueblesController> _logger;

    // Repositorio para interactuar con la base de datos
    private readonly RepositorioInmuebles repositorio;

    // Constructor para inyectar dependencias
    public InmueblesController(ILogger<InmueblesController> logger)
    {
        _logger = logger;
        repositorio = new RepositorioInmuebles();
    }

    // Método para obtener la lista de propietarios
    public IActionResult ListadoTodosInmuebles()
    {
        // Obtener la lista de propietarios desde el repositorio
        var lista = repositorio.ListarTodosInmuebles();
        return View(lista);
    }

    // Metodo para listar todos los Inmuebles alquilados
    /*public IActionResult ListadoInmueblesAlquilados()
    {
        // Enviar la lista de Inmuebles Alquilados
        var lista = repositorio.ListarInmueblesAlquilados();

        // Enviar la lista de los Contratos
        RepositorioContrato repoContrato = new RepositorioContrato();
        var contratos = repoContrato.ListarContratos();
        ViewBag.contratos = contratos;

        // Enviar la lista de Inquilinos
        RepositorioInquilino repoInquilino = new RepositorioInquilino();
        var inquilinos = repoInquilino.ListarInquilinos();
        ViewBag.inquilinos = inquilinos;

        return View(lista);
    }*/

    // Metodo para editar un inmueble
    public IActionResult EditarInmueble(int id)
    {
        //Enviar la lista de tipos de inmueble
        var listTipos = repositorio.ListarTiposInmueble();
        ViewBag.tipos = listTipos;

        //Enviar la lista de propietarios
        RepositorioPropietarios repoProp = new RepositorioPropietarios();
        var listPropietarios = repoProp.ListarPropietarios();
        ViewBag.propietarios = listPropietarios;

        var inmueble = repositorio.ObtenerInmueble(id);

        return View(inmueble);
    }

    // Metodo para modificar un inmueble
    public IActionResult ModificarInmueble(Inmuebles inmueble)
    {
        if (ModelState.IsValid)
        {
            // Crear el repositorio de Contratos
            RepositorioContratos repoContrato = new RepositorioContratos();

            // Obtener todos los contratos asociados al inmueble
            var contratosDelInmueble = repoContrato.ListarContratosPorInmueble(inmueble.Id_inmueble);

            // Verificar si existen contratos asociados al inmueble
            if (contratosDelInmueble.Count > 0)
            {
                // Si hay contratos, ver si Inmueble.Disponible es 1 o 0
                Inmuebles inmuebleOriginal = repositorio.ObtenerInmueble(inmueble.Id_inmueble); // Obtener el inmueble original de la BD
                if (inmuebleOriginal != null)
                {
                    if (inmuebleOriginal.Disponible == true)
                    {
                        // Permitir la modificación de todos los campos
                        repositorio.ActualizarInmueble(inmueble);
                    }
                    else
                    {
                        // Permitir la modificación de todos los campos excepto Disponible
                        inmueble.Disponible = inmuebleOriginal.Disponible; // Mantener el valor original de Disponible
                        repositorio.ActualizarInmuebleExceptoDisponible(inmueble);
                        TempData["AlertMessage"] =
                            "Se actualizó los datos excepto Disponible, ya que el inmueble tiene un contrato activo.";
                    }
                }
                else
                {
                    // No se encontro el inmueble original en la BD
                    return View("EditarInmueble", inmueble);
                }
            }
            else
            {
                // No hay contratos asociados, permitir la modificación de todos los campos
                repositorio.ActualizarInmueble(inmueble);
            }

            return RedirectToAction(nameof(ListadoTodosInmuebles));
        }

        return View("EditarInmueble", inmueble);
    }










}
