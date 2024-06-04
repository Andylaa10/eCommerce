using OpenTelemetry.Trace;

namespace ProductService.Service.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IRedisClient> _redisClient = new();
    private readonly Mock<TracerProvider> _tracerProvider = new();
    private readonly Core.Services.ProductService _productService;

    public ProductServiceTests()
    {
        _productService = new Core.Services.ProductService(
            _productRepository.Object,
            _mapper.Object,
            _redisClient.Object,
            _tracerProvider.Object.GetTracer("TestTracer"));
    }

    public static IEnumerable<object[]> GetProducts_TestCase()
    {
        var product1 = new Product
        {
            Id = ObjectId.GenerateNewId(),
            Title = "Soft cotton shoes",
            Description = "Made of out soft cotton",
            Price = 99.99f,
            InStock = true,
            NumberInStock = 44
        };

        var product2 = new Product
        {
            Id = ObjectId.GenerateNewId(),
            Title = "Hard leather boots",
            Description = "Durable and stylish leather boots",
            Price = 129.99f,
            InStock = true,
            NumberInStock = 10
        };

        var product3 = new Product
        {
            Id = ObjectId.GenerateNewId(),
            Title = "Canvas sneakers",
            Description = "Lightweight and breathable canvas sneakers",
            Price = 59.99f,
            InStock = false,
            NumberInStock = 0
        };

        var product4 = new Product
        {
            Id = ObjectId.GenerateNewId(),
            Title = "Woolen slippers",
            Description = "Warm and comfortable woolen slippers",
            Price = 39.99f,
            InStock = true,
            NumberInStock = 25
        };

        var product5 = new Product
        {
            Id = ObjectId.GenerateNewId(),
            Title = "Running shoes",
            Description = "High-performance running shoes with extra cushioning",
            Price = 89.99f,
            InStock = true,
            NumberInStock = 30
        };

        var paginatedDto1 = new PaginatedDto
        {
            PageNumber = 0,
            PageSize = 0
        };
        var paginatedDto2 = new PaginatedDto
        {
            PageNumber = 0,
            PageSize = 2
        };
        var paginatedDto3 = new PaginatedDto
        {
            PageNumber = 1,
            PageSize = 3
        };
        var paginatedDto4 = new PaginatedDto
        {
            PageNumber = 0,
            PageSize = 9
        };
        var paginatedDto5 = new PaginatedDto
        {
            PageNumber = 1,
            PageSize = 1
        };

        yield return
        [
            new Product[]
            {
            },
            new PaginatedResult<Product>
            {
                Items = [],
                TotalCount = 0
            },
            paginatedDto1
        ];

        yield return
        [
            new[]
            {
                product1, product2, product3, product4, product5
            },
            new PaginatedResult<Product>
            {
                Items = [product1, product2],
                TotalCount = 5
            },
            paginatedDto2
        ];

        yield return
        [
            new[]
            {
                product1, product2, product3, product4, product5
            },
            new PaginatedResult<Product>
            {
                Items = [product4, product5],
                TotalCount = 5
            },
            paginatedDto3
        ];

        yield return
        [
            new[]
            {
                product1, product2, product3, product4, product5
            },
            new PaginatedResult<Product>
            {
                Items = [product1, product2, product3, product4, product5],
                TotalCount = 5
            },
            paginatedDto4
        ];

        yield return
        [
            new[]
            {
                product1, product2, product3, product4, product5
            },
            new PaginatedResult<Product>
            {
                Items = [product2],
                TotalCount = 5
            },
            paginatedDto5
        ];
    }

    public static IEnumerable<object[]> GetProductByIdInvalid_TestCase()
    {
        yield return
        [
            "",
            typeof(ArgumentException),
            "Id cannot be null or empty"
        ];
        yield return
        [
            null,
            typeof(ArgumentException),
            "Id cannot be null or empty"
        ];

        yield return
        [
            "66504c167d44c191e7db8891",
            typeof(KeyNotFoundException),
            "No product with id of 66504c167d44c191e7db8891"
        ];
    }

    [Theory]
    [MemberData(nameof(GetProducts_TestCase))]
    public async Task GetProductsValid(Product[] data, PaginatedResult<Product> expectedResult, PaginatedDto dto)
    {
        // Arrange
        var paginatedResult = new PaginatedResult<Product>
        {
            Items = data.Skip(dto.PageSize * dto.PageNumber).Take(dto.PageSize).ToList(),
            TotalCount = data.Length
        };

        _productRepository.Setup(p => p.GetProducts(dto.PageNumber, dto.PageSize))
            .ReturnsAsync(paginatedResult);

        // Act
        var result = await _productService.GetProducts(dto);

        // Assert
        Assert.Equal(expectedResult.Items.Count, result.Items.Count);
        Assert.True(expectedResult.Items.SequenceEqual(result.Items));
        _productRepository.Verify(r => r.GetProducts(dto.PageNumber, dto.PageSize), Times.Once);
    }


    [Fact]
    public async Task CreateValidProduct()
    {
        // Arrange
        // Create product
        var productDto = new CreateProductDto
        {
            Title = "Soft cotton shoes",
            Description = "Made of out cotton",
            Price = 32.22f,
            InStock = true,
            NumberInStock = 12
        };

        var product = new Product
        {
            Id = ObjectId.GenerateNewId(),
            Title = "Soft cotton shoes",
            Description = "Made of out cotton",
            Price = 99.99f,
            InStock = true,
            NumberInStock = 44
        };

        _productRepository.Setup(p => p.CreateProduct(It.IsAny<Product>())).ReturnsAsync(product);

        // Act
        var result = await _productService.CreateProduct(productDto);

        // Assert
        Assert.Equal(productDto.Description, result.Description);
        _productRepository.Verify(r => r.CreateProduct(It.IsAny<Product>()), Times.Once);
    }

    [Theory]
    [InlineData("665db48d4c12d4ff114eb132")]
    [InlineData("665db48c4c12d4ff114eb131")]
    public async Task GetProductByIdProductValid(string productId)
    {
        // Arrange
        // Create product
        var product1 = new Product
        {
            Id = new ObjectId(productId),
            Title = "Soft cotton shoes",
            Description = "Made of out soft cotton",
            Price = 99.99f,
            InStock = true,
            NumberInStock = 44
        };
        var product2 = new Product
        {
            Id = ObjectId.GenerateNewId(),
            Title = "Hard cotton shoes",
            Description = "Made of out hard cotton",
            Price = 49.99f,
            InStock = true,
            NumberInStock = 2
        };

        var fakeRepo = new List<Product>
        {
            product1, product2
        };

        _productRepository.Setup(p => p.GetProductById(productId))!.ReturnsAsync(fakeRepo.Find(p =>
            p.Id == new ObjectId(productId)));

        // Act
        var result = await _productService.GetProductById(productId);

        // Assert
        Assert.Equal("Made of out soft cotton", result.Description);
        Assert.Equal("Soft cotton shoes", result.Title);
        Assert.Equal(99.99f, result.Price);
        Assert.True(result.InStock);
        Assert.Equal(44, result.NumberInStock);
        _productRepository.Verify(r => r.GetProductById(productId), Times.Once);
    }

    [Theory]
    [InlineData("", "Id cannot be null or empty")]
    [InlineData(null, "Id cannot be null or empty")]
    public async Task GetProductByIdProductInvalid(string productId, string expectedMessage)
    {
        // Arrange
        _productRepository.Setup(s => s.GetProductById(productId)).Throws<ArgumentException>();

        // Act
        var exception =
            await Assert.ThrowsAsync<ArgumentException>(async () => await _productService.GetProductById(productId));

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
        _productRepository.Verify(r => r.GetProductById(It.IsAny<string>()), Times.Never);
    }
}