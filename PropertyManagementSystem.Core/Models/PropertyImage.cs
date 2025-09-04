using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Core.Models
{
    public class PropertyImage
    {
        public int Id { get; set; }
        
        [Required]
        public int PropertyId { get; set; }
        
        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? AltText { get; set; }
        
        public bool IsPrimary { get; set; }
        
        public DateTime CreatedDate { get; set; }
        
        public Property Property { get; set; } = null!;
    }
}
