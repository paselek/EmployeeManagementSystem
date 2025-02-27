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
					//GenerateScheduleGrid(selectedDay, hoursRange, selectedLocation.stanowiska);
				}

				
			}
		}

		private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
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
					//GenerateScheduleGrid(selectedDay, hoursRange, selectedLocation.stanowiska);
					//GenerateScheduleGrid( null , selectedLocation.stanowiska,8,20);
					var dailySchedule = new List<ScheduleEntry>
						{
							new ScheduleEntry { EmployeeName = "Kacper Hooless", Position = "Stanowisko 1", StartHour = 8, StartMinute = 0, EndHour = 12, EndMinute = 15 },
							new ScheduleEntry { EmployeeName = "Anna Nowak", Position = "Stanowisko 2", StartHour = 9, StartMinute = 0, EndHour = 13, EndMinute = 0 },
							new ScheduleEntry { EmployeeName = "Jan Polański", Position = "Stanowisko 1", StartHour = 12, StartMinute = 15, EndHour = 16, EndMinute = 15 },
							new ScheduleEntry { EmployeeName = "Mariusz Nowak", Position = "Stanowisko 1", StartHour = 16, StartMinute = 15, EndHour = 18, EndMinute = 0 }
						};

					var positions = new List<string> { "Stanowisko 1", "Stanowisko 2", "Stanowisko 3" };
					GenerateScheduleInterface(ScheduleGrid, dailySchedule, positions, 8, 18);
				}
			}
		}


		private void GenerateScheduleInterface(Grid scheduleGrid, List<ScheduleEntry> dailySchedule, List<string> positions, int startHour, int endHour)
		{
			scheduleGrid.Children.Clear();
			scheduleGrid.RowDefinitions.Clear();
			scheduleGrid.ColumnDefinitions.Clear();


			double cellWidth = 15; // Stała szerokość komórki
			double cellHeight = 40; // Stała wysokość komórki

			// Dodawanie pierwszej kolumny (nazwy stanowisk)
			scheduleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

			// Dodawanie wierszy dla stanowisk
			foreach (string position in positions)
			{
				scheduleGrid.RowDefinitions.Add(new RowDefinition
				{
					Height = new GridLength(cellHeight, GridUnitType.Pixel)
				});
			}

			// Dodawanie nagłówka godzin (pierwszy wiersz)
			scheduleGrid.RowDefinitions.Insert(0, new RowDefinition
			{
				Height = new GridLength(cellHeight, GridUnitType.Pixel)
			});

			// Dodawanie kolumn dla 15-minutowych przedziałów
			for (int hour = startHour; hour < endHour; hour++)
			{
				for (int quarter = 0; quarter < 4; quarter++) // 4 kolumny na godzinę (co 15 minut)
				{
					scheduleGrid.ColumnDefinitions.Add(new ColumnDefinition
					{
						Width = new GridLength(cellWidth, GridUnitType.Pixel)
					});
				}

				// Dodawanie nagłówka dla pełnej godziny, obejmującego 4 kolumny
				TextBlock hourHeader = new TextBlock
				{
					Text = $"{hour}:00",
					FontWeight = FontWeights.Bold,
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Center,
					Margin = new Thickness(5)
				};

				// Ustawienie w siatce (pierwszy wiersz, scalone 4 kolumny)
				Grid.SetRow(hourHeader, 0);
				Grid.SetColumn(hourHeader, (hour - startHour) * 4 + 1); // +1, aby ominąć kolumnę nazw stanowisk
				Grid.SetColumnSpan(hourHeader, 4); // 4 kolumny na jedną godzinę
				scheduleGrid.Children.Add(hourHeader);
			}

			// Tworzenie nagłówków dla stanowisk (pierwsza kolumna)
			for (int i = 0; i < positions.Count; i++)
			{
				TextBlock positionHeader = new TextBlock
				{
					Text = positions[i],
					FontWeight = FontWeights.Bold,
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					Margin = new Thickness(5)
				};
				Grid.SetRow(positionHeader, i + 1); // Wiersze zaczynają się od 1, bo nagłówki godzin zajmują wiersz 0
				Grid.SetColumn(positionHeader, 0); // Pierwsza kolumna dla stanowisk
				scheduleGrid.Children.Add(positionHeader);
			}

			// Rysowanie poziomych linii (siatka między stanowiskami)
			for (int i = 1; i <= positions.Count; i++)
			{
				Border horizontalLine = new Border
				{
					BorderBrush = Brushes.Gray,
					BorderThickness = new Thickness(0, 1, 0, 0) // Linia pozioma na górze
				};
				Grid.SetRow(horizontalLine, i);
				Grid.SetColumn(horizontalLine, 0);
				Grid.SetColumnSpan(horizontalLine, (endHour - startHour + 1) * 4 + 1); // Cała szerokość siatki
				scheduleGrid.Children.Add(horizontalLine);
			}

			// Rysowanie pionowych linii co godzinę
			for (int hour = 0; hour <= (endHour - startHour); hour++)
			{
				Border verticalLine = new Border
				{
					BorderBrush = Brushes.LightGray,
					BorderThickness = new Thickness(0, 0, 1, 0) // Linia pionowa z prawej strony komórki
				};
				Grid.SetRow(verticalLine, 0);
				Grid.SetColumn(verticalLine, hour * 4 ); // Kolumny co 4 (pełne godziny)
				Grid.SetRowSpan(verticalLine, positions.Count+1); // Cała wysokość siatki
				scheduleGrid.Children.Add(verticalLine);
			}

			// Rysowanie cienkich pionowych linii co 30 minut (opcjonalnie)
			for (int halfHour = 0; halfHour < (endHour - startHour) * 4; halfHour += 2)
			{
				Border halfHourLine = new Border
				{
					BorderBrush = Brushes.LightGray,
					BorderThickness = new Thickness(0.5, 0, 0, 0) // Cieńsza linia
				};
				Grid.SetRow(halfHourLine, 1);
				Grid.SetColumn(halfHourLine, halfHour + 1); // Kolumny co 2 (pół godziny)
				Grid.SetRowSpan(halfHourLine, positions.Count); // Cała wysokość siatki
				scheduleGrid.Children.Add(halfHourLine);
			}

			// Iteracja przez harmonogram dnia
			foreach (var scheduleItem in dailySchedule)
			{
				// Obliczanie pozycji startowej i długości
				int startColumn = (scheduleItem.StartHour - startHour) * 4 + (scheduleItem.StartMinute / 15) + 1; // Kolumna początkowa
				int endColumn = (scheduleItem.EndHour - startHour) * 4 + (scheduleItem.EndMinute / 15) + 1; // Kolumna końcowa
				int columnSpan = endColumn - startColumn; // Liczba scalanych kolumn
				int row = positions.IndexOf(scheduleItem.Position) + 1; // Wiersz na podstawie stanowiska (+1 dla przesunięcia o wiersz godzin)

				if (row == 0)
				{
					// Wyświetl komunikat, jeśli stanowisko jest nieprawidłowe
					MessageBox.Show($"Stanowisko '{scheduleItem.Position}' nie istnieje.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
					continue;
				}

				// Tworzenie elementu ListBoxItem
				ListBoxItem scheduleItemBox = new ListBoxItem
				{
					Content = new StackPanel
					{
						Orientation = Orientation.Vertical,
						Children =
			{
				new TextBlock
				{
					Text = scheduleItem.EmployeeName,
					FontWeight = FontWeights.Bold,
					TextAlignment = TextAlignment.Center
				},
				new TextBlock
				{
					Text = $"{scheduleItem.StartHour}:{scheduleItem.StartMinute:D2} - {scheduleItem.EndHour}:{scheduleItem.EndMinute:D2}",
					FontSize = 10,
					TextAlignment = TextAlignment.Center
				}
			}
					},
					Background = Brushes.LightBlue,
					Margin = new Thickness(2),
					//Tag = scheduleItem // Przechowywanie danych w Tag dla przyszłej edycji
				};

				/*// Obsługa kliknięcia elementu
				scheduleItemBox.MouseLeftButtonUp += (s, e) =>
				{
					var clickedItem = (ListBoxItem)s;
					var itemData = (ScheduleItem)clickedItem.Tag;
					// Wywołanie funkcji edycji
					EditScheduleItem(itemData);
				};*/

				// Ustawianie w siatce
				Grid.SetRow(scheduleItemBox, row);
				Grid.SetColumn(scheduleItemBox, startColumn);
				Grid.SetColumnSpan(scheduleItemBox, columnSpan);

				// Dodanie do siatki głównej
				scheduleGrid.Children.Add(scheduleItemBox);
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

		private async void OpenShiftDialog(object sender, RoutedEventArgs e)
		{
			/*// Oblicz początek i koniec przedziału czasowego
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

			}*/
			string? selLoc = null;
			if (LocationComboBox.SelectedItem is Location selectedLocation)
			{
				selLoc = selectedLocation.Id;
				var dialog = new ShiftDialog();
				dialog.InitializeData(
						initialStartTime: "12:15",
						initialEndTime: "12:30",
						availableEmployees: await _firestoreTest.GetEmployeesFromLocation(selLoc)
					);
				dialog.ShowDialog();
			}
		}




	}
}