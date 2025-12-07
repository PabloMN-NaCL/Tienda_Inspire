namespace TiendaInspire.Catalog.Dtos
{
    public class ProductDTO
    {
        public record ProductResponse(
            int Id,
            string Name,
            string? Description,
            decimal Price,
            int Stock,
            
            int CategoryId,
            string CategoryName);

        public record ProductListResponse(
            int Id,
            string Name,
            decimal Price,
            int Stock,
            string CategoryName);

        public record CreateProductRequest(
            string Name,
            string? Description,
            decimal Price,
            int Stock,
            int CategoryId);

        public record UpdateProductRequest(
            string Name,
            string? Description,
            decimal Price,
            int Stock,
         
            int CategoryId);

        public record UpdateStockRequest(int Quantity);

        public record StockOperationRequest(int Quantity);
    }
}
