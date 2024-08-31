using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIntegration.DbContext;

public class OrderDetail
{
    [Required]
    [Comment("訂單明細編號")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [Comment("產品")]
    public Product Product { get; set; }
    
    [Required]
    [Comment("訂購價格")]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal OrderPrice { get; set; }
    
    [Required]
    [Comment("數量")]
    public int OrderQuantity { get; set; }
    
}
