
using System.ComponentModel.DataAnnotations;

namespace Libros.Models;

public class TitleTag
{
    public int TitleTagId { get; set; }

    [Required]
    public int TitleId { get; set; }

    [Required]
    public int TagId { get; set; }

    public Title Title { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
