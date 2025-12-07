using Microsoft.EntityFrameworkCore;
using TiendaInspire.Catalog.Data;
using TiendaInspire.Catalog.Dtos;
using TiendaInspire.Catalog.Entities;
using TiendaInspire.Shared.CommonQuerys;
using static TiendaInspire.Catalog.Dtos.ProductDTO;

namespace TiendaInspire.Catalog.Services
{
    public class ProductService(CatalogDbContext dbContext, ILogger<ProductService> logger) : IProductService
    {

        //Get all
        public async Task<ServiceResult<IEnumerable<ProductListResponse>>> GetAllAsync(int? categoryId, string? search)
        {
            var query = dbContext.Products.Include(p => p.Category).AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search) || (p.Description != null && p.Description.Contains(search)));

            var products = await query
                .Select(p => new ProductListResponse(
                    p.Id, p.Name, p.Price, p.Stock, p.Category!.Name))
                .ToListAsync();

            return ServiceResult<IEnumerable<ProductListResponse>>.Success(products);

        }
        //GetbyId
        public async Task<ServiceResult<ProductResponse>> GetByIdAsync(int id)
        {
            var product = await dbContext.Products
                .Include(p => p.Category)
                .Where(p => p.Id == id)
                .Select(p => new ProductResponse(
                    p.Id, p.Name, p.Description, p.Price, p.Stock,  p.CategoryId, p.Category!.Name))
                .FirstOrDefaultAsync();

            if (product is null)
            {
                return ServiceResult<ProductResponse>.Failure("Product not found");
            }

            return ServiceResult<ProductResponse>.Success(product);
        }
        //Create
        public async Task<ServiceResult<ProductResponse>> CreateAsync(CreateProductRequest request)
        {
            var categoryExists = await dbContext.Categories.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
            {
                return ServiceResult<ProductResponse>.Failure("Category not found");
            }

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Stock = request.Stock,
                CategoryId = request.CategoryId
            };

            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync();

            var category = await dbContext.Categories.FindAsync(request.CategoryId);

            logger.LogInformation("Product created: {ProductId} - {Name}", product.Id, product.Name);

            var response = new ProductResponse(
                product.Id, product.Name, product.Description, product.Price,
                product.Stock, product.CategoryId, category!.Name);

            return ServiceResult<ProductResponse>.Success(response, "Product created successfully");
        }
        //Update
        public async Task<ServiceResult<ProductResponse>> UpdateAsync(int id, UpdateProductRequest request)
        {
            var product = await dbContext.Products.FindAsync(id);
            if (product is null)
            {
                return ServiceResult<ProductResponse>.Failure("Product not found");
            }

            var categoryExists = await dbContext.Categories.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
            {
                return ServiceResult<ProductResponse>.Failure("Category not found");
            }

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.Stock = request.Stock;
       
            product.CategoryId = request.CategoryId;
            

            await dbContext.SaveChangesAsync();

            var category = await dbContext.Categories.FindAsync(request.CategoryId);

            logger.LogInformation("Product updated: {ProductId} - {Name}", product.Id, product.Name);

            var response = new ProductResponse(
                product.Id, product.Name, product.Description, product.Price,
                product.Stock, product.CategoryId, category!.Name);

            return ServiceResult<ProductResponse>.Success(response, "Product updated successfully");
        }
        //Update stock
        public async Task<ServiceResult> UpdateStockAsync(int id, int quantity)
        {
            var product = await dbContext.Products.FindAsync(id);
            if (product is null)
            {
                return ServiceResult.Failure("Product not found");
            }

            product.Stock = quantity;
         

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Product stock updated: {ProductId} - New stock: {Stock}", id, quantity);

            return ServiceResult.Success("Stock updated successfully");
        }
        //Delete a product
        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var product = await dbContext.Products.FindAsync(id);
            if (product is null)
            {
                return ServiceResult.Failure("Product not found");
            }

            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Product deleted: {ProductId}", id);

            return ServiceResult.Success("Product deleted successfully");
        }
    }
}
