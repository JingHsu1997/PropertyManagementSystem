using PropertyManagementSystem.Core.Interfaces;
using PropertyManagementSystem.Core.Models;

namespace PropertyManagementSystem.Core.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;

        public PropertyService(IPropertyRepository propertyRepository)
        {
            _propertyRepository = propertyRepository;
        }

        public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
        {
            return await _propertyRepository.GetAllAsync();
        }

        public async Task<Property?> GetPropertyByIdAsync(int id)
        {
            if (id <= 0)
                return null;

            return await _propertyRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Property>> SearchPropertiesAsync(string? city = null, string? district = null, 
            int? typeId = null, int? statusId = null, 
            decimal? minPrice = null, decimal? maxPrice = null)
        {
            return await _propertyRepository.SearchAsync(city, district, typeId, statusId, minPrice, maxPrice);
        }

        public async Task<Property> CreatePropertyAsync(Property property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            property.CreatedAt = DateTime.UtcNow;
            return await _propertyRepository.CreateAsync(property);
        }

        public async Task<Property> UpdatePropertyAsync(Property property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (!await _propertyRepository.ExistsAsync(property.Id))
                throw new InvalidOperationException($"Property with ID {property.Id} not found.");

            property.UpdatedAt = DateTime.UtcNow;
            return await _propertyRepository.UpdateAsync(property);
        }

        public async Task<bool> DeletePropertyAsync(int id)
        {
            if (id <= 0)
                return false;

            return await _propertyRepository.DeleteAsync(id);
        }

        public async Task<bool> PropertyExistsAsync(int id)
        {
            if (id <= 0)
                return false;

            return await _propertyRepository.ExistsAsync(id);
        }
    }
}
