using System.ComponentModel.DataAnnotations;

namespace BookLibrarySystem.Models
{
    public class Review
    {
        public int Id { get; set; }
        
        [Required, Range(1, 5)]
        public int Rating { get; set; }
        
        [MaxLength(1000)]
        public string? Comment { get; set; }
        
        public DateTime ReviewDate { get; set; }
        
        [MaxLength(100)]
        public string? ReviewerName { get; set; }
        
        [MaxLength(100)]
        public string? ReviewerEmail { get; set; }
        
        public int BookId { get; set; }
        [Required]
        public Book Book { get; set; } = null!;
    }
}