namespace Craftmanship.Core.Models.ViewModel
{
	public class OrdersVM
	{
		public Orders Orders { get; set; }

		public IEnumerable<OrderDetails> OrderDetails { get; set; }
	}
}
