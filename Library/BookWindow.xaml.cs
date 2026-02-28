using Library.Data;
using Library.entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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
            var authors = _context.Authors.ToList();
            AuthorComboBox.ItemsSource = authors;

            var genres = _context.Genres.ToList();
            GenreComboBox.ItemsSource = genres;
        }

        private void LoadBookData()
        {
            TitleTextBox.Text = _currentBook.Title;
            YearTextBox.Text = _currentBook.PublishYear.ToString();
            IsbnTextBox.Text = _currentBook.ISBN;
            QuantityTextBox.Text = _currentBook.QuantityInStock.ToString();

            if (_currentBook.AuthorId > 0)
            {
                AuthorComboBox.SelectedValue = _currentBook.AuthorId;
            }
            if (_currentBook.GenreId > 0)
            {
                GenreComboBox.SelectedValue = _currentBook.GenreId;
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

                if (AuthorComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите автора", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    AuthorComboBox.Focus();
                    return;
                }

                if (GenreComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите жанр", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    GenreComboBox.Focus();
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

                var selectedAuthor = AuthorComboBox.SelectedItem as Author;
                var selectedGenre = GenreComboBox.SelectedItem as Genre;

                _currentBook.AuthorId = selectedAuthor.Id;
                _currentBook.GenreId = selectedGenre.Id;

                if (!_isEditMode)
                {
                    _context.Books.Add(_currentBook);
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