using BookLibrarySystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookLibrarySystem.Data
{
    public static class DbInitializer
    {
        public static void Initialize(BookLibraryContext context)
        {
            context.Database.EnsureCreated();

            if (context.Books.Any()) return;

            var authors = new Author[]
            {
                new Author { FirstName = "J.K.", LastName = "Rowling", Biography = "Harry Potter author", DateOfBirth = new DateTime(1965, 7, 31), Nationality = "British" },
                new Author { FirstName = "George", LastName = "Orwell", Biography = "1984 author", DateOfBirth = new DateTime(1903, 6, 25), Nationality = "British" }
            };

            context.Authors.AddRange(authors);
            context.SaveChanges();

            var categories = new Category[]
            {
                new Category { Name = "Fantasy", Description = "Fantasy books" },
                new Category { Name = "Dystopian", Description = "Dystopian fiction" }
            };

            context.Categories.AddRange(categories);
            context.SaveChanges();

            var books = new Book[]
            {
                new Book { Title = "Harry Potter", Description = "Wizard book", PublishedDate = new DateTime(1997, 6, 26), ISBN = "123456789", Price = 19.99m, PageCount = 320, AuthorId = authors[0].Id },
                new Book { Title = "1984", Description = "Dystopian novel", PublishedDate = new DateTime(1949, 6, 8), ISBN = "987654321", Price = 14.99m, PageCount = 328, AuthorId = authors[1].Id }
            };

            context.Books.AddRange(books);
            context.SaveChanges();

            var bookCategories = new List<BookCategory>
            {
                new BookCategory 
                { 
                    Book = books[0], 
                    Category = categories[0] 
                },
                new BookCategory 
                { 
                    Book = books[1], 
                    Category = categories[1] 
                }
            };

            context.Set<BookCategory>().AddRange(bookCategories);
            context.SaveChanges();

            var reviews = new Review[]
            {
                new Review { BookId = books[0].Id, Rating = 5, Comment = "Great book!", ReviewDate = DateTime.Now, ReviewerName = "John Doe", ReviewerEmail = "john@example.com" },
                new Review { BookId = books[1].Id, Rating = 4, Comment = "Thought-provoking", ReviewDate = DateTime.Now, ReviewerName = "Jane Smith", ReviewerEmail = "jane@example.com" }
            };

            context.Reviews.AddRange(reviews);
            context.SaveChanges();
        }
    }
}