using Craftmanship.Core.Interfaces;
using Craftmanship.Core.Models;
using Craftmanship.Core.Models.ViewModel;
using Craftmanship.Core.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System.Security.Claims;

namespace Craftmanship.MVC.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        //[BindProperty]
        //public ShoppingCartVM ShoppingCartVM { get; set; }
        public int OrderTotal { get; set; }
        public ShoppingCartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartsVM shoppingCartVM = new ShoppingCartsVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value,
                includeProperties: "Product"),
                Orders = new()
            };
            foreach (var shoppingCart in shoppingCartVM.ShoppingCartList)
            {
                shoppingCart.Price = shoppingCart.Product.Price;
                shoppingCartVM.Orders.OrderTotal += (shoppingCart.Price * shoppingCart.Count);
            }
            return View(shoppingCartVM);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartsVM shoppingCartVM = new ShoppingCartsVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value,
                includeProperties: "Product"),
                Orders = new()
            };

            shoppingCartVM.Orders.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefalut(x => x.Id == claim.Value);
            
            shoppingCartVM.Orders.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefalut(x => x.Id == claim.Value);
            shoppingCartVM.Orders.Name = shoppingCartVM.Orders.ApplicationUser.Name;
            shoppingCartVM.Orders.Address = shoppingCartVM.Orders.ApplicationUser.Address;
            shoppingCartVM.Orders.City = shoppingCartVM.Orders.ApplicationUser.City;
            shoppingCartVM.Orders.ZipCode = shoppingCartVM.Orders.ApplicationUser.ZipCode;
            shoppingCartVM.Orders.PhoneNumber = shoppingCartVM.Orders.ApplicationUser.PhoneNumber;


            foreach (var shoppingCart in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.Orders.OrderTotal += (shoppingCart.Product.Price * shoppingCart.Count);
            }
            return View(shoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SummaryPOST(ShoppingCartsVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == claim.Value, includeProperties: "Product");
            
            shoppingCartVM.Orders.PaymentStatus = StaticDetails.PaymentStatusPending;
            shoppingCartVM.Orders.OrderStatus = StaticDetails.StatusPending;
            shoppingCartVM.Orders.OrderDate = DateTime.Now;
            shoppingCartVM.Orders.ApplicationUserId = claim.Value;

            foreach (var shoppingCart in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.Orders.OrderTotal += (shoppingCart.Product.Price * shoppingCart.Count);    
            }

            await _unitOfWork.Orders.AddAsync(shoppingCartVM.Orders);
            await _unitOfWork.Save();

            foreach (var shoppingCart in shoppingCartVM.ShoppingCartList)
            {
                OrderDetails orderDetails = new()
                {
                    ProductId = shoppingCart.ProductId,
                    OrderId = shoppingCartVM.Orders.Id,
                    Price = shoppingCart.Product.Price, 
                    Count = shoppingCart.Count
                };
                await _unitOfWork.OrderDetails.AddAsync(orderDetails);
                await _unitOfWork.Save();
            }

            //Stripe
            var domain = "https://localhost:44351/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },

                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain+$"Customer/ShoppingCart/OrderConfirmation?id={shoppingCartVM.Orders.Id}",
				CancelUrl = domain+$"Customer/ShoppingCart/Index",
			};

            foreach(var products in shoppingCartVM.ShoppingCartList)
            {
                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (products.Product.Price * 100),
                        Currency = "SEK",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = products.Product.Name
                        },
                    },
                    Quantity = products.Count,
                };
                options.LineItems.Add(sessionLineItem);
            }

			var service = new SessionService();
			Session session = service.Create(options);

            //shoppingCartVM.Orders.SessionId = session.Id; 
            //shoppingCartVM.Orders.PaymentIntentId = session.PaymentIntentId;
           
            _unitOfWork.Orders.UpdateStripePaymentId(shoppingCartVM.Orders.Id, session.Id, session.PaymentIntentId);
            await _unitOfWork.Save();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);
            
			//_unitOfWork.ShoppingCart.RemoveRange(shoppingCartVM.ShoppingCartList);
            //await _unitOfWork.Save();
            //return RedirectToAction("Index", "Home");
        }

        public IActionResult OrderConfirmation(int id)
        {
            Orders orders = _unitOfWork.Orders.GetFirstOrDefalut(x => x.Id == id);
			var service = new SessionService();
            Session session = service.Get(orders.SessionId);
            if(session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.Orders.UpdateStripePaymentId(id, orders.SessionId, session.PaymentIntentId);
                _unitOfWork.Orders.UpdateOrderStatus(id, StaticDetails.StatusPending, StaticDetails.PaymentStatusApproved);
                _unitOfWork.Save();
            }

            List<ShoppingCarts> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == orders.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);
        }


		public async Task<IActionResult> Increment(int shoppingCartId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.GetFirstOrDefalut(x => x.Id == shoppingCartId);
            _unitOfWork.ShoppingCart.IncrementCount(shoppingCart, 1);
            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Decrement(int shoppingCartId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.GetFirstOrDefalut(x => x.Id == shoppingCartId);
            if (shoppingCart.Count <= 1)
            {
                _unitOfWork.ShoppingCart.Remove(shoppingCart);
            }
            else
            {
                _unitOfWork.ShoppingCart.DecrementCount(shoppingCart, 1);
            }
            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Remove(int shoppingCartId)
        {
            var shoppingCart = _unitOfWork.ShoppingCart.GetFirstOrDefalut(x => x.Id == shoppingCartId);
            _unitOfWork.ShoppingCart.Remove(shoppingCart);
            await _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        //private int CalculatePrice(int price, int count)
        //{
        //    var total = price * count;
        //    return total;
        //}
    }
}
