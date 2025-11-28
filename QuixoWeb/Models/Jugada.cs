using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuixoWeb.Models
{
    public class Jugada
    {
        [Key]
        public int JugadaId { get; set; }

        [Required]
        public int PartidaId { get; set; }

        [Required]
        public int NumeroJugada { get; set; }

        [Required]
        public int JugadorActual { get; set; } // 1, 2, 3 o 4

        [Required]
        public int FilaOrigen { get; set; } // 0-4

        [Required]
        public int ColumnaOrigen { get; set; } // 0-4

        [Required]
        public int FilaDestino { get; set; } // 0-4

        [Required]
        public int ColumnaDestino { get; set; } // 0-4

        public byte? OrientacionPunto { get; set; } // 0=arriba, 1=derecha, 2=abajo, 3=izquierda

        [Required]
        public string EstadoTablero { get; set; } = "[]"; // JSON del tablero

        [Required]
        public TimeSpan TiempoTranscurrido { get; set; }

        // Navegación
        [ForeignKey("PartidaId")]
        public virtual Partida? Partida { get; set; }
    }
}