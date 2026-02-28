using System;
using System.Windows;
using Library.Data;
using Library.entity;

namespace Library
{
    public partial class AuthorWindow : Window
    {
        private LibraryContext _context;
        private Author _currentAuthor;
        private bool _isEditMode;

        internal AuthorWindow(LibraryContext context)
        {
            InitializeComponent();

            _context = context;
            _currentAuthor = new Author();
            _isEditMode = false;

            this.Title = "Добавление автора";
            BirthDatePicker.SelectedDate = DateTime.Today;
        }

        internal AuthorWindow(LibraryContext context, Author authorToEdit)
        {
            InitializeComponent();

            _context = context;
            _currentAuthor = authorToEdit;
            _isEditMode = true;

            this.Title = "Редактирование автора";

            FirstNameTextBox.Text = _currentAuthor.FirstName;
            LastNameTextBox.Text = _currentAuthor.LastName;
            BirthDatePicker.SelectedDate = _currentAuthor.BirthDate;
            CountryTextBox.Text = _currentAuthor.Country;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
                {
                    MessageBox.Show("Введите имя автора", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    FirstNameTextBox.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
                {
                    MessageBox.Show("Введите фамилию автора", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    LastNameTextBox.Focus();
                    return;
                }

                if (BirthDatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Выберите дату рождения", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    BirthDatePicker.Focus();
                    return;
                }

                if (BirthDatePicker.SelectedDate > DateTime.Today)
                {
                    MessageBox.Show("Дата рождения не может быть в будущем", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    BirthDatePicker.Focus();
                    return;
                }

                if (BirthDatePicker.SelectedDate < DateTime.Today.AddYears(-120))
                {
                    MessageBox.Show("Возраст автора не может быть больше 120 лет", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    BirthDatePicker.Focus();
                    return;
                }


                _currentAuthor.FirstName = FirstNameTextBox.Text.Trim();
                _currentAuthor.LastName = LastNameTextBox.Text.Trim();
                _currentAuthor.BirthDate = BirthDatePicker.SelectedDate.Value;
                _currentAuthor.Country = CountryTextBox.Text?.Trim() ?? "";

                if (!_isEditMode)
                {
                    _context.Authors.Add(_currentAuthor);
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