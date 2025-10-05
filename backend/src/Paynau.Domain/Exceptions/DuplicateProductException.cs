
namespace Paynau.Domain.Exceptions;

public class DuplicateProductException : DomainException
{
    public DuplicateProductException(string productName) 
        : base($"A product with name '{productName}' already exists") { }
}

