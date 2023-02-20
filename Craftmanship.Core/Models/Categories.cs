using System.ComponentModel.DataAnnotations;

namespace Craftmanship.Core.Models
{
    public class Categories
    {
        public int Id { get; set; }

        [Display(Name = "Kategori")]
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
    }
}
