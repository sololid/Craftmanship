using Craftmanship.Core.Interfaces;
using Craftmanship.Core.Models;
using Craftmanship.Core.Models.ViewModel;
using Craftmanship.Core.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Diagnostics;
using System.Security.Claims;

namespace Craftmanship.MVC.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize]
	public class OrderController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		[BindProperty]
		public OrdersVM OrdersVM { get; set; }
		public OrderController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult Index()
		{
			return View();
		}
		
		public IActionResult Details(int orderId)
		{
			OrdersVM = new OrdersVM()
			{
				Orders = _unitOfWork.Orders.GetFirstOrDefalut(x => x.Id == orderId, includeProperties: "ApplicationUser"),
				OrderDetails = _unitOfWork.OrderDetails.GetAll(x => x.OrderId == orderId, includeProperties: "Product")
			};
			return View(OrdersVM);
		}

		[HttpPost]
		[Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateOrderDetails()
		{
			var orderFromDb = _unitOfWork.Orders.GetFirstOrDefalut(x => x.Id == OrdersVM.Orders.Id, tracked: false);
			orderFromDb.Name = OrdersVM.Orders.Name;
			orderFromDb.PhoneNumber = OrdersVM.Orders.PhoneNumber;
			orderFromDb.Address = OrdersVM.Orders.Address;
			orderFromDb.City = OrdersVM.Orders.City;
			orderFromDb.ZipCode = OrdersVM.Orders.ZipCode;
			if (OrdersVM.Orders.Carrier != null) orderFromDb.Carrier = OrdersVM.Orders.Carrier;
			if (OrdersVM.Orders.TrackingNumber != null) orderFromDb.TrackingNumber = OrdersVM.Orders.TrackingNumber;
			_unitOfWork.Orders.Update(orderFromDb);
			_unitOfWork.Save();
			TempData["Success"] = "Orderdetaljerna är uppdaterade!";
			return RedirectToAction("Details", "Order", new {orderId = orderFromDb.Id});
		}

		[HttpPost]
		[Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult StartManufacturing()
		{
			_unitOfWork.Orders.UpdateOrderStatus(OrdersVM.Orders.Id, StaticDetails.StatusInprocess);
			_unitOfWork.Save();
			TempData["Success"] = "Tillverkningen av ordern är påbörjad.";
			return RedirectToAction("Details", "Order", new {orderId = OrdersVM.Orders.Id});
		}

		[HttpPost]
		[Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult OrderComplete()
		{
			_unitOfWork.Orders.UpdateOrderStatus(OrdersVM.Orders.Id, StaticDetails.StatusCompleted);
			_unitOfWork.Save();
			TempData["Success"] = "Ordern är redo att skickas.";
			return RedirectToAction("Details", "Order", new { orderId = OrdersVM.Orders.Id });
		}

		[HttpPost]
		[Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult ShipOrder()
		{
			var order = _unitOfWork.Orders.GetFirstOrDefalut(x => x.Id == OrdersVM.Orders.Id, tracked: false);
			order.TrackingNumber = OrdersVM.Orders.TrackingNumber;
			order.Carrier = OrdersVM.Orders.Carrier;
			order.OrderStatus = StaticDetails.StatusShipped;
			order.ShippingDate = DateTime.Now;

			_unitOfWork.Orders.Update(order);
			_unitOfWork.Save();
			TempData["Success"] = "Ordern är skickad.";
			return RedirectToAction("Details", "Order", new {orderId = OrdersVM.Orders.Id});
		}

		[HttpPost]
		[Authorize(Roles = StaticDetails.Role_Admin + "," + StaticDetails.Role_Employee)]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder()
		{
			var order = _unitOfWork.Orders.GetFirstOrDefalut(X => X.Id == OrdersVM.Orders.Id, tracked: false);
			if (order.PaymentStatus == StaticDetails.PaymentStatusApproved)
			{
				var options = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = order.PaymentIntentId
					//Stripe läser av ID och returnerar det totala priset av just den ordern som ID:t tillhör "by default"
				};

				var service = new RefundService();
				Refund refund = service.Create(options);

				_unitOfWork.Orders.UpdateOrderStatus(order.Id, StaticDetails.StatusCancelled, StaticDetails.StatusRefunded);
			}
			else
			{
				_unitOfWork.Orders.UpdateOrderStatus(order.Id, StaticDetails.StatusCancelled, StaticDetails.StatusCancelled);
			}
			_unitOfWork.Save();
			TempData["Success"] = "Ordern är makulerad!";
			return RedirectToAction("Details", "Order", new { orderId = OrdersVM.Orders.Id });
		}

		#region API CALLS
		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<Orders> orders;
			if (User.IsInRole(StaticDetails.Role_Admin)  || User.IsInRole(StaticDetails.Role_Employee)){
				orders = _unitOfWork.Orders.GetAll(includeProperties: "ApplicationUser");
			} 
			else
			{
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
				orders = _unitOfWork.Orders.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "ApplicationUser");
			}

            switch (status)
            {
				case "pending":
					orders = orders.Where(x => x.OrderStatus == StaticDetails.StatusPending);
					break;
				case "inprocess":
					orders = orders.Where(x => x.OrderStatus == StaticDetails.StatusInprocess);
					break;
				case "completed":
					orders = orders.Where(x => x.OrderStatus == StaticDetails.StatusCompleted);
					break;
				case "shipped":
					orders = orders.Where(x => x.OrderStatus == StaticDetails.StatusShipped);
					break;
				case "cancelled":
					orders = orders.Where(x => x.OrderStatus == StaticDetails.StatusCancelled);
					break;
				default:
					orders = _unitOfWork.Orders.GetAll();
					break;
			}
            return Json(new { data = orders });
		}
		#endregion
	}
}
