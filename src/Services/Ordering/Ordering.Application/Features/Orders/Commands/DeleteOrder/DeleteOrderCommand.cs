using MediatR;

namespace Ordering.Application.Features.Orders.Commands.DeleteOrder
{
    public class DeleteOrderCommand : IRequest<Unit>
    {
        public int Id { get; set; }

        public DeleteOrderCommand()
        {
            
        }

        public DeleteOrderCommand(int id)
        {
            Id = id;
        }
    }
}
