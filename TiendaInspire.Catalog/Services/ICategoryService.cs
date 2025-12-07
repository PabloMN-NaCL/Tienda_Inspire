using TiendaInspire.Shared.CommonQuerys;
using static TiendaInspire.Catalog.Dtos.CategoryDTOs;

namespace TiendaInspire.Catalog.Services
{
    public interface ICategoryService
    {
        /// <summary>
        /// Obtiene todas las categorías disponibles.
        /// </summary>
        /// <returns>Un ServiceResult que contiene una colección de CategoryResponse.</returns>
        Task<ServiceResult<IEnumerable<CategoryResponse>>> GetAllAsync();

        /// <summary>
        /// Obtiene una categoría por su identificador.
        /// </summary>
        /// <param name="id">El ID de la categoría.</param>
        /// <returns>Un ServiceResult que contiene un CategoryResponse.</returns>
        Task<ServiceResult<CategoryResponse>> GetByIdAsync(int id);

        /// <summary>
        /// Crea una nueva categoría.
        /// </summary>
        /// <param name="request">Los datos necesarios para crear la categoría.</param>
        /// <returns>Un ServiceResult que contiene el CategoryResponse de la categoría creada.</returns>
        Task<ServiceResult<CategoryResponse>> CreateAsync(CreateCategoryRequest request);

        /// <summary>
        /// Actualiza una categoría existente.
        /// </summary>
        /// <param name="id">El ID de la categoría a actualizar.</param>
        /// <param name="request">Los nuevos datos de la categoría.</param>
        /// <returns>Un ServiceResult que contiene el CategoryResponse de la categoría actualizada.</returns>
        Task<ServiceResult<CategoryResponse>> UpdateAsync(int id, UpdateCategoryRequest request);

        /// <summary>
        /// Elimina una categoría por su identificador.
        /// </summary>
        /// <param name="id">El ID de la categoría a eliminar.</param>
        /// <returns>Un ServiceResult sin un cuerpo de datos si la eliminación es exitosa.</returns>
        Task<ServiceResult> DeleteAsync(int id);
    }
}