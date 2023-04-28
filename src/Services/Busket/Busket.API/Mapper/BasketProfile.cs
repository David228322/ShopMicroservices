using AutoMapper;
using Busket.API.Entities;
using EventBus.Messages.Events;

namespace Busket.API.Mapper
{
    public class BasketProfile : Profile
    {
        public BasketProfile() 
        {
            CreateMap<BasketCheckout, BasketCheckoutEvent>().ReverseMap();
        }
    }
}
