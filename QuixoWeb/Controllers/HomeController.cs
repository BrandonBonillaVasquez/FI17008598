using Microsoft.AspNetCore.Mvc;
using QuixoWeb.Data;
using QuixoWeb.Models;
using System.Diagnostics;

namespace QuixoWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly QuixoDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(QuixoDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Home/Index
        public IActionResult Index()
        {
            try
            {
                // Verificar conexión a la base de datos
                var canConnect = _context.Database.CanConnect();
                ViewBag.DbStatus = canConnect ? "Conectado" : "Desconectado";

                if (!canConnect)
                {
                    _logger.LogWarning("No se pudo conectar a la base de datos");
                    TempData["Warning"] = "Advertencia: No hay conexión con la base de datos.";
                }

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la página principal");
                ViewBag.DbStatus = "Error";
                return View();
            }
        }

        // GET: Home/About
        public IActionResult About()
        {
            return View();
        }

        // GET: Home/Privacy
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}