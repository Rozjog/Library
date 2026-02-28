using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.entity;

namespace Library
{
    public partial class MainWindow : Window
    {
        private LibraryContext _context;

        public MainWindow()
        {
            InitializeComponent();

            _context = new LibraryContext();
            _context.Database.EnsureCreated();

            LoadData();
            LoadFilters();
        }

        // Загрузка книг в таблицу
        private void LoadData()
        {
            var books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .ToList();

            BooksDataGrid.ItemsSource = books;
            TotalBooksText.Text = $"Всего книг: {books.Count}";
        }

        // Загрузка авторов и жанров в фильтры
        private void LoadFilters()
        {
            // Загружаем авторов
            var authors = _context.Authors.ToList();
            authors.Insert(0, new Author { Id = 0, LastName = "Все авторы" });
            AuthorFilterComboBox.ItemsSource = authors;
            AuthorFilterComboBox.SelectedValuePath = "Id";
            AuthorFilterComboBox.DisplayMemberPath = "LastName";

            // Загружаем жанры
            var genres = _context.Genres.ToList();
            genres.Insert(0, new Genre { Id = 0, Name = "Все жанры" });
            GenreFilterComboBox.ItemsSource = genres;
            GenreFilterComboBox.SelectedValuePath = "Id";
            GenreFilterComboBox.DisplayMemberPath = "Name";
        }

        // Добавление книги
        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            var bookWindow = new BookWindow(_context);
            bookWindow.Owner = this;

            if (bookWindow.ShowDialog() == true)
            {
                LoadData();
                LoadFilters();  // обновляем фильтры (мог появиться новый автор/жанр)
            }
        }

        // Редактирование книги
        private void EditBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                var bookWindow = new BookWindow(_context, selectedBook);
                bookWindow.Owner = this;

                if (bookWindow.ShowDialog() == true)
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Выберите книгу для редактирования");
            }
        }

        // Удаление книги
        private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                var result = MessageBox.Show($"Удалить книгу '{selectedBook.Title}'?",
                                             "Подтверждение",
                                             MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Books.Remove(selectedBook);
                        _context.SaveChanges();

                        LoadData();

                        MessageBox.Show("Книга удалена");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите книгу для удаления");
            }
        }

        // Управление авторами
        private void ManageAuthorsButton_Click(object sender, RoutedEventArgs e)
        {
            var authorsWindow = new AuthorsWindow(_context);
            authorsWindow.Owner = this;
            authorsWindow.ShowDialog();

            // После закрытия окна авторов перезагружаем данные
            LoadData();
            LoadFilters();
        }

        // Управление жанрами (пока заглушка)
        private void ManageGenresButton_Click(object sender, RoutedEventArgs e)
        {
            var genresWindow = new GenresWindow(_context);
            genresWindow.Owner = this;
            genresWindow.ShowDialog();

            // После закрытия окна жанров перезагружаем данные
            LoadData();
            LoadFilters();  // обновляем список жанров в фильтре
        }

        private void SearchTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Применяем фильтры при каждом изменении текста
            ApplyFilters();
        }

        // Применение фильтров (пока заглушка)
        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .AsQueryable();

            // Поиск по названию
            if (!string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string searchText = SearchTextBox.Text.ToLower();
                query = query.Where(b => b.Title.ToLower().Contains(searchText));
            }

            // Фильтр по автору (если выбран не "Все авторы")
            if (AuthorFilterComboBox.SelectedItem is Author selectedAuthor && selectedAuthor.Id > 0)
            {
                query = query.Where(b => b.AuthorId == selectedAuthor.Id);
            }

            // Фильтр по жанру (если выбран не "Все жанры")
            if (GenreFilterComboBox.SelectedItem is Genre selectedGenre && selectedGenre.Id > 0)
            {
                query = query.Where(b => b.GenreId == selectedGenre.Id);
            }

            var filteredBooks = query.ToList();
            BooksDataGrid.ItemsSource = filteredBooks;
            TotalBooksText.Text = $"Всего книг: {filteredBooks.Count}";
        }

        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTextBox.Text = "";

            // Сбрасываем на "Все авторы" и "Все жанры"
            AuthorFilterComboBox.SelectedIndex = 0;
            GenreFilterComboBox.SelectedIndex = 0;

            // Применяем фильтры (покажет все книги)
            ApplyFilters();
        }
    }
}