namespace AspNetCoreIntegrationTest.Models.Response;

public class GetProductResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }
 
    public int Quantity { get; set; }
}