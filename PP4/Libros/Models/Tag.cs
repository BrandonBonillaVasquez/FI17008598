using System.ComponentModel.DataAnnotations;

namespace Libros.Models;

public class Tag
{
    public int TagId { get; set; }

    [Required]
    public string TagName { get; set; } = null!;

    public ICollection<TitleTag> TitlesTags { get; set; } = new List<TitleTag>();
}