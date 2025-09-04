using PropertyManagementSystem.Core.Models;

namespace PropertyManagementSystem.Core.Interfaces
{
    public interface IPropertyService
    {
        Task<IEnumerable<Property>> GetAllPropertiesAsync();
        Task<Property?> GetPropertyByIdAsync(int id);
        Task<IEnumerable<Property>> SearchPropertiesAsync(string? city = null, string? district = null, 
            PropertyType? propertyType = null, PropertyStatus? status = null, 
            decimal? minPrice = null, decimal? maxPrice = null);
        Task<Property> CreatePropertyAsync(Property property);
        Task<Property> UpdatePropertyAsync(Property property);
        Task<bool> DeletePropertyAsync(int id);
        Task<bool> PropertyExistsAsync(int id);
    }
}
