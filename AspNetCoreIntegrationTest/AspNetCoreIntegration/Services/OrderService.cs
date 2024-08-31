using AspNetCoreIntegration.DbContext;
using AspNetCoreIntegration.Models.Request;
using AspNetCoreIntegrationTest.Models.Response;

namespace AspNetCoreIntegration.Services;

public class OrderService : IOrderService 
{
    private ProductDbContext _dbContext;

    public OrderService(ProductDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool AddOrder(List<AddOrderRequest> addOrderRequests, Guid userId)
    {
        var user = _dbContext.Users.FirstOrDefault(item => item.Id == userId);
        if (user == null)
        {
            return false;
        }
        var orderDetails = new List<OrderDetail>();
        foreach (var request in addOrderRequests)
        {
            var product = _dbContext.Products.FirstOrDefault(item => item.Id == request.ProductId);
            if (product == null)
            {
                return false;
            }
            if (product.Quantity < request.Quantity)
            {
                return false;
            }
            product.Quantity -= request.Quantity;
            product.Updated = DateTime.Now;
            orderDetails.Add(new OrderDetail
            {
                OrderPrice = product.Price,
                Product = product,
                OrderQuantity = request.Quantity
            });
        }

        var order = new Order
        {
            OrderUser = user,
            OrderDetails = orderDetails,
            Created = DateTime.Now,
            Updated = DateTime.Now
        };
        _dbContext.Orders.Add(order);
        _dbContext.SaveChanges();
        return true;
    }

    public IEnumerable<GetOrderDetailsResponse> GetOrderDetails(Guid userId)
    {
        var result = _dbContext.Orders.Where(item => item.OrderUser.Id == userId).Select(item =>
            new GetOrderDetailsResponse
            {
                OrderId = item.Id,
                OrderName = item.OrderUser.Name,
                Created = item.Created,
                Details = item.OrderDetails.Select(item2 => new OrderDetailResult
                {
                    ProductName = item2.Product.Name,
                    Price = item2.Product.Price,
                    Quantity = item2.OrderQuantity
                }),
                TotalPrice = item.OrderDetails.Sum(item3 => item3.OrderPrice),
                TotalQuantity = item.OrderDetails.Sum(item3 => item3.OrderQuantity)
            });
        return result;
    }
}