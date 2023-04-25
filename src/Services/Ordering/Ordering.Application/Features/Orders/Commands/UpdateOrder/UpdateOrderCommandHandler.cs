using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Persistents;
using Ordering.Application.Exceptions;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CheckoutOrderCommandHandler> _logger;

        public UpdateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, ILogger<CheckoutOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToupdate = await _orderRepository.GetByIdAsync(request.Id);
            if (orderToupdate == null)
            {
                _logger.LogInformation("Order not exist on database");
                throw new NotFoundException(nameof(Order), request.Id);
            }

           _mapper.Map(request, orderToupdate);
            await _orderRepository.UpdateAsync(orderToupdate);
            _logger.LogInformation($"Order {orderToupdate.Id} is successfully updated.");

            return Unit.Value;
        }
    }
}
