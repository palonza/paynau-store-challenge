
namespace Paynau.Domain.Interfaces;

using Paynau.Domain.Entities;

public interface IOrderDomainService
{
    Order CreateOrder(Product product, int quantity);
}

