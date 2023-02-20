
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.NetworkInformation;

namespace Craftmanship.Core.Models
{
    public class ShoppingCarts
    {
        public ShoppingCarts()
        {
            Count = 1;
        }

        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public int ProductId { get; set; }
        
        [ForeignKey("ProductId")]
        public Products Product { get; set; }

        [Range(1, 10, ErrorMessage = "Ange ett antal, 1-10")]
        public int Count { get; set; }

        [NotMapped]
        public int Price { get; set; } 
       
    }
}
