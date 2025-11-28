using QuixoWeb.Models;
using QuixoWeb.Models.ViewModels;

namespace QuixoWeb.Services
{
    public interface IGameService
    {
        // Gestión de partidas
        Task<Partida> CrearNuevaPartidaAsync(byte modoJuego);
        Task<GameViewModel> ObtenerEstadoPartidaAsync(int partidaId);
        Task<bool> RealizarJugadaAsync(int partidaId, int fila, int columna, int filaDestino, int columnaDestino, byte? orientacionPunto = null);
        Task<bool> ReiniciarPartidaAsync(int partidaId);

        // Validaciones
        bool EsMovimientoValido(GameViewModel juego, int fila, int columna, int filaDestino, int columnaDestino);
        bool EsCuboJugable(GameViewModel juego, int fila, int columna);

        // Verificación de victoria
        Task<string?> VerificarVictoriaAsync(GameViewModel juego);

        // Historial
        Task<List<Partida>> ObtenerPartidasFinalizadasAsync();
        Task<List<Jugada>> ObtenerHistorialPartidaAsync(int partidaId);

        // Exportación
        Task<string> ExportarPartidaXmlAsync(int partidaId);

        // Estadísticas
        Task<StatsViewModel> ObtenerEstadisticasAsync();
        Task ActualizarEstadisticasAsync(byte modoJuego, string ganador);

        Task<bool> ActualizarTiempoPartidaAsync(int partidaId, TimeSpan tiempoTranscurrido);
    }
}