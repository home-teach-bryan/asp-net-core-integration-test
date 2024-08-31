using AspNetCoreIntegration.DbContext;
using AspNetCoreIntegration.Models.Request;
using AspNetCoreIntegrationTest.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIntegration.Services;

public class ProductService : IProductService
{
    private readonly ProductDbContext _dbContext;

    public ProductService(ProductDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public bool AddProduct(AddProductRequest product)
    {
        if (_dbContext.Products.Any(item => item.Name == product.Name))
        {
            return false;
        }
        var newProduct = new Product
        {
            Name = product.Name,
            Price = product.Price,
            Quantity = product.Quantity,
            Created = DateTime.Now,
            Updated = DateTime.Now
        };
        _dbContext.Products.Add(newProduct);
        _dbContext.SaveChanges();
        return true;
    }

    public bool UpdateProduct(Guid id, UpdateProductRequest product)
    {
        var existProduct = _dbContext.Products.FirstOrDefault(item => item.Id == id);
        if (existProduct == null)
        {
            return false;
        }
        existProduct.Name = product.Name;
        existProduct.Price = product.Price;
        existProduct.Quantity = product.Quantity;
        existProduct.Updated = DateTime.Now;
        _dbContext.SaveChanges();
        return true;
    }

    public bool RemoveProduct(Guid id)
    {
        var existProduct = _dbContext.Products.FirstOrDefault(item => item.Id == id);
        if (existProduct == null)
        {
            return false;
        }
        _dbContext.Products.Remove(existProduct);
        _dbContext.SaveChanges();
        return true;
    }

    public List<GetProductResponse> GetAllProducts()
    {
        return _dbContext.Products.AsNoTracking().Select(item => new GetProductResponse
        {
            Id = item.Id,
            Name = item.Name,
            Price = item.Price,
            Quantity = item.Quantity
        }).ToList();
    }

    public GetProductResponse GetProduct(Guid id)
    {
        var existProduct = _dbContext.Products.AsNoTracking().FirstOrDefault(item => item.Id == id);
        if (existProduct == null)
        {
            return new GetProductResponse();
        }
        return new GetProductResponse
        {
            Id = existProduct.Id,
            Name = existProduct.Name,
            Price = existProduct.Price,
            Quantity = existProduct.Quantity
        };
    }
}