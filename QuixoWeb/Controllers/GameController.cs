using Microsoft.AspNetCore.Mvc;
using QuixoWeb.Models.ViewModels;
using QuixoWeb.Services;
using System.Text.Json;
using QuixoWeb.Data;
using QuixoWeb.Models;

namespace QuixoWeb.Controllers
{
    public class GameController : Controller
    {
        private readonly IGameService _gameService;
        private readonly ILogger<GameController> _logger;
        private readonly QuixoDbContext _context;

        public GameController(IGameService gameService, ILogger<GameController> logger, QuixoDbContext context)
        {
            _gameService = gameService;
            _logger = logger;
            _context = context;
        }

        // GET: Game/SelectMode
        [HttpGet]
        public IActionResult SelectMode()
        {
            return View();
        }
        // POST: Game/NewGame
        [HttpPost]
        public async Task<IActionResult> NewGame(byte modoJuego)
        {
            try
            {
                if (modoJuego != 2 && modoJuego != 4)
                {
                    TempData["Error"] = "Modo de juego inválido. Debe ser 2 o 4 jugadores.";
                    return RedirectToAction(nameof(SelectMode));
                }

                _logger.LogInformation($"Intentando crear partida con modo {modoJuego}");

                var partida = await _gameService.CrearNuevaPartidaAsync(modoJuego);

                // Guardar ID de partida en sesión
                HttpContext.Session.SetInt32("PartidaActual", partida.PartidaId);

                _logger.LogInformation($"Nueva partida iniciada: ID {partida.PartidaId}, Modo {modoJuego}");

                return RedirectToAction(nameof(Play), new { id = partida.PartidaId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error detallado al crear nueva partida");
                TempData["Error"] = $"Ocurrió un error al crear la partida: {ex.Message}";
                return RedirectToAction(nameof(SelectMode));
            }
        }

        // GET: Game/Play/5
        [HttpGet]
        public async Task<IActionResult> Play(int id)
        {
            try
            {
                var gameState = await _gameService.ObtenerEstadoPartidaAsync(id);

                // Guardar en sesión por si se pierde
                HttpContext.Session.SetInt32("PartidaActual", id);

                return View(gameState);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cargar partida {id}");
                TempData["Error"] = "No se pudo cargar la partida.";
                return RedirectToAction("Index", "Home");
            }
        }

        // POST: Game/MakeMove
        [HttpPost]
        public async Task<IActionResult> MakeMove([FromBody] MoveRequest request)
        {
            try
            {
                var success = await _gameService.RealizarJugadaAsync(
                    request.PartidaId,
                    request.FilaOrigen,
                    request.ColumnaOrigen,
                    request.FilaDestino,
                    request.ColumnaDestino,
                    request.OrientacionPunto
                );

                if (!success)
                {
                    return Json(new { success = false, message = "Movimiento inválido" });
                }

                // Obtener estado actualizado
                var gameState = await _gameService.ObtenerEstadoPartidaAsync(request.PartidaId);

                // Convertir tablero a formato serializable para JSON
                var tableroLista = new List<List<string>>();
                var orientacionesLista = new List<List<int?>>();

                for (int i = 0; i < 5; i++)
                {
                    var filaSimbolos = new List<string>();
                    var filaOrientaciones = new List<int?>();

                    for (int j = 0; j < 5; j++)
                    {
                        filaSimbolos.Add(gameState.Tablero[i, j]);
                        filaOrientaciones.Add(gameState.OrientacionesPuntos[i, j]);
                    }

                    tableroLista.Add(filaSimbolos);
                    orientacionesLista.Add(filaOrientaciones);
                }

                return Json(new
                {
                    success = true,
                    gameState = new
                    {
                        tablero = tableroLista,
                        orientaciones = orientacionesLista,
                        jugadorActual = gameState.JugadorActual,
                        estado = gameState.Estado,
                        ganador = gameState.Ganador,
                        esPrimeraVuelta = gameState.EsPrimeraVuelta
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al realizar jugada");
                return Json(new { success = false, message = $"Error al procesar el movimiento: {ex.Message}" });
            }
        }

        // POST: Game/UpdateTime
        [HttpPost]
        public async Task<IActionResult> UpdateTime([FromBody] TimeUpdateRequest request)
        {
            try
            {
                // Buscar la partida
                var partida = await _context.Partidas.FindAsync(request.PartidaId);

                if (partida == null)
                {
                    return Json(new { success = false, message = "Partida no encontrada" });
                }

                // Convertir el string de tiempo a TimeSpan
                if (TimeSpan.TryParse(request.TiempoTranscurrido, out TimeSpan tiempo))
                {
                    partida.TiempoTranscurrido = tiempo;
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Tiempo actualizado para partida {request.PartidaId}: {tiempo}");

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Formato de tiempo inválido" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar tiempo");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Game/Restart
        [HttpPost]
        public async Task<IActionResult> Restart(int id)
        {
            try
            {
                var success = await _gameService.ReiniciarPartidaAsync(id);

                if (!success)
                {
                    TempData["Error"] = "No se pudo reiniciar la partida.";
                    return RedirectToAction(nameof(Play), new { id });
                }

                TempData["Success"] = "Partida reiniciada exitosamente.";
                return RedirectToAction(nameof(Play), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al reiniciar partida {id}");
                TempData["Error"] = "Ocurrió un error al reiniciar la partida.";
                return RedirectToAction(nameof(Play), new { id });
            }
        }

        // GET: Game/ValidateMove
        [HttpGet]
        public async Task<IActionResult> ValidateMove(int partidaId, int fila, int columna, int filaDestino, int columnaDestino)
        {
            try
            {
                var gameState = await _gameService.ObtenerEstadoPartidaAsync(partidaId);

                var esJugable = _gameService.EsCuboJugable(gameState, fila, columna);
                var esValido = _gameService.EsMovimientoValido(gameState, fila, columna, filaDestino, columnaDestino);

                return Json(new
                {
                    esJugable,
                    esValido,
                    mensaje = !esJugable ? "Este cubo no se puede jugar" :
                              !esValido ? "Movimiento no válido" :
                              "Movimiento válido"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar movimiento");
                return Json(new { esJugable = false, esValido = false, mensaje = "Error al validar" });
            }
        }

        // GET: Game/History
        [HttpGet]
        public async Task<IActionResult> History()
        {
            try
            {
                var partidas = await _gameService.ObtenerPartidasFinalizadasAsync();
                return View(partidas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener historial");
                TempData["Error"] = "No se pudo cargar el historial.";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Game/ViewHistory/5
        [HttpGet]
        public async Task<IActionResult> ViewHistory(int id)
        {
            try
            {
                var jugadas = await _gameService.ObtenerHistorialPartidaAsync(id);
                var partida = await _gameService.ObtenerEstadoPartidaAsync(id);

                var viewModel = new HistoryViewModel
                {
                    Partida = partida,
                    Jugadas = jugadas
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al ver historial de partida {id}");
                TempData["Error"] = "No se pudo cargar el historial de la partida.";
                return RedirectToAction(nameof(History));
            }
        }
        // GET: Game/GetHistoryMove
        [HttpGet]
        public async Task<IActionResult> GetHistoryMove(int partidaId, int numeroJugada)
        {
            try
            {
                var jugadas = await _gameService.ObtenerHistorialPartidaAsync(partidaId);
                var jugada = jugadas.FirstOrDefault(j => j.NumeroJugada == numeroJugada);

                if (jugada == null)
                {
                    return Json(new { success = false, message = "Jugada no encontrada" });
                }

                // Deserializar como TableroDataSerializable
                var tableroSerializable = JsonSerializer.Deserialize<TableroDataSerializable>(jugada.EstadoTablero);

                // NO convertir a TableroData, enviar directamente las listas
                return Json(new
                {
                    success = true,
                    jugada = new
                    {
                        numeroJugada = jugada.NumeroJugada,
                        jugadorActual = jugada.JugadorActual,
                        tablero = tableroSerializable.Simbolos,  
                        orientaciones = tableroSerializable.Orientaciones,  
                        tiempoTranscurrido = jugada.TiempoTranscurrido.ToString(@"hh\:mm\:ss")
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener jugada");
                return Json(new { success = false, message = "Error al cargar jugada" });
            }
        }

        // GET: Game/Export/5
        [HttpGet]
        public async Task<IActionResult> Export(int id)
        {
            try
            {
                var xml = await _gameService.ExportarPartidaXmlAsync(id);
                var fileName = $"Partida_{id}_{DateTime.Now:yyyyMMdd_HHmmss}.xml";

                return File(
                    System.Text.Encoding.UTF8.GetBytes(xml),
                    "application/xml",
                    fileName
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al exportar partida {id}");
                TempData["Error"] = "No se pudo exportar la partida.";
                return RedirectToAction(nameof(History));
            }
        }
    }

    // Clases para requests
    public class MoveRequest
    {
        public int PartidaId { get; set; }
        public int FilaOrigen { get; set; }
        public int ColumnaOrigen { get; set; }
        public int FilaDestino { get; set; }
        public int ColumnaDestino { get; set; }
        public byte? OrientacionPunto { get; set; }
    }

    public class TimeUpdateRequest
    {
        public int PartidaId { get; set; }
        public string TiempoTranscurrido { get; set; }
    }
}