using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Core.Models
{
    public class Property
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string District { get; set; } = string.Empty;
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Bedrooms { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Bathrooms { get; set; }
        
        [Required]
        [Range(1, double.MaxValue)]
        public double AreaInSquareMeters { get; set; }
        
        [Required]
        public PropertyType PropertyType { get; set; }
        
        [Required]
        public PropertyStatus Status { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public DateTime? UpdatedDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public string? ImageUrl { get; set; }
        
        public List<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    }
    
    public enum PropertyType
    {
        Apartment = 1,
        House = 2,
        Studio = 3,
        Villa = 4,
        Office = 5,
        Shop = 6
    }
    
    public enum PropertyStatus
    {
        ForSale = 1,
        ForRent = 2,
        Sold = 3,
        Rented = 4,
        Pending = 5
    }
}
