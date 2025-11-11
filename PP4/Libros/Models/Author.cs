using System.ComponentModel.DataAnnotations;

namespace Libros.Models;

public class Author
{
    public int AuthorId { get; set; }

    [Required]
    public string AuthorName { get; set; } = null!;

    public ICollection<Title> Titles { get; set; } = new List<Title>();
}