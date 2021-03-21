namespace BookShop
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            //1.AgeRestriction
            //string command = Console.ReadLine();
            //Console.WriteLine(GetBooksByAgeRestriction(db,command));

            //2.GoldenBooks
            //Console.WriteLine(GetGoldenBooks(db));

            //3.BooksByPrice
            //Console.WriteLine(GetBooksByPrice(db));

            //4.NotReleasedIn
            //int year = int.Parse(Console.ReadLine());
            //Console.WriteLine(GetBooksNotReleasedIn(db, year));

            //5.BookTitlesByCategory
            //var input = Console.ReadLine();
            //Console.WriteLine(GetBooksByCategory(db, input));

            //6.ReleasedBeforeDate
            //var date = Console.ReadLine();
            //Console.WriteLine(GetBooksReleasedBefore(db, date));

            //7.AuthorSearch
            //var dateInput = Console.ReadLine();
            //Console.WriteLine(GetAuthorNamesEndingIn(db, dateInput));

            //8.BookSearch
            //var input = Console.ReadLine();
            //Console.WriteLine(GetBookTitlesContaining(db, input));

            //9.BookSearchByAuthor
            //var input = Console.ReadLine();
            //Console.WriteLine(GetBooksByAuthor(db, input));

            //10.CountBooks
            //int input = int.Parse(Console.ReadLine());
            //Console.WriteLine(CountBooks(db, input));

            //11.TotalBookCopies
            //Console.WriteLine(CountCopiesByAuthor(db));

            //12.ProfitByCategory
            //Console.WriteLine(GetTotalProfitByCategory(db));

            //13.MostRecentBooks
            //Console.WriteLine(GetMostRecentBooks(db));

            //14.IncreasePrices
            //IncreasePrices(db);

            //15.RemoveBooks
            //RemoveBooks(db);
        }

        //1.AgeRestriction
        public static string GetBooksByAgeRestriction(BookShopContext contex, string command)
        {
            var ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            var books = contex
                .Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToList();

            return string.Join(Environment.NewLine, books);
        }

        //2.GoldenBooks
        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context
                .Books
                .AsEnumerable()
                .Where(b => b.EditionType.ToString() == "Gold"
                            && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, books);
        }

        //3.BooksByPrice
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context
                .Books
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => new
                {
                    bookTitle = b.Title,
                    bookPrice = b.Price
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb
                    .AppendLine($"{book.bookTitle} - ${book.bookPrice:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //4.NotReleasedIn
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(x => x.Title)
                .ToList();

            return string.Join(Environment.NewLine, books);
        }

        //5.BookTitlesByCategory
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var listOfCategories = input
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToLower())
                .ToArray();

            var books = context
                .Books
                .Include(x => x.BookCategories)
                .ThenInclude(x => x.Category)
                .ToArray()
                .Where(b => b.BookCategories
                    .Any(category => listOfCategories.Contains(category.Category.Name.ToLower())))
                .Select(b => b.Title)
                .OrderBy(title => title)
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }

        //6.ReleasedBeforeDate
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var books = context
                .Books
                .Where(b => b.ReleaseDate < DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.CurrentCulture))
                .OrderByDescending(b => b.ReleaseDate)
                .Select(x => new
                {
                    bookTitle = x.Title,
                    editType = x.EditionType,
                    bookPrice = x.Price
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb
                    .AppendLine($"{book.bookTitle} - {book.editType} - ${book.bookPrice:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //7.AuthorSearch
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context
                .Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new
                {
                    fullName = a.FirstName + " " + a.LastName
                })
                .OrderBy(a => a.fullName)
                .ToList();


            return string.Join(Environment.NewLine, authors.Select(a => a.fullName));
        }

        //8.BookSearch
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context
                .Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .OrderBy(b => b.Title)
                .Select(t => t.Title)
                .ToArray();

            return string.Join(Environment.NewLine, books);

        }

        //9.BookSearchByAuthor
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context
                .Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(b => new
                {
                    bookId = b.BookId,
                    bookTitle = b.Title,
                    fullName = b.Author.FirstName + " " + b.Author.LastName
                })
                .OrderBy(b => b.bookId)
                .ToList();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb
                    .AppendLine($"{book.bookTitle} ({book.fullName})");
            }

            return sb.ToString().TrimEnd();
        }

        //10.CountBooks
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books = context
                .Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();

            return books;
        }

        //11.TotalBookCopies
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context
                .Authors
                .Select(x => new
                {
                    fullName = x.FirstName + " " + x.LastName,
                    countOfBooks = x.Books.Select(x => x.Copies).Sum()
                })
                .OrderByDescending(x => x.countOfBooks)
                .ToList();

            var sb = new StringBuilder();

            foreach (var author in authors)
            {
                sb
                    .AppendLine($"{author.fullName} - {author.countOfBooks}");
            }

            return sb.ToString().TrimEnd();
        }

        //12.ProfitByCategory
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context
                .Categories
                .Select(x => new
                {
                    categoryName = x.Name,
                    profit = x.CategoryBooks.Select(x => x.Book.Price * x.Book.Copies).Sum()
                })
                .OrderByDescending(x => x.profit)
                .ToList();

            var sb = new StringBuilder();

            foreach (var c in categories)
            {
                sb
                    .AppendLine($"{c.categoryName} ${c.profit:F2}");
            }

            return sb.ToString().TrimEnd();
        }

        //13.MostRecentBooks
        public static string GetMostRecentBooks(BookShopContext context)
        {
            var books = context
                .Categories
                .Select(x => new
                {
                    categoryName = x.Name,
                    bookTitleAndYear = x.CategoryBooks.Select(cb => new
                    {
                        bookTitle = cb.Book.Title,
                        bookYear = cb.Book.ReleaseDate
                    })
                    .OrderByDescending(x => x.bookYear)
                    .Take(3)
                })
                .OrderBy(x => x.categoryName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var category in books)
            {
                sb
                    .AppendLine($"--{category.categoryName}");

                foreach (var book in category.bookTitleAndYear)
                {
                    sb
                        .AppendLine($"{book.bookTitle} ({book.bookYear.Value.Year})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        //14.IncreasePrices
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context
                .Books
                .Where(x => x.ReleaseDate.Value.Year < 2010)
                .ToList();

            foreach (var book in books)
            {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        //15.RemoveBooks
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context
                .Books
                .Where(x => x.Copies < 4200)
                .ToList();

            var deletedBooks = books.Count();

            context.Books.RemoveRange(books);

            context.SaveChanges();

            return deletedBooks;
        }
    }
}
