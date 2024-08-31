using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIntegration.DbContext;

public class Product
{
    [Key]
    [Required]
    [Comment("商品編號")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [StringLength(10)]
    [Comment("商品名稱")]
    public string Name { get; set; }

    [Required] 
    [Comment("價格")] 
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [Required] 
    [Comment("數量")] 
    public int Quantity { get; set; }

    [Required]
    [Comment("建立時間")]
    public DateTime Created { get; set; }
    
    [Comment("更新時間")]
    public DateTime? Updated { get; set; }
}