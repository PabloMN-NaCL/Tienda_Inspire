
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Tienda_Inspire.Catalog; // Asume que CatalogDbContext está aquí
using TiendaInspire.Catalog.Data;
using TiendaInspire.Catalog.Services;

namespace Tienda_Inspire.Catalog.Tests
{
    //[TestFixture]
    //public class CategoryServiceTests
    //{
    //    private Mock<ILogger<CategoryService>> _mockLogger;
    //    private CatalogDbContext _dbContext;

    //    private CategoryService _categoryService;


    //    [SetUp]
    //    public void Setup()
    //    {
    //        // 1. Mock Logger
    //        // El logger generalmente no necesita setups específicos a menos que pruebes el logueo
    //        _mockLogger = new Mock<ILogger<CategoryService>>();

    //        // 2. Configurar el DbContext usando una base de datos en memoria (InMemory)
    //        var options = new DbContextOptionsBuilder<CatalogDbContext>()
    //            .UseInMemoryDatabase(databaseName: $"TestCatalogDb_{TestContext.CurrentContext.Test.ID}")
    //            .Options;

    //        // Inicializar el DbContext. Es importante usar un nombre de base de datos único
    //        // para cada ejecución de test (usando TestContext.CurrentContext.Test.ID)
    //        // para asegurar el aislamiento entre tests.
    //        _dbContext = new CatalogDbContext(options);

    //        // Asegúrate de que la base de datos se crea y está limpia antes de cada test
    //        _dbContext.Database.EnsureDeleted(); // Opcional, pero recomendado para tests
    //        _dbContext.Database.EnsureCreated();

    //        // 3. Inicializar el servicio a testear
    //        _categoryService = new CategoryService(
    //            _dbContext,
    //            _mockLogger.Object
    //        );

    //        // Opcional: Agregar datos iniciales a la base de datos en memoria si es necesario
    //        // SeedDatabase();
    //    }

    //    [Test]
    //    public void Test1()
    //    {
    //        Assert.Pass();
    //    }
    //}
}
