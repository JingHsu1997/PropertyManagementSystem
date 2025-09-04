using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.Core.Models;
using PropertyManagementSystem.Core.Services;
using PropertyManagementSystem.Data;
using PropertyManagementSystem.Data.Repositories;

namespace PropertyManagementSystem.Tests
{
    public class PropertyServiceTests
    {
        private PropertyDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<PropertyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new PropertyDbContext(options);
        }

        [Fact]
        public async Task GetAllPropertiesAsync_ShouldReturnAllActiveProperties()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new PropertyRepository(context);
            var service = new PropertyService(repository);

            var properties = new List<Property>
            {
                new Property
                {
                    Title = "Test Property 1",
                    Description = "Test Description 1",
                    Address = "Test Address 1",
                    City = "台北市",
                    District = "信義區",
                    Price = 1000000,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    AreaInSquareMeters = 30,
                    PropertyType = PropertyType.Apartment,
                    Status = PropertyStatus.ForSale,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new Property
                {
                    Title = "Test Property 2",
                    Description = "Test Description 2",
                    Address = "Test Address 2",
                    City = "台北市",
                    District = "大安區",
                    Price = 2000000,
                    Bedrooms = 3,
                    Bathrooms = 2,
                    AreaInSquareMeters = 40,
                    PropertyType = PropertyType.House,
                    Status = PropertyStatus.ForRent,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new Property
                {
                    Title = "Inactive Property",
                    Description = "Inactive Description",
                    Address = "Inactive Address",
                    City = "台北市",
                    District = "松山區",
                    Price = 1500000,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    AreaInSquareMeters = 25,
                    PropertyType = PropertyType.Studio,
                    Status = PropertyStatus.Sold,
                    IsActive = false,
                    CreatedDate = DateTime.UtcNow
                }
            };

            context.Properties.AddRange(properties);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetAllPropertiesAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, p => Assert.True(p.IsActive));
        }

        [Fact]
        public async Task GetPropertyByIdAsync_WithValidId_ShouldReturnProperty()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new PropertyRepository(context);
            var service = new PropertyService(repository);

            var property = new Property
            {
                Title = "Test Property",
                Description = "Test Description",
                Address = "Test Address",
                City = "台北市",
                District = "信義區",
                Price = 1000000,
                Bedrooms = 2,
                Bathrooms = 1,
                AreaInSquareMeters = 30,
                PropertyType = PropertyType.Apartment,
                Status = PropertyStatus.ForSale,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            context.Properties.Add(property);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetPropertyByIdAsync(property.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(property.Title, result.Title);
            Assert.Equal(property.City, result.City);
        }

        [Fact]
        public async Task GetPropertyByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new PropertyRepository(context);
            var service = new PropertyService(repository);

            // Act
            var result = await service.GetPropertyByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreatePropertyAsync_WithValidProperty_ShouldCreateProperty()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new PropertyRepository(context);
            var service = new PropertyService(repository);

            var property = new Property
            {
                Title = "New Property",
                Description = "New Description",
                Address = "New Address",
                City = "台北市",
                District = "信義區",
                Price = 1500000,
                Bedrooms = 3,
                Bathrooms = 2,
                AreaInSquareMeters = 35,
                PropertyType = PropertyType.Apartment,
                Status = PropertyStatus.ForSale
            };

            // Act
            var result = await service.CreatePropertyAsync(property);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal(property.Title, result.Title);
            Assert.True(result.CreatedDate > DateTime.MinValue);

            var savedProperty = await context.Properties.FindAsync(result.Id);
            Assert.NotNull(savedProperty);
        }

        [Fact]
        public async Task CreatePropertyAsync_WithNullProperty_ShouldThrowArgumentNullException()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new PropertyRepository(context);
            var service = new PropertyService(repository);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreatePropertyAsync(null!));
        }

        [Fact]
        public async Task SearchPropertiesAsync_WithCityFilter_ShouldReturnFilteredProperties()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new PropertyRepository(context);
            var service = new PropertyService(repository);

            var properties = new List<Property>
            {
                new Property
                {
                    Title = "Taipei Property",
                    Description = "Description",
                    Address = "Address",
                    City = "台北市",
                    District = "信義區",
                    Price = 1000000,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    AreaInSquareMeters = 30,
                    PropertyType = PropertyType.Apartment,
                    Status = PropertyStatus.ForSale,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                },
                new Property
                {
                    Title = "Taichung Property",
                    Description = "Description",
                    Address = "Address",
                    City = "台中市",
                    District = "西屯區",
                    Price = 800000,
                    Bedrooms = 2,
                    Bathrooms = 1,
                    AreaInSquareMeters = 25,
                    PropertyType = PropertyType.Apartment,
                    Status = PropertyStatus.ForSale,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow
                }
            };

            context.Properties.AddRange(properties);
            await context.SaveChangesAsync();

            // Act
            var result = await service.SearchPropertiesAsync(city: "台北市");

            // Assert
            Assert.Single(result);
            Assert.Equal("台北市", result.First().City);
        }

        [Fact]
        public async Task DeletePropertyAsync_WithValidId_ShouldSoftDeleteProperty()
        {
            // Arrange
            using var context = GetInMemoryDbContext();
            var repository = new PropertyRepository(context);
            var service = new PropertyService(repository);

            var property = new Property
            {
                Title = "Property to Delete",
                Description = "Description",
                Address = "Address",
                City = "台北市",
                District = "信義區",
                Price = 1000000,
                Bedrooms = 2,
                Bathrooms = 1,
                AreaInSquareMeters = 30,
                PropertyType = PropertyType.Apartment,
                Status = PropertyStatus.ForSale,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            context.Properties.Add(property);
            await context.SaveChangesAsync();

            // Act
            var result = await service.DeletePropertyAsync(property.Id);

            // Assert
            Assert.True(result);

            var deletedProperty = await context.Properties.FindAsync(property.Id);
            Assert.NotNull(deletedProperty);
            Assert.False(deletedProperty.IsActive);
            Assert.NotNull(deletedProperty.UpdatedDate);
        }
    }
}