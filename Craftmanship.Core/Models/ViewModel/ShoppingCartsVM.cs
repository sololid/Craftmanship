namespace Craftmanship.Core.Models.ViewModel
{
	public class ShoppingCartsVM
	{
		public IEnumerable<ShoppingCarts> ShoppingCartList { get; set; }
		public Orders Orders { get; set; }
	}
}
