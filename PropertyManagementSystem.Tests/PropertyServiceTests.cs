using Moq;
using PropertyManagementSystem.Core.Interfaces;
using PropertyManagementSystem.Core.Models;
using PropertyManagementSystem.Core.Services;

namespace PropertyManagementSystem.Tests
{
    public class PropertyServiceTests
    {
        [Fact]
        public async Task GetAllPropertiesAsync_ShouldReturnAllProperties()
        {
            // Arrange
            var mockRepository = new Mock<IPropertyRepository>();
            var expectedProperties = new List<Property>
            {
                new Property
                {
                    Id = 1,
                    Title = "Test Property 1",
                    Description = "Test Description 1",
                    Address = "Test Address 1",
                    City = "台北市",
                    District = "信義區",
                    Price = 1000000,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    Area = 30,
                    TypeId = 1,
                    StatusId = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Property
                {
                    Id = 2,
                    Title = "Test Property 2",
                    Description = "Test Description 2",
                    Address = "Test Address 2",
                    City = "台北市",
                    District = "大安區",
                    Price = 1500000,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    Area = 45,
                    TypeId = 1,
                    StatusId = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            mockRepository.Setup(r => r.GetAllAsync())
                          .ReturnsAsync(expectedProperties);

            var service = new PropertyService(mockRepository.Object);

            // Act
            var result = await service.GetAllPropertiesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Test Property 1", result.First().Title);
        }

        [Fact]
        public async Task GetPropertyByIdAsync_ShouldReturnProperty_WhenPropertyExists()
        {
            // Arrange
            var mockRepository = new Mock<IPropertyRepository>();
            var expectedProperty = new Property
            {
                Id = 1,
                Title = "Test Property",
                Description = "Test Description",
                Address = "Test Address",
                City = "台北市",
                District = "信義區",
                Price = 1000000,
                Bedrooms = 2,
                Bathrooms = 1,
                Area = 30,
                TypeId = 1,
                StatusId = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            mockRepository.Setup(r => r.GetByIdAsync(1))
                          .ReturnsAsync(expectedProperty);

            var service = new PropertyService(mockRepository.Object);

            // Act
            var result = await service.GetPropertyByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test Property", result.Title);
        }

        [Fact]
        public async Task GetPropertyByIdAsync_ShouldReturnNull_WhenPropertyDoesNotExist()
        {
            // Arrange
            var mockRepository = new Mock<IPropertyRepository>();
            mockRepository.Setup(r => r.GetByIdAsync(999))
                          .ReturnsAsync((Property?)null);

            var service = new PropertyService(mockRepository.Object);

            // Act
            var result = await service.GetPropertyByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreatePropertyAsync_ShouldReturnCreatedProperty()
        {
            // Arrange
            var mockRepository = new Mock<IPropertyRepository>();
            var propertyToCreate = new Property
            {
                Title = "New Property",
                Description = "New Description",
                Address = "New Address",
                City = "台北市",
                District = "信義區",
                Price = 2000000,
                Bedrooms = 3,
                Bathrooms = 2,
                Area = 50,
                TypeId = 1,
                StatusId = 1
            };

            var createdProperty = new Property
            {
                Id = 1,
                Title = propertyToCreate.Title,
                Description = propertyToCreate.Description,
                Address = propertyToCreate.Address,
                City = propertyToCreate.City,
                District = propertyToCreate.District,
                Price = propertyToCreate.Price,
                Bedrooms = propertyToCreate.Bedrooms,
                Bathrooms = propertyToCreate.Bathrooms,
                Area = propertyToCreate.Area,
                TypeId = propertyToCreate.TypeId,
                StatusId = propertyToCreate.StatusId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            mockRepository.Setup(r => r.CreateAsync(It.IsAny<Property>()))
                          .ReturnsAsync(createdProperty);

            var service = new PropertyService(mockRepository.Object);

            // Act
            var result = await service.CreatePropertyAsync(propertyToCreate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("New Property", result.Title);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task SearchPropertiesAsync_ShouldReturnFilteredProperties()
        {
            // Arrange
            var mockRepository = new Mock<IPropertyRepository>();
            var expectedProperties = new List<Property>
            {
                new Property
                {
                    Id = 1,
                    Title = "Property in 信義區",
                    Description = "Test Description",
                    Address = "Test Address",
                    City = "台北市",
                    District = "信義區",
                    Price = 1000000,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    Area = 30,
                    TypeId = 1,
                    StatusId = 1,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            mockRepository.Setup(r => r.SearchAsync("台北市", "信義區", null, null, null, null))
                          .ReturnsAsync(expectedProperties);

            var service = new PropertyService(mockRepository.Object);

            // Act
            var result = await service.SearchPropertiesAsync("台北市", "信義區");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("信義區", result.First().District);
        }

        [Fact]
        public async Task CreatePropertyAsync_ShouldHandleChineseCharacters()
        {
            // Arrange
            var mockRepository = new Mock<IPropertyRepository>();
            var propertyWithChinese = new Property
            {
                Title = "豪華中文物件",
                Description = "這是一個非常好的物件，位於台北市中心",
                Address = "台北市信義區市府路101號",
                City = "台北市",
                District = "信義區",
                Price = 25000000,
                Bedrooms = 4,
                Bathrooms = 3,
                Area = 80,
                TypeId = 1,
                StatusId = 1
            };

            var createdProperty = new Property
            {
                Id = 1,
                Title = propertyWithChinese.Title,
                Description = propertyWithChinese.Description,
                Address = propertyWithChinese.Address,
                City = propertyWithChinese.City,
                District = propertyWithChinese.District,
                Price = propertyWithChinese.Price,
                Bedrooms = propertyWithChinese.Bedrooms,
                Bathrooms = propertyWithChinese.Bathrooms,
                Area = propertyWithChinese.Area,
                TypeId = propertyWithChinese.TypeId,
                StatusId = propertyWithChinese.StatusId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            mockRepository.Setup(r => r.CreateAsync(It.IsAny<Property>()))
                          .ReturnsAsync(createdProperty);

            var service = new PropertyService(mockRepository.Object);

            // Act
            var result = await service.CreatePropertyAsync(propertyWithChinese);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("豪華中文物件", result.Title);
            Assert.Equal("這是一個非常好的物件，位於台北市中心", result.Description);
            Assert.Equal("台北市信義區市府路101號", result.Address);
            Assert.Equal("台北市", result.City);
            Assert.Equal("信義區", result.District);
        }
    }
}
