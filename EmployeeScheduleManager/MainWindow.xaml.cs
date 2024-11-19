using EmployeeScheduleManager.Models;
using Google.Cloud.Firestore;
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
		public MainWindow()
		{
			_firestoreTest = new FirestoreTest();

			InitializeComponent();
			LoadData();
		}

		private async void LoadData()
		{
			var employees = await _firestoreTest.GetEmployeesAsync();
			var locations = await _firestoreTest.GetLocationsAsync();
			await _firestoreTest.CountEmployessAll();

			EmployeeListDataGrid.ItemsSource = employees;
			LocationsDataGrid.ItemsSource = locations;
		}
		private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
		{
			new AddEmployeeWindow().ShowDialog();
			LoadData();
		}
		private void RefreshData_Click(object sender, RoutedEventArgs e)
		{
			LoadData();
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
						LoadData();

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
				new EditEmployeeWindow(selectedEmployee).ShowDialog();
				LoadData();
			}
			else
			{
				MessageBox.Show("Proszę wybrać pracownika do edycji.");
			}
		}

		private void AddLocationButton_Click(object sender, RoutedEventArgs e)
		{
			new AddLocationWindow().ShowDialog();
			LoadData();
		}

		private async void DeleteLocationButton_Click(object sender, RoutedEventArgs e)
		{
			// Ensure an employee is selected
			if (LocationsDataGrid.SelectedItem is Location selectedLocation)
			{
				// Confirm deletion
				var result = MessageBox.Show($"Czy jesteś pewien że chcesz usunąć lokalizacje: {selectedLocation.nazwa} {selectedLocation.adres}?",
											  "Potwierdź usunięcie", MessageBoxButton.YesNo, MessageBoxImage.Warning);

				if (result == MessageBoxResult.Yes)
				{
					try
					{
						// Delete the selected employee
						await _firestoreTest.DeleteLocationAsync(selectedLocation.Id);

						// Refresh the employee list
						LoadData();

						MessageBox.Show("Lokalizacja usunięta pomyślnie.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Nie udało się usunąć lokalizacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
					}
				}
			}
			else
			{
				MessageBox.Show("Proszę wybrać lokalizację do usunięcia.", "Brak wyboru", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private void EditLocationButton_Click(object sender, RoutedEventArgs e)
		{
			// Sprawdź, czy pracownik jest zaznaczony w DataGrid
			if (LocationsDataGrid.SelectedItem is Location selectedLocation)
			{
				// Utwórz okno edycji i przekaż dane pracownika
				new EditLocationWindow(selectedLocation).ShowDialog();
				LoadData();
			}
			else
			{
				MessageBox.Show("Proszę wybrać lokalizację do edycji.");
			}
		}
	}
}