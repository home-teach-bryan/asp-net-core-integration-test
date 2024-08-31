namespace AspNetCoreIntegration.Models.Request;

public class AddUserRequest
{
    /// <summary>
    /// 名稱
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 密碼
    /// </summary>
    public string Password { get; set; }
    
    /// <summary>
    /// 角色
    /// </summary>
    public string[] Roles { get; set; }
}