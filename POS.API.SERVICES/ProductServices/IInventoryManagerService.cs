using POS.API.MODEL.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.SERVICES.ProductServices
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
