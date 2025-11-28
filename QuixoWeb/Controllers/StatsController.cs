using Microsoft.AspNetCore.Mvc;
using QuixoWeb.Services;

namespace QuixoWeb.Controllers
{
    public class StatsController : Controller
    {
        private readonly IGameService _gameService;
        private readonly ILogger<StatsController> _logger;

        public StatsController(IGameService gameService, ILogger<StatsController> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        // GET: Stats/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var estadisticas = await _gameService.ObtenerEstadisticasAsync();
                return View(estadisticas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas");
                TempData["Error"] = "No se pudieron cargar las estadísticas.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Stats/GetStats (API para AJAX)
        [HttpGet]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var estadisticas = await _gameService.ObtenerEstadisticasAsync();

                return Json(new
                {
                    success = true,
                    dosJugadores = estadisticas.EstadisticasDosJugadores,
                    cuatroJugadores = estadisticas.EstadisticasCuatroJugadores
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas via API");
                return Json(new { success = false, message = "Error al cargar estadísticas" });
            }
        }
    }
}