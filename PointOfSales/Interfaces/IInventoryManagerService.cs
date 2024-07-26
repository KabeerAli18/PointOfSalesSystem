using System.Collections.Generic;
using System.Threading.Tasks;
using PointOfSales.Entities;
using PointOfSales.Services;

namespace PointOfSales.Interfaces
{
    public interface IInventoryManagerService
    {
        Task<Product> AddProductAsync(Product product);
        Task<IEnumerable<Product>> TrackProductInventory();
        Task<bool> UpdateProductAsync(int id, Product product);
        Task<bool> RemoveProductAsync(int id);
        Task<Product> ReceiveNewStockAsync(int id, int quantity);
        Task<Product> ReduceStockAsync(int id, int quantity);
        Task<Product> FindProductByIDAsync(int id);
    }

}
