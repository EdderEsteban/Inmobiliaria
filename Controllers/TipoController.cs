using Inmobiliaria.Models;
using Inmobiliaria.Repositorios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers
{
    [Authorize]
    public class TipoController : Controller
    {
        private readonly ILogger<TipoController> _logger;
        private readonly RepositorioTipo repositorio;

        public TipoController(ILogger<TipoController> logger)
        {
            _logger = logger;
            repositorio = new RepositorioTipo();
        }






        
    }
}
