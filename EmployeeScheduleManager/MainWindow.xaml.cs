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
			LocationComboBox.ItemsSource = locations;
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

		private void DetailLocationButton_Click(object sender, RoutedEventArgs e)
		{
			// Sprawdź, czy pracownik jest zaznaczony w DataGrid
			if (LocationsDataGrid.SelectedItem is Location selectedLocation)
			{
				// Utwórz okno edycji i przekaż dane pracownika
				new DetailedLocationWindow(selectedLocation).ShowDialog();
			}
			else
			{
				MessageBox.Show("Proszę wybrać lokalizację do wyświetlenia.");
			}
		}

		private void LocationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (LocationComboBox.SelectedItem is Location selectedLocation)
			{
				var dniTygodniaMap = new Dictionary<string, string>
				{
					{ "sunday", "niedziela" },
					{ "monday", "poniedzialek" },
					{ "tuesday", "wtorek" },
					{ "wednesday", "sroda" },
					{ "thursday", "czwartek" },
					{ "friday", "piatek" },
					{ "saturday", "sobota" }
				};

				string? selectedDayEnglish = Calendar.SelectedDate?.DayOfWeek.ToString().ToLower(); // Nazwa dnia tygodnia

				if (selectedDayEnglish != null && dniTygodniaMap.TryGetValue(selectedDayEnglish, out string? selectedDay))
				{
					string hoursRange = selectedLocation.godzinyOtwarcia[selectedDay];


					// Sprawdź, czy lokalizacja jest otwarta w wybrany dzień
					if (hoursRange == "-")
					{
						MessageBox.Show("Wybrana lokalizacja jest zamknięta w tym dniu.");
						return;
					}
					GenerateScheduleGrid(selectedDay, hoursRange, selectedLocation.stanowiska);
				}

				
			}
		}

		private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
		{
			if (LocationComboBox.SelectedItem is Location selectedLocation)
			{
				string selectedDay = Calendar.SelectedDate?.DayOfWeek.ToString().ToLower();
				/*string hoursRange = selectedLocation.GodzinyOtwarcia[selectedDay];

				if (hoursRange == "-")
				{
					MessageBox.Show("Wybrana lokalizacja jest zamknięta w tym dniu.");
					return;
				}

				GenerateScheduleGrid(selectedDay, hoursRange, selectedLocation.Stanowiska);
				LoadScheduleFromFirestore(selectedLocation.Id, Calendar.SelectedDate.Value); // Wczytaj zapisany grafik dla dnia*/
			}
		}

		private void GenerateScheduleGrid(string day, string hoursRange, List<string> stanowiska)
		{
			ScheduleGrid.Children.Clear();
			ScheduleGrid.RowDefinitions.Clear();
			ScheduleGrid.ColumnDefinitions.Clear();

			if (hoursRange == "-")
			{
				// Wyświetl komunikat, że nie można przypisać grafik na dany dzień
				MessageBox.Show("Lokalizacja jest zamknięta w wybranym dniu.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
				return;
			}

			// Parse godzin otwarcia, np. "08:00-18:00"
			var hours = hoursRange.Split('-');
			TimeSpan startTime = TimeSpan.Parse(hours[0]);
			TimeSpan endTime = TimeSpan.Parse(hours[1]);
			int totalSlots = (int)(endTime - startTime).TotalMinutes / 30; // 30-minutowe przedziały

			// Dodaj kolumny
			ScheduleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) }); // Kolumna dla stanowisk
			for (int i = 0; i < totalSlots; i++)
			{
				ScheduleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(35) });
			}

			// Dodaj wiersze
			for (int row = 0; row <= stanowiska.Count; row++)
			{
				ScheduleGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			}

			// Nagłówki czasowe
			for (int slot = 0; slot <= totalSlots; slot++)
			{
				if (slot == 0)
				{
					AddTextBlockToGrid("Stanowisko", 0, slot);
				}
				else
				{
					var time = startTime.Add(TimeSpan.FromMinutes(slot * 30));
					AddTextBlockToGrid(time.ToString(@"hh\:mm"), 0, slot);
				}
			}

			// Stanowiska i przyciski
			for (int row = 1; row <= stanowiska.Count; row++)
			{
				AddTextBlockToGrid(stanowiska[row - 1], row, 0);

				for (int col = 1; col <= totalSlots; col++)
				{
					var button = new Button
					{
						Content = "",
						Background = Brushes.LightGray,
						Tag = new { Row = row, Column = col }
					};
					button.Click += (s, e) => OpenShiftDialog((Button)s, startTime, col);

					Grid.SetRow(button, row);
					Grid.SetColumn(button, col);
					ScheduleGrid.Children.Add(button);
				}
			}
		}

		private void AddTextBlockToGrid(string text, int row, int col)
		{
			var textBlock = new TextBlock
			{
				Text = text,
				VerticalAlignment = VerticalAlignment.Center,
				HorizontalAlignment = HorizontalAlignment.Center,
				FontWeight = FontWeights.Bold
			};
			Grid.SetRow(textBlock, row);
			Grid.SetColumn(textBlock, col);
			ScheduleGrid.Children.Add(textBlock);
		}

		private async void OpenShiftDialog(Button button, TimeSpan startTime, int column)
		{
			// Oblicz początek i koniec przedziału czasowego
			var start = startTime.Add(TimeSpan.FromMinutes((column - 1) * 30));
			var end = start.Add(TimeSpan.FromMinutes(30));
			string? selLoc = null;
			if (LocationComboBox.SelectedItem is Location selectedLocation)
			{
				selLoc = selectedLocation.Id;

				// Stwórz instancję ShiftDialog
				var dialog = new ShiftDialog();
				dialog.InitializeData(
					initialStartTime: start.ToString(@"hh\:mm"),
					initialEndTime: end.ToString(@"hh\:mm"),
					availableEmployees: await _firestoreTest.GetEmployeesFromLocation(selLoc)
				);


				// Pokaż dialog i przetwarzaj wynik
				if (dialog.ShowDialog() == true)
				{
					button.Content = $"{dialog.EmployeeName}\n{dialog.StartTime}-{dialog.EndTime}";
					button.Background = Brushes.LightGreen;

					// Logika zapisu pracownika do lokalnej struktury lub bezpośrednio do bazy
					//SaveShiftToDatabase(dialog.EmployeeName, dialog.StartTime, dialog.EndTime);
				}
			}
		}




	}
}