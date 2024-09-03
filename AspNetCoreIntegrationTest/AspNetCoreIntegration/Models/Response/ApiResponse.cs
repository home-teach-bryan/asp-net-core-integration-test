using System.Text.Json.Serialization;
using AspNetCoreIntegration.Models.Enum;
using AspNetCoreIntegrationTest.Extension;

namespace AspNetCoreIntegration.Models.Response;

public class ApiResponse<T>
{
    [JsonPropertyName("status")]
    public ApiResponseStatus Status { get; set; }
    
    [JsonPropertyName("message")]
    public string Message { get; set; }
    
    [JsonPropertyName("errors")]
    public List<string>? Errors { get; set; }
    
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    public ApiResponse()
    {

    }

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