namespace QuixoWeb.Models.ViewModels
{
    public class StatsViewModel
    {
        public List<EstadisticaDto> EstadisticasDosJugadores { get; set; } = new();
        public List<EstadisticaDto> EstadisticasCuatroJugadores { get; set; } = new();

        // Resumen general
        public int TotalPartidas { get; set; }
        public int PartidasDosJugadores { get; set; }
        public int PartidasCuatroJugadores { get; set; }
        public TimeSpan TiempoPromedioTotal { get; set; }
        public TimeSpan TiempoPromedioDosJugadores { get; set; }
        public TimeSpan TiempoPromedioCuatroJugadores { get; set; }
    }

    public class EstadisticaDto
    {
        public string Nombre { get; set; } = string.Empty;
        public decimal Efectividad { get; set; }
        public int Ganadas { get; set; }
    }
}