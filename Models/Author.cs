using System.ComponentModel.DataAnnotations;

namespace BookLibrarySystem.Models
{
    public class Author
    {
        public int Id { get; set; }
        
        [Required, MaxLength(100)]
        public required string FirstName { get; set; }
        
        [Required, MaxLength(100)]
        public required string LastName { get; set; }
        
        [MaxLength(500)]
        public string? Biography { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        public string? Nationality { get; set; }
        
        public ICollection<Book> Books { get; set; } = [];
    }
}