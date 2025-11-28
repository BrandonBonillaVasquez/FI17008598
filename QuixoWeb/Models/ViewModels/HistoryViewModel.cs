using QuixoWeb.Models;

namespace QuixoWeb.Models.ViewModels
{
    public class HistoryViewModel
    {
        public GameViewModel Partida { get; set; }
        public List<Jugada> Jugadas { get; set; } = new List<Jugada>();
        public int JugadaActual { get; set; } = 0;
        public int TotalJugadas => Jugadas.Count;
    }
}