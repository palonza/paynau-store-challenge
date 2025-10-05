
namespace Paynau.Domain.Exceptions;

public class ProductInUseException : DomainException
{
    public ProductInUseException(int productId) 
        : base($"Product with ID {productId} cannot be deleted because it has associated orders") { }
}

