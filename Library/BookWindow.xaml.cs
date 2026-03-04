using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.entity;

namespace Library
{
    public partial class BookWindow : Window
    {
        private LibraryContext _context;
        private Book _currentBook;
        private bool _isEditMode;

        internal BookWindow(LibraryContext context)
        {
            InitializeComponent();

            _context = context;
            _currentBook = new Book();
            _isEditMode = false;

            this.Title = "Добавление книги";
            LoadComboBoxes();
        }

        internal BookWindow(LibraryContext context, Book bookToEdit)
        {
            InitializeComponent();

            _context = context;
            _currentBook = bookToEdit;
            _isEditMode = true;

            this.Title = "Редактирование книги";

            LoadComboBoxes();
            LoadBookData();
        }

        private void LoadComboBoxes()
        {
            try
            {
                var authors = _context.Authors.ToList();
                AuthorsListBox.ItemsSource = authors;
                AuthorsListBox.DisplayMemberPath = "LastName";

                var genres = _context.Genres.ToList();
                GenresListBox.ItemsSource = genres;
                GenresListBox.DisplayMemberPath = "Name";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadBookData()
        {
            try
            {
                TitleTextBox.Text = _currentBook.Title;
                YearTextBox.Text = _currentBook.PublishYear.ToString();
                IsbnTextBox.Text = _currentBook.ISBN;
                QuantityTextBox.Text = _currentBook.QuantityInStock.ToString();

                if (_isEditMode && _currentBook.Id > 0)
                {

                    _context.Entry(_currentBook).Collection(b => b.BookAuthors).Load();
                    var selectedAuthorIds = _currentBook.BookAuthors.Select(ba => ba.AuthorId).ToList();

                    foreach (var item in AuthorsListBox.Items)
                    {
                        var author = item as Author;
                        if (author != null && selectedAuthorIds.Contains(author.Id))
                        {
                            AuthorsListBox.SelectedItems.Add(item);
                        }
                    }

                    _context.Entry(_currentBook).Collection(b => b.BookGenres).Load();
                    var selectedGenreIds = _currentBook.BookGenres.Select(bg => bg.GenreId).ToList();

                    foreach (var item in GenresListBox.Items)
                    {
                        var genre = item as Genre;
                        if (genre != null && selectedGenreIds.Contains(genre.Id))
                        {
                            GenresListBox.SelectedItems.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных книги: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
                {
                    MessageBox.Show("Введите название книги", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    TitleTextBox.Focus();
                    return;
                }

                if (AuthorsListBox.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Выберите хотя бы одного автора", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    AuthorsListBox.Focus();
                    return;
                }

                if (GenresListBox.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Выберите хотя бы один жанр", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    GenresListBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(YearTextBox.Text))
                {
                    MessageBox.Show("Введите год издания", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    YearTextBox.Focus();
                    return;
                }

                if (!int.TryParse(YearTextBox.Text, out int year))
                {
                    MessageBox.Show("Год должен быть числом", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    YearTextBox.SelectAll();
                    YearTextBox.Focus();
                    return;
                }

                if (year < 1400 || year > DateTime.Now.Year + 5)
                {
                    MessageBox.Show($"Год должен быть от 1400 до {DateTime.Now.Year + 5}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    YearTextBox.SelectAll();
                    YearTextBox.Focus();
                    return;
                }

                if (!string.IsNullOrWhiteSpace(IsbnTextBox.Text))
                {
                    string isbnClean = IsbnTextBox.Text.Replace("-", "").Replace(" ", "");

                    if (!isbnClean.All(char.IsDigit))
                    {
                        MessageBox.Show("ISBN может содержать только цифры, дефисы и пробелы",
                                       "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        IsbnTextBox.Focus();
                        return;
                    }
                }

                    int quantity = 0;
                if (!string.IsNullOrWhiteSpace(QuantityTextBox.Text))
                {
                    if (!int.TryParse(QuantityTextBox.Text, out quantity))
                    {
                        MessageBox.Show("Количество должно быть числом", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        QuantityTextBox.SelectAll();
                        QuantityTextBox.Focus();
                        return;
                    }

                    if (quantity < 0)
                    {
                        MessageBox.Show("Количество не может быть отрицательным", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        QuantityTextBox.SelectAll();
                        QuantityTextBox.Focus();
                        return;
                    }
                }

                _currentBook.Title = TitleTextBox.Text.Trim();
                _currentBook.PublishYear = year;
                _currentBook.ISBN = IsbnTextBox.Text?.Trim() ?? "";
                _currentBook.QuantityInStock = quantity;

                var selectedAuthors = AuthorsListBox.SelectedItems.Cast<Author>().ToList();
                var selectedGenres = GenresListBox.SelectedItems.Cast<Genre>().ToList();

                if (!_isEditMode)
                {
                    _context.Books.Add(_currentBook);
                    _context.SaveChanges();

                    foreach (var author in selectedAuthors)
                    {
                        _context.BookAuthors.Add(new BookAuthor
                        {
                            BookId = _currentBook.Id,
                            AuthorId = author.Id
                        });
                    }

                    foreach (var genre in selectedGenres)
                    {
                        _context.BookGenres.Add(new BookGenre
                        {
                            BookId = _currentBook.Id,
                            GenreId = genre.Id
                        });
                    }
                }
                else
                {
                    var oldAuthors = _context.BookAuthors.Where(ba => ba.BookId == _currentBook.Id);
                    _context.BookAuthors.RemoveRange(oldAuthors);

                    foreach (var author in selectedAuthors)
                    {
                        _context.BookAuthors.Add(new BookAuthor
                        {
                            BookId = _currentBook.Id,
                            AuthorId = author.Id
                        });
                    }

                    var oldGenres = _context.BookGenres.Where(bg => bg.BookId == _currentBook.Id);
                    _context.BookGenres.RemoveRange(oldGenres);

                    foreach (var genre in selectedGenres)
                    {
                        _context.BookGenres.Add(new BookGenre
                        {
                            BookId = _currentBook.Id,
                            GenreId = genre.Id
                        });
                    }
                }

                _context.SaveChanges();

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}