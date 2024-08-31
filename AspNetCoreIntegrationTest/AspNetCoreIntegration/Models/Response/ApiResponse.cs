using AspNetCoreIntegration.Models.Enum;
using AspNetCoreIntegrationTest.Extension;

namespace AspNetCoreIntegrationTest.Models.Response;

public class ApiResponse<T>
{
    public ApiResponseStatus Status { get; private set; }
    
    public string Message { get; private set; }
    
    public List<string>? Errors { get; set; }
    
    public T? Data { get; set; }
    
    public ApiResponse(ApiResponseStatus status)
    {
        this.Status = status;
        this.Message = status.GetDescription();
    }

    public ApiResponse(ApiResponseStatus status, string message)
    {
        this.Status = status;
        this.Message = message;
    }
}