namespace AspNetCoreIntegration.Models.Request;

public class AddOrderRequest
{
    public Guid ProductId { get; set; }
    
    public int Quantity { get; set; }
}