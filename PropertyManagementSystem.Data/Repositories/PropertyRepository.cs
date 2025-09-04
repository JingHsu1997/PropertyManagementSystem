using Microsoft.EntityFrameworkCore;
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
            return await _context.Properties
                .Include(p => p.Images)
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<Property?> GetByIdAsync(int id)
        {
            return await _context.Properties
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);
        }

        public async Task<IEnumerable<Property>> SearchAsync(string? city = null, string? district = null, 
            PropertyType? propertyType = null, PropertyStatus? status = null, 
            decimal? minPrice = null, decimal? maxPrice = null)
        {
            var query = _context.Properties
                .Include(p => p.Images)
                .Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(city))
            {
                query = query.Where(p => p.City.Contains(city));
            }

            if (!string.IsNullOrWhiteSpace(district))
            {
                query = query.Where(p => p.District.Contains(district));
            }

            if (propertyType.HasValue)
            {
                query = query.Where(p => p.PropertyType == propertyType.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(p => p.Status == status.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            return await query
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();
        }

        public async Task<Property> CreateAsync(Property property)
        {
            _context.Properties.Add(property);
            await _context.SaveChangesAsync();
            return property;
        }

        public async Task<Property> UpdateAsync(Property property)
        {
            _context.Entry(property).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return property;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
                return false;

            // Soft delete - mark as inactive
            property.IsActive = false;
            property.UpdatedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Properties
                .AnyAsync(p => p.Id == id && p.IsActive);
        }
    }
}
