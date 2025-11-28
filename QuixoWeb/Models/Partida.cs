using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuixoWeb.Models
{
    public class Partida
    {
        [Key]
        public int PartidaId { get; set; }

        [Required]
        public byte ModoJuego { get; set; } // 2 o 4 jugadores

        [Required]
        public DateTime FechaHoraCreacion { get; set; } = DateTime.Now;

        [Required]
        public TimeSpan TiempoTranscurrido { get; set; } = TimeSpan.Zero;

        [Required]
        [MaxLength(50)]
        public string Estado { get; set; } = "EnCurso"; // "EnCurso" o "Finalizada"

        public int? GanadorId { get; set; } // 1 o 2 para modo 2 jugadores

        [MaxLength(1)]
        public string? EquipoGanador { get; set; } // "A" o "B" para modo 4 jugadores

        // Navegación
        public virtual ICollection<Jugada> Jugadas { get; set; } = new List<Jugada>();
    }
}