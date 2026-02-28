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
        internal GenreWindow(LibraryContext context)
        {
            InitializeComponent();

            _context = context;
            _currentGenre = new Genre();
            _isEditMode = false;

            this.Title = "Добавление жанра";
        }
        internal GenreWindow(LibraryContext context, Genre genreToEdit)
        {
            InitializeComponent();

            _context = context;
            _currentGenre = genreToEdit;
            _isEditMode = true;

            this.Title = "Редактирование жанра";
            NameTextBox.Text = _currentGenre.Name;
            DescriptionTextBox.Text = _currentGenre.Description;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
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

                _currentGenre.Name = NameTextBox.Text.Trim();
                _currentGenre.Description = DescriptionTextBox.Text?.Trim() ?? "";
                if (!_isEditMode)
                {
                    _context.Genres.Add(_currentGenre);
                }

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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}