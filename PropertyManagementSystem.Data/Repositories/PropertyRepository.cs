using PropertyManagementSystem.Core.Interfaces;
using PropertyManagementSystem.Core.Models;

namespace PropertyManagementSystem.Data.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly PropertyDbContext _context;

        public PropertyRepository(PropertyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Property>> GetAllAsync()
        {
            return await _context.GetAllPropertiesAsync();
        }

        public async Task<Property?> GetByIdAsync(int id)
        {
            return await _context.GetPropertyByIdAsync(id);
        }

        public async Task<IEnumerable<Property>> SearchAsync(string? city = null, string? district = null, 
            int? typeId = null, int? statusId = null, 
            decimal? minPrice = null, decimal? maxPrice = null)
        {
            return await _context.SearchPropertiesAsync(city, district, typeId, statusId, minPrice, maxPrice);
        }

        public async Task<Property> CreateAsync(Property property)
        {
            var id = await _context.CreatePropertyAsync(property);
            property.Id = id;
            return property;
        }

        public async Task<Property> UpdateAsync(Property property)
        {
            var success = await _context.UpdatePropertyAsync(property);
            if (!success)
            {
                throw new InvalidOperationException($"Property with ID {property.Id} not found or could not be updated.");
            }
            return property;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _context.DeletePropertyAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.PropertyExistsAsync(id);
        }
    }
}
