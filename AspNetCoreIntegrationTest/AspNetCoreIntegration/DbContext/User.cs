using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIntegration.DbContext;

public class User
{
    [Key]
    [Required]
    [Comment("使用者編號")]
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(10)]
    [Comment("使用者名稱")]
    public string Name { get; set; }
    
    [Required]
    [Comment("密碼")]
    public string Password { get; set; }
    
    [Comment("角色")]
    public string[]? Roles { get; set; }
    
    [Comment("建立時間")]
    public DateTime Created { get; set; }
    
    [Comment("更新時間")]
    public DateTime Updated { get; set; }
}