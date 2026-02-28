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
        // Ссылка на контекст базы данных
        private LibraryContext _context;

        // Книга, которую мы добавляем или редактируем
        private Book _currentBook;

        // Флаг: true - редактирование, false - добавление
        private bool _isEditMode;

        // Конструктор для режима ДОБАВЛЕНИЯ
        internal BookWindow(LibraryContext context)
        {
            InitializeComponent();

            _context = context;
            _currentBook = new Book();
            _isEditMode = false;

            // Меняем заголовок окна
            this.Title = "Добавление книги";

            // Загружаем списки авторов и жанров
            LoadComboBoxes();
        }

        // Конструктор для режима РЕДАКТИРОВАНИЯ
        internal BookWindow(LibraryContext context, Book bookToEdit)
        {
            InitializeComponent();

            _context = context;
            _currentBook = bookToEdit;
            _isEditMode = true;

            // Меняем заголовок окна
            this.Title = "Редактирование книги";

            // Загружаем списки авторов и жанров
            LoadComboBoxes();

            // Заполняем поля данными из редактируемой книги
            LoadBookData();
        }

        // Загрузка списков авторов и жанров
        private void LoadComboBoxes()
        {
            // Загружаем всех авторов из базы
            var authors = _context.Authors.ToList();
            AuthorComboBox.ItemsSource = authors;

            // Загружаем все жанры из базы
            var genres = _context.Genres.ToList();
            GenreComboBox.ItemsSource = genres;
        }

        // Заполнение полей данными книги (для режима редактирования)
        private void LoadBookData()
        {
            TitleTextBox.Text = _currentBook.Title;
            YearTextBox.Text = _currentBook.PublishYear.ToString();
            IsbnTextBox.Text = _currentBook.ISBN;
            QuantityTextBox.Text = _currentBook.QuantityInStock.ToString();

            // Выбираем нужного автора в списке
            if (_currentBook.AuthorId > 0)
            {
                AuthorComboBox.SelectedValue = _currentBook.AuthorId;
            }

            // Выбираем нужный жанр в списке
            if (_currentBook.GenreId > 0)
            {
                GenreComboBox.SelectedValue = _currentBook.GenreId;
            }
        }

        // Нажатие на кнопку "Сохранить"
        // Нажатие на кнопку "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // === ВАЛИДАЦИЯ ===

                // 1. Проверка названия
                if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
                {
                    MessageBox.Show("Введите название книги", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    TitleTextBox.Focus();
                    return;
                }

                // 2. Проверка выбора автора
                if (AuthorComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите автора", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    AuthorComboBox.Focus();
                    return;
                }

                // 3. Проверка выбора жанра
                if (GenreComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите жанр", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    GenreComboBox.Focus();
                    return;
                }

                // 4. Проверка года (число и диапазон)
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

                // 5. Проверка количества (число и неотрицательное)
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

                // === ВСЕ ПРОВЕРКИ ПРОЙДЕНЫ ===

                // Заполняем книгу данными из полей
                _currentBook.Title = TitleTextBox.Text.Trim();
                _currentBook.PublishYear = year;
                _currentBook.ISBN = IsbnTextBox.Text?.Trim() ?? "";
                _currentBook.QuantityInStock = quantity;

                // Получаем выбранного автора и жанр
                var selectedAuthor = AuthorComboBox.SelectedItem as Author;
                var selectedGenre = GenreComboBox.SelectedItem as Genre;

                _currentBook.AuthorId = selectedAuthor.Id;
                _currentBook.GenreId = selectedGenre.Id;

                // Если это добавление новой книги
                if (!_isEditMode)
                {
                    _context.Books.Add(_currentBook);
                }

                // Сохраняем изменения в базу
                _context.SaveChanges();

                // Закрываем окно с результатом "успешно"
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Нажатие на кнопку "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Закрываем окно с результатом "отмена"
            this.DialogResult = false;
            this.Close();
        }
    }
}