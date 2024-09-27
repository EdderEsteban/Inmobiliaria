using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Inmobiliaria.Models;
using Inmobiliaria.Repositorios;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;

namespace Inmobiliaria.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly ILogger<UsuariosController> _logger;
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;

        private readonly RepositorioUsuarios? repositorio;

        public UsuariosController(
            ILogger<UsuariosController> logger,
            IConfiguration configuration,
            IWebHostEnvironment environment
        )
        {
            _logger = logger;
            repositorio = new RepositorioUsuarios();
            this.configuration = configuration;
            this.environment = environment;
        }

        // Método para listar los usuarios
        [HttpGet]
        public IActionResult ListadoUsuarios()
        {
            var lista = repositorio.ListarUsuarios();
            return View(lista);
        }

        // Método para obtener los detalles de un usuario por ID
        [HttpGet]
        public IActionResult DetalleUsuario(int id)
        {
            var usuario = repositorio.ObtenerUsuarioPorId(id);
            if (usuario == null)
            {
                TempData["ErrorMessage"] = "El usuario no existe. Intente nuevamente.";
            }
            return View(usuario);
        }

        // Método para la vista crear un nuevo usuario
        [HttpGet]
        public IActionResult CrearUsuario()
        {
            return View();
        }

        // Método para crear un nuevo usuario
        [HttpPost]
        public IActionResult CrearUsuario(Usuario usuario)
        {
            var existe = repositorio.ExisteUsuario(usuario.Email);

            // Verificar si existe el usuario
            if (existe)
            {
                TempData["ErrorMessage"] =
                    "Ocurrió un error al crear el usuario. El Correo ya esta registrado.";
                return View(usuario);
            }
            // Verifica si el modelo es válido
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Modelo inválido");
                TempData["ErrorMessage"] =
                    "Ocurrió un error al crear el usuario. Intente nuevamente.";
                return View(usuario);
            }

            try
            {
                // Hashea la contraseña usando un algoritmo seguro
                string hashed = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: usuario.Password,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8
                    )
                );
                
                // Asigna la contraseña hasheada al usuario
                usuario.Password = hashed;

                // Genera un nuevo nombre aleatorio para el avatar (opcional)
                var nbreRnd = Guid.NewGuid(); // Puedes usar este nombre si necesitas aleatoriedad

                // Guarda el nuevo usuario en la base de datos
                int res = repositorio.CrearUsuario(usuario);

                
                // Verifica si se ha subido un archivo de avatar 
                if (usuario.AvatarFile != null )
                {
                    // Obtiene la ruta donde se guardará el avatar
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");

                    // Crea el directorio si no existe
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    // Genera un nombre de archivo único para el avatar usando el ID del usuario
                    string fileName =
                        "avatar_"
                        + res
                        + usuario.Nombre
                        + Path.GetExtension(usuario.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    usuario.Avatar = Path.Combine("/Uploads", fileName); // Guarda la ruta relativa del avatar

                    // Guarda el archivo del avatar en la ruta especificada
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        usuario.AvatarFile.CopyTo(stream);
                    }

                    // Actualiza la información del usuario con la ruta del avatar en la base de datos
                    ModificarUser(usuario);
                }

                // Redirige a la acción Index después de crear el usuario
                return RedirectToAction(nameof(ListadoUsuarios));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear el usuario: {ex.Message}");
                TempData["ErrorMessage"] =
                    "Ocurrió un error al crear el usuario. Intente nuevamente.";
                return View(usuario);
            }
        }

        // Método para editar un usuario
        [HttpGet]
        public IActionResult EditarUsuario(int id)
        {
            var usuario = repositorio.ObtenerUsuarioPorId(id);
            return View(usuario);
        }

        // Método para Modificar un usuario
        [HttpPost]
        public IActionResult ModificarUser(Usuario user)
        {
            Console.WriteLine($"El modelo es valido: {ModelState.IsValid}");
            if (ModelState.IsValid)
            {

                int resultado = repositorio.ActualizarUsuario(user);

                if (resultado > 0)
                {
                    return RedirectToAction("ListadoUsuarios");
                }
                else
                {
                    TempData["ErrorMessage"] =
                        "Ocurrió un error al actualizar el usuario. Intente nuevamente.";
                }
            }
            return View("EditarUsuario", user);
        }

        // Método para eliminar un usuario
        public IActionResult EliminarUsuario(int id)
        {
            try
            {
                repositorio.EliminarUsuario(id);
                TempData["SuccessMessage"] = "Usuario eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar el usuario.";
            }

            return RedirectToAction("ListadoUsuarios");
        }

        // Acceso a LoginIn
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }    

        // Método para procesar el formulario de Login
        [HttpPost]
        public IActionResult LoginIn(Login usuario)
        {
            Console.WriteLine($"El modelo es valido: {ModelState.IsValid}");
            // Validar si el modelo es correcto
            if (ModelState.IsValid)
            {
                // Verificar las credenciales del usuario
                Usuario? user = repositorio.BuscarLogin(usuario.Email, usuario.Password);

                if (user != null)
                {
                    // Credenciales correctas, iniciamos sesión
                    HttpContext.Session.SetString("IdUsuario", user.Id_usuario.ToString());
                    HttpContext.Session.SetString("Nombre", user.Nombre);
                    HttpContext.Session.SetString("Rol", user.Rol.ToString());

                    

                    // Redireccionar a la página principal del sistema
                    return RedirectToAction("Inicio", "Usuario");
                }
                else
                {
                    // Credenciales incorrectas, mostrar mensaje de error
                    TempData["ErrorMessage"] = "Correo o contraseña incorrectos.";
                }
            }

            // Si algo falla, volvemos a mostrar el formulario con los errores
            return View("Index", usuario);
        }

        // Método para cerrar sesión
        public IActionResult Logout()
        {
            // Limpiar la sesión
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    

    }
}
