using PropertyManagementSystem.Core.Models;

namespace PropertyManagementSystem.Core.Interfaces
{
    public interface IPropertyRepository
    {
        Task<IEnumerable<Property>> GetAllAsync();
        Task<Property?> GetByIdAsync(int id);
        Task<IEnumerable<Property>> SearchAsync(string? city = null, string? district = null, 
            PropertyType? propertyType = null, PropertyStatus? status = null, 
            decimal? minPrice = null, decimal? maxPrice = null);
        Task<Property> CreateAsync(Property property);
        Task<Property> UpdateAsync(Property property);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
