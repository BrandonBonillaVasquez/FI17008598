using System.ComponentModel.DataAnnotations;

namespace Libros.Models;

public class Title
{
    public int TitleId { get; set; }

    [Required]
    public int AuthorId { get; set; }

    [Required]
    public string TitleName { get; set; } = null!;

    public Author Author { get; set; } = null!;
    public ICollection<TitleTag> TitlesTags { get; set; } = new List<TitleTag>();
}