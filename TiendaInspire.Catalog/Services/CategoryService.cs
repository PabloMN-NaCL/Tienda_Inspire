using MassTransit;
using Microsoft.EntityFrameworkCore;
using TiendaInspire.Catalog.Data;
using TiendaInspire.Catalog.Entities;
using TiendaInspire.Shared.CommonQuerys;
using static TiendaInspire.Catalog.Dtos.CategoryDTOs;

namespace TiendaInspire.Catalog.Services
{
    public class CategoryService :ICategoryService
    {
        CatalogDbContext _dbContext;
        ILogger<CategoryService> _logger;
        public  CategoryService(CatalogDbContext dbContext, ILogger<CategoryService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<CategoryResponse>>> GetAllAsync()
        {
            var categories = await _dbContext.Categories
                .Select(c => new CategoryResponse(c.Id, c.Name, c.Description, c.Products.Count))
                .ToListAsync();

            return ServiceResult<IEnumerable<CategoryResponse>>.Success(categories);
        }

        public async Task<ServiceResult<CategoryResponse>> GetByIdAsync(int id)
        {
            var category = await _dbContext.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryResponse(c.Id, c.Name, c.Description, c.Products.Count))
                .FirstOrDefaultAsync();

            if (category is null)
            {
                return ServiceResult<CategoryResponse>.Failure("Category not found");
            }

            return ServiceResult<CategoryResponse>.Success(category);
        }

        public async Task<ServiceResult<CategoryResponse>> CreateAsync(CreateCategoryRequest request)
        {
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description
            };

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Category created: {CategoryId} - {Name}", category.Id, category.Name);

            var response = new CategoryResponse(category.Id, category.Name, category.Description, 0);
            return ServiceResult<CategoryResponse>.Success(response, "Category created successfully");
        }

        public async Task<ServiceResult<CategoryResponse>> UpdateAsync(int id, UpdateCategoryRequest request)
        {
            var category = await _dbContext.Categories.FindAsync(id);
            if (category is null)
            {
                return ServiceResult<CategoryResponse>.Failure("Category not found");
            }

            category.Name = request.Name;
            category.Description = request.Description;

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Category updated: {CategoryId} - {Name}", category.Id, category.Name);

            var productCount = await _dbContext.Products.CountAsync(p => p.CategoryId == id);
            var response = new CategoryResponse(category.Id, category.Name, category.Description, productCount);
            return ServiceResult<CategoryResponse>.Success(response, "Category updated successfully");
        }

        public async Task<ServiceResult> DeleteAsync(int id)
        {
            var category = await _dbContext.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category is null)
            {
                return ServiceResult.Failure("Category not found");
            }

            if (category.Products.Count > 0)
            {
                return ServiceResult.Failure("Cannot delete category with products");
            }

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Category deleted: {CategoryId}", id);

            return ServiceResult.Success("Category deleted successfully");
        }
    }
}
