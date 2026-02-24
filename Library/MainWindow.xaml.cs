using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.entity;

namespace Library
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LibraryContext context;
        public MainWindow()
        {
            InitializeComponent();
            context = new LibraryContext();
            context.Database.EnsureCreated();

            LoadData();
            LoadFilters();
        }

        private void LoadData()
        {
            var books = context.Books.Include(b => b.Author).Include(b => b.Genre).ToList();

            BooksDataGrid.ItemsSource = books;

            TotalBooksText.Text = $"Всего книг: {books.Count}";
        }

        private void LoadFilters()
        {
            var authors = context.Authors.ToList();
            AuthorFilterComboBox.ItemsSource = authors;

            var genres = context.Genres.ToList();
            GenreFilterComboBox.ItemsSource = genres;
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаем окно добавления книги
            var bookWindow = new BookWindow(context);

            // Показываем окно и ждем, пока его закроют
            bookWindow.Owner = this; // главное окно - владелец

            if (bookWindow.ShowDialog() == true)
            {
                // Если пользователь нажал "Сохранить" - перезагружаем список книг
                LoadData();
            }
        }

        private void EditBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                MessageBox.Show($"Редактирование книги: {selectedBook.Title}");
            }
            else
            {
                MessageBox.Show("Выберите книгу для редактирования");
            }
        }

        private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                var result = MessageBox.Show($"Удалить книгу '{selectedBook.Title}'?",
                                             "Подтверждение",
                                             MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    MessageBox.Show("Удаление пока не реализовано");
                }
            }
            else
            {
                MessageBox.Show("Выберите книгу для удаления");
            }
        }

        private void ManageAuthorsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Здесь будет окно управления авторами");
        }

        private void ManageGenresButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Здесь будет окно управления жанрами");
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Здесь будет фильтрация");
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Здесь будет сброс фильтров");
        }

        private void BooksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }


    }
}