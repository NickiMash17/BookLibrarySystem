using BookLibrarySystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookLibrarySystem.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(int authorId);
        Task<IEnumerable<Book>> GetBooksByCategoryAsync(int categoryId);
        Task<Book> AddBookAsync(Book book);
        Task<Book> UpdateBookAsync(Book book);
        Task<bool> DeleteBookAsync(int id);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
        Task<Dictionary<int, double>> GetBooksWithAverageRatingAsync();
        Task<IEnumerable<Book>> GetBooksByYearAsync(int year);
        Task<PagedResult<Book>> GetBooksPagedAsync(int pageNumber, int pageSize);
        Task<PagedResult<Book>> GetBooksByAuthorPagedAsync(int authorId, int pageNumber, int pageSize);
        Task<PagedResult<Book>> GetBooksByCategoryPagedAsync(int categoryId, int pageNumber, int pageSize);
        Task<PagedResult<Book>> SearchBooksPagedAsync(string searchTerm, int pageNumber, int pageSize);
    }
}