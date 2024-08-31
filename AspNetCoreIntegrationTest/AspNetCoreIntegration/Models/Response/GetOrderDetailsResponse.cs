namespace AspNetCoreIntegrationTest.Models.Response;

public class GetOrderDetailsResponse
{
    /// <summary>
    /// 訂單編號
    /// </summary>
    public Guid OrderId { get; set; }
    
    /// <summary>
    /// 訂單成立時間
    /// </summary>
    public DateTime Created { get; set; }
    
    /// <summary>
    /// 訂購者
    /// </summary>
    public string OrderName { get; set; }
    
    /// <summary>
    /// 訂購明細
    /// </summary>
    public IEnumerable<OrderDetailResult> Details { get; set; }
    
    /// <summary>
    /// 總價
    /// </summary>
    public decimal TotalPrice { get; set; }
    
    /// <summary>
    /// 總量
    /// </summary>
    public int TotalQuantity { get; set; }
}