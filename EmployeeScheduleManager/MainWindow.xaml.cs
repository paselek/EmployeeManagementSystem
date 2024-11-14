using EmployeeScheduleManager.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmployeeScheduleManager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly FirestoreTest _firestoreTest;
		private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
		{
			AddEmployeeWindow addEmployeeWindow = new AddEmployeeWindow();
			bool? result = addEmployeeWindow.ShowDialog();
			if (result == true)
			{
				// Refresh the employee list if an employee was added
				RefreshEmployeeList_Click(sender, e);
			}
		}
		public MainWindow()
		{
			_firestoreTest = new FirestoreTest();

			InitializeComponent();
		}

		private async void RefreshEmployeeList_Click(object sender, RoutedEventArgs e)
		{
			// Pobieranie pracowników z Firestore
			var employees = await _firestoreTest.GetEmployeesAsync();

			// Przypisanie listy pracowników do DataGrid
			EmployeeListDataGrid.ItemsSource = employees;
		}

		private async void DeleteEmployeeButton_Click(object sender, RoutedEventArgs e)
		{
			// Ensure an employee is selected
			if (EmployeeListDataGrid.SelectedItem is Employee selectedEmployee)
			{
				// Confirm deletion
				var result = MessageBox.Show($"Czy jesteś pewien że chcesz usunąć pracownika: {selectedEmployee.imie} {selectedEmployee.nazwisko}?",
											  "Potwierdź usunięcie", MessageBoxButton.YesNo, MessageBoxImage.Warning);

				if (result == MessageBoxResult.Yes)
				{
					try
					{

						// Delete the selected employee
						await _firestoreTest.DeleteEmployeeAsync(selectedEmployee.Id);

						// Refresh the employee list
						RefreshEmployeeList_Click(sender, e);

						MessageBox.Show("Pracownik usunięty pomyślnie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Nie udało się usunąć pracownika: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Proszę wybrać pracownika do usunięcia.", "Brak wyboru", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private void EditEmployeeButton_Click(object sender, RoutedEventArgs e)
		{
			// Sprawdź, czy pracownik jest zaznaczony w DataGrid
			if (EmployeeListDataGrid.SelectedItem is Employee selectedEmployee)
			{
				// Utwórz okno edycji i przekaż dane pracownika
				EditEmployeeWindow editWindow = new EditEmployeeWindow(selectedEmployee);

				bool? result = editWindow.ShowDialog();
				if (result == true)
				{
					// Refresh the employee list if an employee was edited
					RefreshEmployeeList_Click(sender, e);
				}
			}
			else
			{
				MessageBox.Show("Proszę wybrać pracownika do edycji.");
			}
		}
	}
}