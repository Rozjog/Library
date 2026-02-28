using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.entity;

namespace Library
{
    public partial class GenresWindow : Window
    {
        private LibraryContext _context;

        internal GenresWindow(LibraryContext context)
        {
            InitializeComponent();

            _context = context;

            // Загружаем жанры вместе с их книгами (чтобы показать количество)
            LoadGenres();
        }

        private void LoadGenres()
        {
            var genres = _context.Genres
                .Include(g => g.Books)  // загружаем книги, чтобы посчитать количество
                .ToList();

            GenresDataGrid.ItemsSource = genres;
        }

        // Добавить жанр
        private void AddGenreButton_Click(object sender, RoutedEventArgs e)
        {
            var genreWindow = new GenreWindow(_context);
            genreWindow.Owner = this;

            if (genreWindow.ShowDialog() == true)
            {
                LoadGenres();
            }
        }

        // Редактировать жанр
        private void EditGenreButton_Click(object sender, RoutedEventArgs e)
        {
            if (GenresDataGrid.SelectedItem is Genre selectedGenre)
            {
                var genreWindow = new GenreWindow(_context, selectedGenre);
                genreWindow.Owner = this;

                if (genreWindow.ShowDialog() == true)
                {
                    LoadGenres();
                }
            }
            else
            {
                MessageBox.Show("Выберите жанр для редактирования");
            }
        }

        // Удалить жанр
        private void DeleteGenreButton_Click(object sender, RoutedEventArgs e)
        {
            if (GenresDataGrid.SelectedItem is Genre selectedGenre)
            {
                // Проверяем, есть ли у жанра книги
                if (selectedGenre.Books != null && selectedGenre.Books.Any())
                {
                    MessageBox.Show("Нельзя удалить жанр, в котором есть книги. Сначала удалите все книги этого жанра.");
                    return;
                }

                var result = MessageBox.Show($"Удалить жанр '{selectedGenre.Name}'?",
                                             "Подтверждение",
                                             MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Genres.Remove(selectedGenre);
                        _context.SaveChanges();
                        LoadGenres();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите жанр для удаления");
            }
        }

        // Закрыть окно
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}