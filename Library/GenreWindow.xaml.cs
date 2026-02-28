using System;
using System.Windows;
using Library.Data;
using Library.entity;

namespace Library
{
    public partial class GenreWindow : Window
    {
        private LibraryContext _context;
        private Genre _currentGenre;
        private bool _isEditMode;

        // Конструктор для ДОБАВЛЕНИЯ
        internal GenreWindow(LibraryContext context)
        {
            InitializeComponent();

            _context = context;
            _currentGenre = new Genre();
            _isEditMode = false;

            this.Title = "Добавление жанра";
        }

        // Конструктор для РЕДАКТИРОВАНИЯ
        internal GenreWindow(LibraryContext context, Genre genreToEdit)
        {
            InitializeComponent();

            _context = context;
            _currentGenre = genreToEdit;
            _isEditMode = true;

            this.Title = "Редактирование жанра";

            // Заполняем поля
            NameTextBox.Text = _currentGenre.Name;
            DescriptionTextBox.Text = _currentGenre.Description;
        }

        // Сохранение
        // Сохранение
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверки
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("Введите название жанра", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    NameTextBox.Focus();
                    return;
                }

                if (NameTextBox.Text.Length > 50)
                {
                    MessageBox.Show("Название жанра не должно превышать 50 символов", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    NameTextBox.Focus();
                    return;
                }

                // Заполняем жанр
                _currentGenre.Name = NameTextBox.Text.Trim();
                _currentGenre.Description = DescriptionTextBox.Text?.Trim() ?? "";

                // Добавляем если новый
                if (!_isEditMode)
                {
                    _context.Genres.Add(_currentGenre);
                }

                // Сохраняем
                _context.SaveChanges();

                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Отмена
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}