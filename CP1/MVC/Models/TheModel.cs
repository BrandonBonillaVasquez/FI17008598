
using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
    public class TheModel
    {
        [Required(ErrorMessage = "El valor es requerido.")]
        [StringLength(25, MinimumLength = 5, ErrorMessage = "La longitud debe ser de 5 a 25 caracteres.")]
        public string? Phrase { get; set; }   // ← anulable: lo llenará el model binding

        public Dictionary<char, int> Counts { get; set; } = new(); // inicializado

        public string? Lower { get; set; }    // ← anulable
        public string? Upper { get; set; }    // ← anulable
    }
}