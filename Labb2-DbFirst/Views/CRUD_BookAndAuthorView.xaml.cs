using System.Collections;
using System.Printing;
using System.Windows;
using DataAccess;
using System.Windows.Controls;
using System.Windows.Documents;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Labb2_DbFirst.Views
{
    /// <summary>
    /// Interaction logic for CRUD_BookAndAuthorView.xaml
    /// </summary>
    public partial class CRUD_BookAndAuthorView : UserControl
    {

        public Book SelectedBook { get; set; }
        public Author SelectedAuthor { get; set; }


        public CRUD_BookAndAuthorView()
        {
            InitializeComponent();
            LoadBooks();
            LoadAuthors();
            LoadGenreComboBox();
        }

        private void LoadBooks()
        {
            BooksView.Items.Clear();
            using var bokhandelsDbContext = new BokhandelDbContext();
            var listOfBooks = bokhandelsDbContext.Books.ToList();

            foreach (var book in listOfBooks)
            {
                BooksView.Items.Add(book);
            }
        }

        private void LoadAuthors()
        {
            AuthorsView.Items.Clear();
            using var bokhandelsDbContext = new BokhandelDbContext();
            var listOfAllAuthors = bokhandelsDbContext.Authors.ToList();

            foreach (var author in listOfAllAuthors)
            {
                AuthorsView.Items.Add(author);
            }
        }

        private void LoadGenreComboBox()
        {
            using var bokhandelsDbContext = new BokhandelDbContext();

            var listOfGenre = bokhandelsDbContext.Genres.ToList();

            foreach (var genre in listOfGenre)
            {
                GenreCoB.Items.Add(genre);
            }
        }

        private List<Author> GetAuthor(BokhandelDbContext dbContext)
        {
            var authorNewBook = new List<Author>();
            foreach (var item in AuthorToBookView.Items)
            {
                if (item is Author authorItem)
                {
                    var existingAuthor = dbContext.Authors.Find(authorItem.Id);
                    if (existingAuthor != null)
                    {
                        authorNewBook.Add(existingAuthor);
                    }
                }
            }
            return authorNewBook;
        }

        private Publicer GetPublisher(BokhandelDbContext dbContext)
        {
            var specifiedPublisher = PublisherTxt.Text;
            var findSpecifiedPublisher = dbContext.Publicers
                                             .FirstOrDefault(p =>
                                                 p.Name.ToLower() == specifiedPublisher.ToLower())
                                         ?? new Publicer
                                         {
                                             Name = specifiedPublisher,
                                             Books = new List<Book>()
                                         };

            if (findSpecifiedPublisher.Id == 0)
            {
                dbContext.Publicers.Add(findSpecifiedPublisher);
                dbContext.SaveChanges();
            }

            return findSpecifiedPublisher;
        }

        private Genre GetGenre(BokhandelDbContext dbContext)
        {

            var selectedGenreId = GenreCoB.SelectedIndex + 1;
            var findSelectedGenre = dbContext.Genres.Find(selectedGenreId);

            return findSelectedGenre;
        }

        private void Books_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedBook = BooksView.SelectedItem as Book;

            if (SelectedBook == null)
            {
                ISBN13Txt.Text = string.Empty;
                TitleTxt.Text = string.Empty;
                LanguageTxt.Text = string.Empty;
                PriceTxt.Text = string.Empty;
                ReleaseDateTxt.Text = string.Empty;
                PublisherTxt.Text = string.Empty;
                GenreCoB.Text = string.Empty;
                AuthorToBookView.Items.Clear();

            }
            else
            {
                ISBN13Txt.Text = SelectedBook.Isbn13;
                TitleTxt.Text = SelectedBook.Title;
                LanguageTxt.Text = SelectedBook.Language;
                PriceTxt.Text = SelectedBook.Price.ToString();
                ReleaseDateTxt.Text = SelectedBook.ReleaseDate.ToString();
                GenreCoB.SelectedIndex = SelectedBook.GenreId.Value - 1;


                using var bokhandelsDbContext = new BokhandelDbContext();

                //Get Publicer
                var publicerId = SelectedBook.PublicerId;
                var booksPublicer = bokhandelsDbContext.Books
                    .Include(b => b.Publicer)
                    .FirstOrDefault(b => b.PublicerId == publicerId)?
                    .Publicer;

                PublisherTxt.Text = booksPublicer.Name;


                //Get Author
                var isbn13 = SelectedBook.Isbn13;
                var authorsForBook = bokhandelsDbContext.Books
                    .Include(b => b.Authors)
                    .FirstOrDefault(b => b.Isbn13 == isbn13)?
                    .Authors
                    .ToList();

                AuthorToBookView.Items.Clear();
                if (authorsForBook != null)
                {
                    foreach (var author in authorsForBook)
                    {
                        AuthorToBookView.Items.Add(author);
                    }
                }
            }
        }

        private void AuthorsView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedAuthor = AuthorsView.SelectedItem as Author;

            if (SelectedAuthor == null)
            {
                FirstNameTxt.Text = string.Empty;
                LastNameTxt.Text = string.Empty;
                DateOfBirth.Text = string.Empty;
            }
            else
            {
                FirstNameTxt.Text = SelectedAuthor.FirstName;
                LastNameTxt.Text = SelectedAuthor.LastName;
                DateOfBirth.Text = SelectedAuthor.DateOfBirth.ToString();
            }
        }

        private void UpdateAddAuthorsBtn_OnClick(object sender, RoutedEventArgs e)
        {
            using var bokhandelsDbContext = new BokhandelDbContext();

            if (DateOnly.TryParse(DateOfBirth.Text, out var parsedDate))
            {
                if (SelectedAuthor != null)
                {
                    var listOfAuthors = bokhandelsDbContext.Authors.ToList();

                    var selectedAuthor = listOfAuthors
                        .FirstOrDefault(a => a.Id == SelectedAuthor.Id);

                    selectedAuthor.FirstName = FirstNameTxt.Text;
                    selectedAuthor.LastName = LastNameTxt.Text;
                    selectedAuthor.DateOfBirth = parsedDate;

                    bokhandelsDbContext.Authors.Update(selectedAuthor);
                }
                else
                {
                    bokhandelsDbContext.Authors.Add
                    (new Author
                        {
                            FirstName = FirstNameTxt.Text,
                            LastName = LastNameTxt.Text,
                            DateOfBirth = parsedDate
                        }
                    );
                }

                bokhandelsDbContext.SaveChanges();
                LoadAuthors();
            }
            else
            {
                MessageBox.Show("Insert correct date", "Invalid Birth of date", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private void DeleteAuthorsBtn_OnClick(object sender, RoutedEventArgs e)
        {
            using var bokhandelsDbContext = new BokhandelDbContext();

            if (SelectedAuthor == null)
            {
                MessageBox.Show("Select an author to remove", "Missing information", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                var authorToRemove = bokhandelsDbContext.Authors
                    .Include(a => a.BookIsbns)
                    .FirstOrDefault(a => a.Id == SelectedAuthor.Id);

                if (authorToRemove.BookIsbns.Any())
                {
                    MessageBox.Show($"Author is associated with one or more books and cannot be removed");
                }
                else if (SelectedAuthor != null)
                {
                    bokhandelsDbContext.Authors.Remove(authorToRemove);
                    bokhandelsDbContext.SaveChanges();
                    LoadAuthors();
                }
            }
        }

        private void AddAuthorToBook_OnClick(object sender, RoutedEventArgs e)
        {
            if (SelectedAuthor != null)
            {
                AuthorToBookView.Items.Add(SelectedAuthor);
            }
            else
            {
                MessageBox.Show($"Select an author");
            }
        }

        private void DeleteAuthorToBook_OnClick(object sender, RoutedEventArgs e)
        {
            var removeAuthor = AuthorToBookView.SelectedItem;

            if (AuthorToBookView.SelectedItem != null)
            {
                AuthorToBookView.Items.Remove(removeAuthor);
            }
            else
            {
                MessageBox.Show($"Select an author");
            }
        }

        private void UpdateAddBooksBtn_OnClick(object sender, RoutedEventArgs e)
        {
            using var bokhandelsDbContext = new BokhandelDbContext();

            if (SelectedBook == null)
            {

                if (DateOnly.TryParse(ReleaseDateTxt.Text, out var convertDate) && decimal.TryParse(PriceTxt.Text, out var convertPrice))
                {
                    if (ISBN13Txt.Text.Length == 13)
                    {
                        var authorNewBook = GetAuthor(bokhandelsDbContext);

                        var findSpecifiedPublisher = GetPublisher(bokhandelsDbContext);

                        var findSelectedGenre = GetGenre(bokhandelsDbContext);

                        var newBook = new Book
                        {
                            Isbn13 = ISBN13Txt.Text,
                            Title = TitleTxt.Text,
                            Language = LanguageTxt.Text,
                            Price = convertPrice,
                            ReleaseDate = convertDate,
                            Publicer = findSpecifiedPublisher,
                            Genre = findSelectedGenre,
                            Authors = authorNewBook
                        };

                        bokhandelsDbContext.Books.Add(newBook);
                    }
                    else
                    {
                        MessageBox.Show($"ISBN13 needs to be 13 characters");
                    }
                    
                }
            }
            else if (SelectedBook != null)
            {

                if (DateOnly.TryParse(ReleaseDateTxt.Text, out var parsedDate) && decimal.TryParse(PriceTxt.Text, out var parsedPrice))
                {
                    var selectedBook = bokhandelsDbContext.Books
                        .Include(b => b.Authors)
                        .FirstOrDefault(b => b.Isbn13 == SelectedBook.Isbn13);

                    if (!bokhandelsDbContext.Books.Any(b => b.Isbn13 == ISBN13Txt.Text))
                    {
                        MessageBox.Show($"You cant change ISBN13, delete and create a new book");
                    }
                    else
                    {
                        
                        var authorNewBook = GetAuthor(bokhandelsDbContext);

                        var findSpecifiedPublisher = GetPublisher(bokhandelsDbContext);

                        var findSelectedGenre = GetGenre(bokhandelsDbContext);

                        selectedBook.Title = TitleTxt.Text;
                        selectedBook.Language = LanguageTxt.Text;
                        selectedBook.Price = parsedPrice;
                        selectedBook.ReleaseDate = parsedDate;
                        selectedBook.Publicer = findSpecifiedPublisher;
                        selectedBook.Genre = findSelectedGenre;

                        selectedBook.Authors.Clear();
                        foreach (var newAuthor in authorNewBook)
                        {
                            selectedBook.Authors.Add(newAuthor);
                        }
                    }
                }

            }
            bokhandelsDbContext.SaveChanges();
            LoadBooks();
        }

        private void DeleteBooksBtn_OnClick(object sender, RoutedEventArgs e)
        {
            using var bokhandelsDbContext = new BokhandelDbContext();

            if (SelectedBook == null)
            {
                MessageBox.Show($"Select a book to remove");
            }
            else
            {
                var removeBook = bokhandelsDbContext.Books
                    .Include(a => a.Authors)
                    .Include(book => book.Stocks)
                    .FirstOrDefault(b => b.Isbn13 == SelectedBook.Isbn13);

                if (removeBook.Stocks.Any(s => s.Isbnid == removeBook.Isbn13))
                {
                    MessageBox.Show($"This title is in stock and cant be deleted");
                }
                else
                {
                    removeBook.Authors.Clear();

                    bokhandelsDbContext.Books.Remove(removeBook);

                    bokhandelsDbContext.SaveChanges();
                    LoadBooks();
                }
            }
        }
    }
}
