namespace ProductService.API.Tests;

public class ProductControllerTests
{
    private readonly Mock<IProductService> _productService = new();
    private readonly ProductController _productController;

    public ProductControllerTests()
    {
        _productController =
            new ProductController(_productService.Object);
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

    [Theory]
    [MemberData(nameof(GetProducts_TestCase))]
    public async Task TestGetProducts_ReturnsOkAndPaginatedResult(Product[] data,
        PaginatedResult<Product> expectedResult, PaginatedDto dto)
    {
        // Arrange
        var paginatedResult = new PaginatedResult<Product>
        {
            Items = data.Skip(dto.PageSize * dto.PageNumber).Take(dto.PageSize).ToList(),
            TotalCount = data.Length
        };

        _productService.Setup(s => s.GetProducts(dto)).ReturnsAsync(paginatedResult);

        // Act
        var result = await _productController.GetProducts(dto);

        // Assert
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var productResult = Assert.IsType<PaginatedResult<Product>>(objectResult.Value);
        Assert.Equal(200, objectResult.StatusCode);
        Assert.True(expectedResult.Items.SequenceEqual(productResult.Items));
        _productService.Verify(r => r.GetProducts(dto), Times.Once);
    }

    [Fact]
    public async Task TestCreateProduct_Returns201AndProduct()
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

        _productService.Setup(p => p.CreateProduct(It.IsAny<CreateProductDto>())).ReturnsAsync(product);

        // Act
        var result = await _productController.CreateProduct(productDto);

        // Assert    
        var objectResult = Assert.IsType<ObjectResult>(result);
        var productResult = Assert.IsType<Product>(objectResult.Value);
        Assert.Equal(201, objectResult.StatusCode);
        Assert.Equal(product, productResult);
        Assert.Equal(product.Description, productResult.Description);
        Assert.Equal(product.InStock, productResult.InStock);
        Assert.Equal(product.NumberInStock, productResult.NumberInStock);
        Assert.Equal(product.Title, productResult.Title);
        Assert.Equal(product.Price, productResult.Price);
        _productService.Verify(s => s.CreateProduct(It.IsAny<CreateProductDto>()), Times.Once);
    }

    [Theory]
    [InlineData("66504a166d44c191e7db8890")]
    [InlineData("66504b15b826b7d8c9e4f62f")]
    public async Task TestGetProductById_ReturnsOkAndProduct(string id)
    {
        // Arrange
        var product1 = new Product
        {
            Id = new ObjectId(id),
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

        _productService.Setup(s => s.GetProductById(It.IsAny<string>()))!.ReturnsAsync(fakeRepo.Find(p => p.Id == new ObjectId(id)));
        // Act
        var result = await _productController.GetProductById(id);

        // Assert
        var objectResult = Assert.IsType<OkObjectResult>(result);
        var productResult = Assert.IsType<Product>(objectResult.Value);
        Assert.Equal(200, objectResult.StatusCode);
        Assert.Equal(product1, productResult);
        Assert.Equal(product1.Description, productResult.Description);
        Assert.Equal(product1.Price, productResult.Price);
        Assert.Equal(product1.Title, productResult.Title);
        Assert.Equal(product1.NumberInStock, productResult.NumberInStock);
        Assert.Equal(product1.InStock, productResult.InStock);
        _productService.Verify(s => s.GetProductById(It.IsAny<string>()), Times.Once());
    }

    [Fact]
    public async Task GetProductById_NonExistingId_ReturnsBadRequest()
    {
        // Arrange
        var nonExistingId = "nonexistingid";

        _productService.Setup(s => s.GetProductById(nonExistingId)).ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _productController.GetProductById(nonExistingId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
}