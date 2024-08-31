using AspNetCoreIntegration.Models.Request;
using AspNetCoreIntegrationTest.Models.Response;

namespace AspNetCoreIntegration.Services;

public interface IOrderService
{
    bool AddOrder(List<AddOrderRequest> addOrderRequests, Guid userId);
    IEnumerable<GetOrderDetailsResponse> GetOrderDetails(Guid userId);
}