// Models/BookCategory.cs
namespace BookLibrarySystem.Models
{
    public class BookCategory
    {
        public int BookId { get; set; }
        public required Book Book { get; set; }
        
        public int CategoryId { get; set; }
        public required Category Category { get; set; }
    }
}