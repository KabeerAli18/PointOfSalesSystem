using POS.API.MODEL.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.REPOSITORIES.ProductRepository
{
    public interface IInventoryManagerRepository
    {
        Task<Product> AddProductAsync(Product product);
        Task<IEnumerable<Product>> TrackProductInventory();
        Task<bool> UpdateProductAsync(string id, Product product);
        Task<bool> RemoveProductAsync(string id);
        Task<Product> ReceiveNewStockAsync(string id, int quantity);
        Task<Product> ReduceStockAsync(string id, int quantity);
        Task<Product> FindProductByIDAsync(string id);
    }
}
