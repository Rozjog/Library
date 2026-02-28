using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Library.Data;
using Library.entity;

namespace Library
{
    public partial class AuthorsWindow : Window
    {
        private LibraryContext _context;

        internal AuthorsWindow(LibraryContext context)
        {
            InitializeComponent();

            _context = context;

            // Загружаем авторов вместе с их книгами (чтобы показать количество)
            LoadAuthors();
        }

        private void LoadAuthors()
        {
            var authors = _context.Authors
                .Include(a => a.Books)  // загружаем книги, чтобы посчитать количество
                .ToList();

            AuthorsDataGrid.ItemsSource = authors;
        }

        // Добавить автора
        private void AddAuthorButton_Click(object sender, RoutedEventArgs e)
        {
            var authorWindow = new AuthorWindow(_context);
            authorWindow.Owner = this;

            if (authorWindow.ShowDialog() == true)
            {
                LoadAuthors();  // перезагружаем список
            }
        }

        // Редактировать автора
        private void EditAuthorButton_Click(object sender, RoutedEventArgs e)
        {
            if (AuthorsDataGrid.SelectedItem is Author selectedAuthor)
            {
                var authorWindow = new AuthorWindow(_context, selectedAuthor);
                authorWindow.Owner = this;

                if (authorWindow.ShowDialog() == true)
                {
                    LoadAuthors();  // перезагружаем
                }
            }
            else
            {
                MessageBox.Show("Выберите автора для редактирования");
            }
        }

        // Удалить автора
        private void DeleteAuthorButton_Click(object sender, RoutedEventArgs e)
        {
            if (AuthorsDataGrid.SelectedItem is Author selectedAuthor)
            {
                // Проверяем, есть ли у автора книги
                if (selectedAuthor.Books != null && selectedAuthor.Books.Any())
                {
                    MessageBox.Show("Нельзя удалить автора, у которого есть книги. Сначала удалите все его книги.");
                    return;
                }

                var result = MessageBox.Show($"Удалить автора {selectedAuthor.FirstName} {selectedAuthor.LastName}?",
                                             "Подтверждение",
                                             MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _context.Authors.Remove(selectedAuthor);
                        _context.SaveChanges();
                        LoadAuthors();  // перезагружаем
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Выберите автора для удаления");
            }
        }

        // Закрыть окно
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}