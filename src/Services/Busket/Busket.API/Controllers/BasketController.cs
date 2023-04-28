using AutoMapper;
using Basket.API.Entities;
using Basket.API.Repositories;
using Busket.API.Entities;
using Busket.API.GrpcServices;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly DiscountGrpcService _discountGrpcService;
        private readonly ILogger<BasketController> _logger;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public BasketController(
            IBasketRepository repository,
            DiscountGrpcService discountGrpcService,
            ILogger<BasketController> logger,
            IMapper mapper,
            IPublishEndpoint publishEndpoint)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _discountGrpcService = discountGrpcService ?? throw new ArgumentNullException();
            _logger = logger;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet("{userName}")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            var basket = await _repository.GetBasket(userName);
            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            //TODO: Communicate with Discount.Grpc and
            //Calculate latest praices of product into shopping cart
            // consume DIscount Grpc
            foreach (var item in basket.Items)
            {
                var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                item.Price -= coupon.Amount;
            }

            return Ok(await _repository.UpdateBasket(basket));
        }

        [HttpDelete("{userName}")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            await _repository.DeleteBasket(userName);
            return NoContent();
        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> Checkout([FromBody] BasketCheckout request)
        {
            // 1 get existing basket with total price
            // 2 Create basketCheckoutEvent -- Set totalPrice on basketCheckout event message
            // 3 Send checkout event rabbitmq
            // 4 remove the basket

            var basket = await _repository.GetBasket(request.UserName);
            if(basket == null)
            {
                _logger.LogError($"Busket with {request.UserName} was not found");
                return NotFound($"Busket with {request.UserName} was not found");
            }

            var eventMessage = _mapper.Map<BasketCheckoutEvent>(request);
            eventMessage.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMessage);


            await _repository.DeleteBasket(request.UserName);
            return Accepted();
        }
    }
}
