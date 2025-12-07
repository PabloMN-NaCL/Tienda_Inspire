namespace TiendaInspire.Catalog.Dtos
{
    public class CategoryDTOs
    {
        public record CategoryResponse(int Id, string Name, string? Description, int ProductCount);

        public record CreateCategoryRequest(string Name, string? Description);

        public record UpdateCategoryRequest(string Name, string? Description);
    }
}
