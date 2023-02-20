using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.Extensions.Primitives;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Craftmanship.Core.Models
{
    public class Products
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Namn")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Display(Name = "Bild")]
        [ValidateNever]
        public string ImageUrl { get; set; }

        [Required]
        [Display(Name = "Pris")]
        public int Price { get; set; }

        //public int VendorId { get; set; }

        //[ForeignKey("VendorId")]
        //[DisplayName("Försäljare")]
        //[ValidateNever]
        //public Vendor Vendor { get; set; }

        [Display(Name = "Kategori")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Categories Category { get; set; }
    }
}
