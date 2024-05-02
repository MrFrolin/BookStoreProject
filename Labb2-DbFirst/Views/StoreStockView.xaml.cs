using System.Windows;
using System.Windows.Controls;
using DataAccess;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Labb2_DbFirst.Views
{
    /// <summary>
    /// Interaction logic for StoreStockView.xaml
    /// </summary>
    public partial class StoreStockView : UserControl
    {
        private Store? SelectedStore { get; set; }

        public Book? BookViewSelectedBook { get; set; }

        public Stock? StoreStockSelectedBook { get; set; } = new();


        public StoreStockView()
        {
            InitializeComponent();
            LoadBooks();
            LoadStoresToComboBox();
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

        private void LoadStoresToComboBox()
        {
            using var bokhandelsDbContext = new BokhandelDbContext();
            var listOfStores = bokhandelsDbContext.Stores.ToList();

            foreach (var store in listOfStores)
            {
                StoresComboBox.Items.Add(store);
            }
        }

        private void LoadSelectedStoreStock()
        {

            StoreStock.Items.Clear();
            using var bokhandelsDbContext = new BokhandelDbContext();
            var listOfBooksInSelectedStore = bokhandelsDbContext.Stocks
                .Include(s => s.Store)
                .Include(s => s.Isbn)
                .Where(s => s.Store.Id == SelectedStore.Id).ToList();

            foreach (var book in listOfBooksInSelectedStore)
            {
                StoreStock.Items.Add(book);
            }
        }

        private void StoresComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedStore = StoresComboBox.SelectedItem as Store;
            LoadSelectedStoreStock();
        }

        private void BooksView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BookViewSelectedBook = BooksView.SelectedItem as Book;
        }

        private void StoreStock_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StoreStockSelectedBook = StoreStock.SelectedItem as Stock;

            if (StoreStockSelectedBook == null)
            {
                StockBox.Text = string.Empty;
            }
            else
            {
                StockBox.Text = StoreStockSelectedBook.StockBalance.ToString();
            }
        }

        private void AddToStoreBtn_OnClick(object sender, RoutedEventArgs e)
        {

            using var bokhandelsDbContext = new BokhandelDbContext();

            if (int.TryParse(QtyBox.Text, out var qtyBoxIsInt) && SelectedStore != null && BookViewSelectedBook != null)
            {
                var listOfStoreStock = bokhandelsDbContext.Stocks
                    .Where(s => s.StoreId == SelectedStore.Id).ToList();

                if (listOfStoreStock.Any(s => s.Isbnid == BookViewSelectedBook.Isbn13))
                {
                    var selectedBookInStoreStock = listOfStoreStock.FirstOrDefault(b => b.Isbnid == BookViewSelectedBook.Isbn13);
                    selectedBookInStoreStock.StockBalance += qtyBoxIsInt;
                    bokhandelsDbContext.Stocks.Update(selectedBookInStoreStock);
                }
                else
                {
                    bokhandelsDbContext.Stocks.Add
                    (
                        new Stock
                        {
                            Isbnid = BookViewSelectedBook.Isbn13,
                            StoreId = SelectedStore.Id,
                            StockBalance = qtyBoxIsInt
                        }
                    );
                }
                bokhandelsDbContext.SaveChanges();
                LoadSelectedStoreStock();
            }
            else
            {
                MessageBox.Show("Quantity, Store and Book needs to be selected", "Missing information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RemoveFromSelectedStore_OnClick(object sender, RoutedEventArgs e)
        {
            using var bokhandelsDbContext = new BokhandelDbContext();

            if  (SelectedStore != null && StoreStockSelectedBook != null)
            {
                var stockToRemove = bokhandelsDbContext.Stocks
                    .FirstOrDefault(b => b.StoreId == StoreStockSelectedBook.StoreId &&
                                b.Isbnid == StoreStockSelectedBook.Isbnid);

                bokhandelsDbContext.Stocks.Remove(stockToRemove);

                bokhandelsDbContext.SaveChanges();
                LoadSelectedStoreStock();
            }
            else
            {
                MessageBox.Show("You need to select a store and book", "Missing information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void StockUpdBtn_OnClick(object sender, RoutedEventArgs e)
        {
            
            using var bokhandelsDbContext = new BokhandelDbContext();

            if (SelectedStore != null && StoreStockSelectedBook != null && int.TryParse(StockBox.Text, out var qtyResult))
            {
                var listOfStoreStock = bokhandelsDbContext.Stocks
                    .Where(s => s.StoreId == SelectedStore.Id).ToList();

                var selectedObject = listOfStoreStock
                    .FirstOrDefault(s => s.Isbnid == StoreStockSelectedBook.Isbnid);

                selectedObject.StockBalance = qtyResult;

                bokhandelsDbContext.Stocks.Update(selectedObject);

                bokhandelsDbContext.SaveChanges();
                StockBox.Text = string.Empty;
                LoadSelectedStoreStock();
            }
            else
            {
                MessageBox.Show("fel");
            }
        }

        private void LoadBooksInStock_OnClick(object sender, RoutedEventArgs e)
        {
            LoadBooks();

            if (StoresComboBox.Text !="-- Select Store --")
            {
                LoadSelectedStoreStock();
            }
        }
    }
}
