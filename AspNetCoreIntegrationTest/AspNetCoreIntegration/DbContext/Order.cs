using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIntegration.DbContext;

public class Order
{
    [Key]
    [Required]
    [Comment("訂單編號")]
    public Guid Id { get; set; }
    
    [Comment("訂購人")]
    public User OrderUser { get; set; }
    
    [Comment("訂購明細")]
    public List<OrderDetail> OrderDetails { get; set; }
    
    [Comment("建立時間")]
    public DateTime Created { get; set; }
    
    [Comment("更新時間")]
    public DateTime Updated { get; set; }
}