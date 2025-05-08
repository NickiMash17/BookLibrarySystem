// Models/Book.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookLibrarySystem.Models
{
    public class Book
    {
        public int Id { get; set; }
        
        [Required, MaxLength(200)]
        public string Title { get; set; } = null!;
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        public DateTime PublishedDate { get; set; }
        
        [MaxLength(50)]
        public string? ISBN { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        public int PageCount { get; set; }
        
        [MaxLength(100)]
        public string? Publisher { get; set; }
        
        public string? Language { get; set; }
        
        public string? CoverImageUrl { get; set; }
        
        public bool IsAvailable { get; set; } = true;
        
        // Foreign key
        public int AuthorId { get; set; }
        
        // Navigation properties
        public Author? Author { get; set; }
        public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}