namespace QuixoWeb.Models.ViewModels
{
    public class GameViewModel
    {
        public int PartidaId { get; set; }
        public byte ModoJuego { get; set; }
        public string[,] Tablero { get; set; } = new string[5, 5];
        public byte?[,] OrientacionesPuntos { get; set; } = new byte?[5, 5]; // Solo para 4 jugadores
        public int JugadorActual { get; set; }
        public TimeSpan TiempoTranscurrido { get; set; }
        public string Estado { get; set; } = "EnCurso";
        public string? Ganador { get; set; }
        public bool EsPrimeraVuelta { get; set; } = true;
    }
}