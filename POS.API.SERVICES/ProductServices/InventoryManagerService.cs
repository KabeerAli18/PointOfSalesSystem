using Microsoft.EntityFrameworkCore.ChangeTracking;
using POS.API.MODEL.Products;
using POS.API.REPOSITORIES.ProductRepository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.API.SERVICES.ProductServices
{
    public class InventoryManagerService : IInventoryManagerService
    {
        private readonly IInventoryManagerRepository _inventoryManagerRepository;

        public InventoryManagerService(IInventoryManagerRepository inventoryManagerRepository)
        {
            _inventoryManagerRepository = inventoryManagerRepository;
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            try
            {
                var addedProduct = await _inventoryManagerRepository.AddProductAsync(product);
                return addedProduct;
            }
            catch(ArgumentException ex) { 
                throw new ArgumentException(ex.Message, nameof(product));
            }
            catch(InvalidOperationException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while adding product: ", ex);
            }
        }

        public async Task<IEnumerable<Product>> TrackProductInventory()
        {
            try
            {
                var products = await _inventoryManagerRepository.TrackProductInventory();
                return products;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while tracking product inventory: ", ex);
            }
        }

        public async Task<bool> UpdateProductAsync(string id, Product product)
        {
            try
            {
                var isUpdated = await _inventoryManagerRepository.UpdateProductAsync(id, product);
                return isUpdated;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message, nameof(product));
            }
            catch (Exception ex)
            {
                throw new Exception("Error while updating product: ", ex);
            }
        }

        public async Task<bool> RemoveProductAsync(string id)
        {
            try
            {
                var isRemoved = await _inventoryManagerRepository.RemoveProductAsync(id);
                return isRemoved;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message, nameof(id));
            }
            catch (Exception ex)
            {
                throw new Exception("Error while removing product: ", ex);
            }
        }

        public async Task<Product> ReceiveNewStockAsync(string id, int quantity)
        {
            try
            {
                var product = await _inventoryManagerRepository.ReceiveNewStockAsync(id, quantity);
                return product;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message, nameof(id));
            }
            catch (Exception ex)
            {
                throw new Exception("Error while receiving new stock: ", ex);
            }
        }

        public async Task<Product> ReduceStockAsync(string id, int quantity)
        {
            try
            {
                var product = await _inventoryManagerRepository.ReduceStockAsync(id, quantity);
                return product;
            }

            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message, nameof(id));
            }
            catch (Exception ex)
            {
                throw new Exception("Error while reducing stock: ", ex);
            }
        }

        public async Task<Product> FindProductByIDAsync(string id)
        {
            try
            {
                var product = await _inventoryManagerRepository.FindProductByIDAsync(id);
                return product;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message, nameof(id));
            }
            catch (Exception ex)
            {
                throw new Exception("Error while finding product by ID: ", ex);
            }
        }

        
    }
}
