using TiendaInspire.Shared.CommonQuerys;
using System.Collections.Generic;
using System.Threading.Tasks;
using static TiendaInspire.Catalog.Dtos.ProductDTO;

namespace TiendaInspire.Catalog.Services
{
    public interface IProductService
    {
        /// <summary>
        /// Obtiene una lista paginada de productos, con filtros opcionales.
        /// </summary>
        /// <param name="categoryId">Filtra por ID de categoría (opcional).</param>
        /// <param name="search">Término de búsqueda por nombre o descripción (opcional).</param>
        /// <returns>Un ServiceResult con una colección de ProductListResponse.</returns>
        Task<ServiceResult<IEnumerable<ProductListResponse>>> GetAllAsync(int? categoryId, string? search);

        /// <summary>
        /// Obtiene un producto por su identificador.
        /// </summary>
        /// <param name="id">El ID del producto.</param>
        /// <returns>Un ServiceResult con el ProductResponse del producto encontrado.</returns>
        Task<ServiceResult<ProductResponse>> GetByIdAsync(int id);

        /// <summary>
        /// Crea un nuevo producto.
        /// </summary>
        /// <param name="request">Los datos de creación del producto.</param>
        /// <returns>Un ServiceResult con el ProductResponse del producto creado.</returns>
        Task<ServiceResult<ProductResponse>> CreateAsync(CreateProductRequest request);

        /// <summary>
        /// Actualiza un producto existente.
        /// </summary>
        /// <param name="id">El ID del producto a actualizar.</param>
        /// <param name="request">Los nuevos datos del producto.</param>
        /// <returns>Un ServiceResult con el ProductResponse del producto actualizado.</returns>
        Task<ServiceResult<ProductResponse>> UpdateAsync(int id, UpdateProductRequest request);

        /// <summary>
        /// Actualiza el stock de un producto específico.
        /// </summary>
        /// <param name="id">El ID del producto.</param>
        /// <param name="quantity">La nueva cantidad de stock.</param>
        /// <returns>Un ServiceResult de éxito/fallo.</returns>
        Task<ServiceResult> UpdateStockAsync(int id, int quantity);

        /// <summary>
        /// Elimina un producto por su identificador.
        /// </summary>
        /// <param name="id">El ID del producto a eliminar.</param>
        /// <returns>Un ServiceResult de éxito/fallo.</returns>
        Task<ServiceResult> DeleteAsync(int id);
    }
}