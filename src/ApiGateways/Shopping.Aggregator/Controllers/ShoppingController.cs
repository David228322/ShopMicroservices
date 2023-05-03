using Microsoft.AspNetCore.Mvc;
using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services.Interfaces;
using System.Net;

namespace Shopping.Aggregator.Controllers
{
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        private readonly ICatalogService _catalogService;
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;

        public ShoppingController(
            ICatalogService catalogService, 
            IBasketService basketService, 
            IOrderService orderService)
        {
            _catalogService = catalogService;
            _basketService = basketService;
            _orderService = orderService;
        }

        [HttpGet("{userName}", Name = "GetShopping")]
        [ProducesResponseType(typeof(ShoppingModel), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingModel>> GetShopping(string userName)
        {
            // get basket with username
            // iterate basket items and consume products with basket item productid member
            // map product related members into basketitem dto with extended columns
            // consume ordering microservices in order to retrive order list
            // return root ShoppingModle dto class which includinig all responses

            var basket = await _basketService.GetBasket(userName);
            foreach (var item in basket.Items)
            {
                var product = await _catalogService.GetCatalog(item.ProductId);
                // set additional fields onto basket item
                item.ProductName = product.Name;
                item.Category = product.Category;
                item.Price = product.Price;
                item.Description = product.Description;
                item.ImageFile = product.ImageFile;
                item.Summary = product.Summary;
            }

            var orders = await _orderService.GetOrdersByUserName(userName);

            var shoppingModel = new ShoppingModel
            {
                UserName = userName,
                BasketWithProducts = basket,
                Orders = orders
            };

            return Ok(shoppingModel);
        } 
    }
}
