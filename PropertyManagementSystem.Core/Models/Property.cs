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
        public decimal Area { get; set; }
        
        [Required]
        public int TypeId { get; set; }
        
        [Required]
        public int StatusId { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public bool IsDeleted { get; set; } = false;
        
        public List<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    }
}
