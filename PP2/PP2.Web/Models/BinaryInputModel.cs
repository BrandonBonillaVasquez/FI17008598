using System.ComponentModel.DataAnnotations;

namespace PP2.Web.Models
{
    public sealed class BinaryInputModel
    {
        [Display(Name = "a")]
        [BinaryString]
        public string A { get; set; } = string.Empty;

        [Display(Name = "b")]
        [BinaryString]
        public string B { get; set; } = string.Empty;
    }
}