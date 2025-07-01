using BookLibrarySystem.Models;
using BookLibrarySystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibrarySystem.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace BookLibrarySystem.Controllers
{
    /// <summary>
    /// Controller for managing books in the library.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ActionLoggingFilter))]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMemoryCache _cache;
        private readonly IDistributedCache _distributedCache;

        public BooksController(IBookRepository bookRepository, IMemoryCache cache, IDistributedCache distributedCache)
        {
            _bookRepository = bookRepository;
            _cache = cache;
            _distributedCache = distributedCache;
        }

        /// <summary>
        /// Gets all books in the library. Uses in-memory and response caching.
        /// </summary>
        /// <returns>List of all books.</returns>
        [HttpGet]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {
            const string cacheKey = "all_books";
            if (!_cache.TryGetValue(cacheKey, out IEnumerable<Book> books))
            {
                books = await _bookRepository.GetAllBooksAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromSeconds(30));
                _cache.Set(cacheKey, books, cacheEntryOptions);
            }
            return Ok(books);
        }

        /// <summary>
        /// Gets a book by its ID.
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns>The requested book, or 404 if not found.</returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _bookRepository.GetBookByIdAsync(id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        /// <summary>
        /// Gets all books by a specific author.
        /// </summary>
        /// <param name="authorId">Author ID</param>
        /// <returns>List of books by the author.</returns>
        [HttpGet("author/{authorId:int}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByAuthor(int authorId)
        {
            return Ok(await _bookRepository.GetBooksByAuthorAsync(authorId));
        }

        /// <summary>
        /// Gets all books in a specific category.
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of books in the category.</returns>
        [HttpGet("category/{categoryId:int}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByCategory(int categoryId)
        {
            return Ok(await _bookRepository.GetBooksByCategoryAsync(categoryId));
        }

        /// <summary>
        /// Creates a new book. Restricted to Admins.
        /// </summary>
        /// <param name="book">Book to create</param>
        /// <returns>The created book.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {
            var createdBook = await _bookRepository.AddBookAsync(book);
            return CreatedAtAction(nameof(GetBook), new { id = createdBook.Id }, createdBook);
        }

        /// <summary>
        /// Updates an existing book. Restricted to Admins.
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <param name="book">Updated book data</param>
        /// <returns>The updated book.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {
            if (id != book.Id) return BadRequest();
            var updatedBook = await _bookRepository.UpdateBookAsync(book);
            return Ok(updatedBook);
        }

        /// <summary>
        /// Deletes a book. Restricted to Admins.
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns>No content if successful.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            if (!await _bookRepository.DeleteBookAsync(id)) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Searches for books by a search term.
        /// </summary>
        /// <param name="term">Search term</param>
        /// <returns>List of books matching the search term.</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Book>>> SearchBooks([FromQuery] string term)
        {
            return Ok(await _bookRepository.SearchBooksAsync(term));
        }

        /// <summary>
        /// Gets the average rating for all books.
        /// </summary>
        /// <returns>Dictionary of book IDs and their average ratings.</returns>
        [HttpGet("ratings")]
        public async Task<ActionResult<Dictionary<int, double>>> GetBooksWithAverageRating()
        {
            return Ok(await _bookRepository.GetBooksWithAverageRatingAsync());
        }

        /// <summary>
        /// Gets all books published in a specific year.
        /// </summary>
        /// <param name="year">Year of publication</param>
        /// <returns>List of books published in the specified year.</returns>
        [HttpGet("published/{year:int}")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksByYear(int year)
        {
            // This method would need to be implemented in the repository
            var books = await _bookRepository.GetBooksByYearAsync(year);
            return Ok(books);
        }

        /// <summary>
        /// Gets all books using distributed (Redis) cache.
        /// </summary>
        /// <returns>List of all books (cached in Redis).</returns>
        [HttpGet("distributed")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooksDistributedCache()
        {
            const string cacheKey = "all_books_distributed";
            var cachedBooks = await _distributedCache.GetStringAsync(cacheKey);
            IEnumerable<Book> books;

            if (string.IsNullOrEmpty(cachedBooks))
            {
                books = await _bookRepository.GetAllBooksAsync();
                var serialized = JsonSerializer.Serialize(books);
                await _distributedCache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });
            }
            else
            {
                books = JsonSerializer.Deserialize<IEnumerable<Book>>(cachedBooks);
            }

            return Ok(books);
        }

        /// <summary>
        /// Gets a paginated list of books.
        /// </summary>
        /// <param name="pageNumber">Page number (default 1)</param>
        /// <param name="pageSize">Page size (default 10)</param>
        /// <returns>Paginated list of books.</returns>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<Book>>> GetBooksPaged([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _bookRepository.GetBooksPagedAsync(pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Gets a paginated list of books by a specific author.
        /// </summary>
        /// <param name="authorId">Author ID</param>
        /// <param name="pageNumber">Page number (default 1)</param>
        /// <param name="pageSize">Page size (default 10)</param>
        /// <returns>Paginated list of books by the author.</returns>
        [HttpGet("author/{authorId:int}/paged")]
        public async Task<ActionResult<PagedResult<Book>>> GetBooksByAuthorPaged(int authorId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _bookRepository.GetBooksByAuthorPagedAsync(authorId, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Gets a paginated list of books in a specific category.
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <param name="pageNumber">Page number (default 1)</param>
        /// <param name="pageSize">Page size (default 10)</param>
        /// <returns>Paginated list of books in the category.</returns>
        [HttpGet("category/{categoryId:int}/paged")]
        public async Task<ActionResult<PagedResult<Book>>> GetBooksByCategoryPaged(int categoryId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _bookRepository.GetBooksByCategoryPagedAsync(categoryId, pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Searches for books with pagination.
        /// </summary>
        /// <param name="term">Search term</param>
        /// <param name="pageNumber">Page number (default 1)</param>
        /// <param name="pageSize">Page size (default 10)</param>
        /// <returns>Paginated list of books matching the search term.</returns>
        [HttpGet("search/paged")]
        public async Task<ActionResult<PagedResult<Book>>> SearchBooksPaged([FromQuery] string term, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _bookRepository.SearchBooksPagedAsync(term, pageNumber, pageSize);
            return Ok(result);
        }
    }
}