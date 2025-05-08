using BookLibrarySystem.Data;
using BookLibrarySystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrarySystem.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly BookLibraryContext _context;

        public BookRepository(BookLibraryContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .Include(b => b.Reviews)
                .ToListAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .Include(b => b.Reviews)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .Include(b => b.Reviews)
                .Where(b => b.AuthorId == authorId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .Include(b => b.Reviews)
                .Where(b => b.BookCategories.Any(bc => bc.CategoryId == categoryId))
                .ToListAsync();
        }

        public async Task<Book> AddBookAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book> UpdateBookAsync(Book book)
        {
            var existingBook = await _context.Books.FindAsync(book.Id);
            if (existingBook == null)
                throw new KeyNotFoundException($"Book with ID {book.Id} not found");

            _context.Entry(existingBook).CurrentValues.SetValues(book);
            await _context.SaveChangesAsync();
            return existingBook;
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            return await _context.Books
                .Include(b => b.Author)
                .Include(b => b.BookCategories)
                    .ThenInclude(bc => bc.Category)
                .Where(b => b.Title.Contains(searchTerm) || 
                            (b.Description != null && b.Description.Contains(searchTerm)) || 
                            (b.Author != null && (b.Author.FirstName.Contains(searchTerm) || 
                                                b.Author.LastName.Contains(searchTerm))))
                .ToListAsync();
        }

        public async Task<Dictionary<int, double>> GetBooksWithAverageRatingAsync()
        {
            return await _context.Books
                .Where(b => b.Reviews.Any())
                .Select(b => new
                {
                    BookId = b.Id,
                    AverageRating = b.Reviews.Average(r => r.Rating)
                })
                .ToDictionaryAsync(b => b.BookId, b => b.AverageRating);
        }
    }
}