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
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, что все поля заполнены
                if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
                {
                    MessageBox.Show("Введите название книги");
                    return;
                }

                if (AuthorComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите автора");
                    return;
                }

                if (GenreComboBox.SelectedItem == null)
                {
                    MessageBox.Show("Выберите жанр");
                    return;
                }

                if (string.IsNullOrWhiteSpace(YearTextBox.Text))
                {
                    MessageBox.Show("Введите год издания");
                    return;
                }

                // Заполняем книгу данными из полей
                _currentBook.Title = TitleTextBox.Text.Trim();
                _currentBook.PublishYear = int.Parse(YearTextBox.Text);
                _currentBook.ISBN = IsbnTextBox.Text?.Trim() ?? "";
                _currentBook.QuantityInStock = string.IsNullOrWhiteSpace(QuantityTextBox.Text) ? 0 : int.Parse(QuantityTextBox.Text);

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
                // Если это редактирование - просто сохраняем изменения

                // Сохраняем изменения в базу
                _context.SaveChanges();

                // Закрываем окно с результатом "успешно"
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
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