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
	/// 

	public partial class MainWindow : Window
	{

		private readonly FirestoreTest _firestoreTest;
		public MainWindow()
		{
			_firestoreTest = new FirestoreTest();

			InitializeComponent();
			LoadData();
			InitializeSummarySelectors();
		}



		private async void LoadData()
		{
			var employees = await _firestoreTest.GetEmployeesAsync();
			var locations = await _firestoreTest.GetLocationsAsync();
			await _firestoreTest.CountEmployessAll();
			LocationComboBox.ItemsSource = locations;
			EmployeeListDataGrid.ItemsSource = employees;
			LocationsDataGrid.ItemsSource = locations;
			LocationSummaryComboBox.ItemsSource = locations;
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
			// Ensure an location is selected
			if (LocationsDataGrid.SelectedItem is Location selectedLocation)
			{
				// Confirm deletion
				var result = MessageBox.Show($"Czy jesteś pewien że chcesz usunąć lokalizacje: {selectedLocation.nazwa} {selectedLocation.adres}?",
											  "Potwierdź usunięcie", MessageBoxButton.YesNo, MessageBoxImage.Warning);

				if (result == MessageBoxResult.Yes)
				{
					try
					{
						// Delete the selected location
						await _firestoreTest.DeleteLocationAsync(selectedLocation.Id);

						// Refresh the data
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
				Calendar.IsEnabled = true;
				AddShift.IsEnabled = true;
				if (!Calendar.SelectedDate.HasValue)
				{
					Calendar.SelectedDate = DateTime.Today;
				}
				else
				{
					// Jeśli data już jest ustawiona, można ją ponownie przypisać, aby wymusić wywołanie zdarzenia
					DateTime currentDate = Calendar.SelectedDate.Value;
					Calendar.SelectedDate = null;
					Calendar.SelectedDate = currentDate;
				}
			}
		}

		private async void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
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
				string hoursRange;
				if (selectedDayEnglish != null && dniTygodniaMap.TryGetValue(selectedDayEnglish, out string? selectedDay))
				{
					hoursRange = selectedLocation.godzinyOtwarcia[selectedDay];


					// Sprawdź, czy lokalizacja jest otwarta w wybrany dzień
					if (hoursRange == "-")
					{
						MessageBox.Show("Wybrana lokalizacja jest zamknięta w tym dniu.");
						var dailyScheduleL = new List<ScheduleEntry>();
						if (Calendar.SelectedDate.HasValue)
						{
							DateTime selectedDate = Calendar.SelectedDate.Value;
							dailyScheduleL = await _firestoreTest.GetDailyScheduleWithNamesAsync(selectedLocation.Id, selectedDate);
						}
						else
						{
							// Obsłuż przypadek, gdy data nie została wybrana
							MessageBox.Show("Proszę wybrać datę.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
						}

						List<string> positionsL = await _firestoreTest.GetPositionsFromDatabase(selectedLocation.Id);
						GenerateScheduleInterface(ScheduleGrid, dailyScheduleL, positionsL, 0, 24);
						return;
					}

					var dailySchedule = new List<ScheduleEntry>();
					if (Calendar.SelectedDate.HasValue)
					{
						DateTime selectedDate = Calendar.SelectedDate.Value;
						dailySchedule = await _firestoreTest.GetDailyScheduleWithNamesAsync(selectedLocation.Id, selectedDate);
					}
					else
					{
						// Obsłuż przypadek, gdy data nie została wybrana
						MessageBox.Show("Proszę wybrać datę.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
					}

					//var positions = new List<string> { "Stanowisko 1", "Stanowisko 2", "Stanowisko 3" };
					List<string> positions = await _firestoreTest.GetPositionsFromDatabase(selectedLocation.Id);
					// Pobierz godziny otwarcia lokalizacji dla wybranego dnia
					string[] hours = hoursRange.Split('-');
					string[] open = hours[0].Split(':');
					string[] close = hours[1].Split(':');

					// Konwersja na liczby całkowite
					int openHour = int.Parse(open[0]);
					int openMinute = int.Parse(open[1]);
					int closeHour = int.Parse(close[0]);
					int closeMinute = int.Parse(close[1]);
					GenerateScheduleInterface(ScheduleGrid, dailySchedule, positions, openHour, closeHour);
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
				Grid.SetColumn(verticalLine, hour * 4); // Kolumny co 4 (pełne godziny)
				Grid.SetRowSpan(verticalLine, positions.Count + 1); // Cała wysokość siatki
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
					Tag = scheduleItem // Przechowywanie danych w Tag dla przyszłej edycji
				};


				// Obsługa kliknięcia elementu - usuwanie zmiany
				scheduleItemBox.MouseRightButtonUp += async (s, e) =>
				{
					var clickedItem = (ListBoxItem)s;
					var itemData = (ScheduleEntry)clickedItem.Tag; // Pobranie danych zmiany

					MessageBoxResult result = MessageBox.Show(
						$"Czy na pewno chcesz usunąć zmianę dla {itemData.EmployeeName}?",
						"Potwierdzenie usunięcia",
						MessageBoxButton.YesNo,
						MessageBoxImage.Warning);

					if (result == MessageBoxResult.Yes)
					{
						var location = LocationComboBox.SelectedItem as Location;
						DateTime Data = Calendar.SelectedDate ?? DateTime.Now;
						// Usuń zmianę z Firestore
						await _firestoreTest.DeleteShiftFromFirestore(location.Id, Data, itemData.EmployeeId);

						// Usuń element z interfejsu
						scheduleGrid.Children.Remove(clickedItem);
					}
				};

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
			if (LocationComboBox.SelectedItem is Location selectedLocation)
			{
				string locationId = selectedLocation.Id;
				DateTime selectedDate = Calendar.SelectedDate ?? DateTime.Today;
				DateTime dayOfWeek = selectedDate;

				// Pobierz godziny otwarcia lokalizacji dla wybranego dnia
				string openingHours = await _firestoreTest.GetOpeningHours(locationId, dayOfWeek.ToString("dddd",new System.Globalization.CultureInfo("pl-PL")));

				// Pobierz listę pracowników i ich niedostępność
				List<Employee> employees = await _firestoreTest.GetEmployeesFromLocation(locationId);
				Dictionary<string, string> employeeAvailability = await _firestoreTest.GetEmployeeAvailability(locationId, dayOfWeek.ToString("dddd", new System.Globalization.CultureInfo("pl-PL")));
				var positions = await _firestoreTest.GetPositionsFromDatabase(locationId);
				// Stworzenie okna dialogowego
				var dialog = new ShiftDialog();
				dialog.InitializeData(
					initialStartTime: "12:15",
					initialEndTime: "12:30",
					availableEmployees: employees,
					locationId: locationId,
					selectedDay: dayOfWeek,
					openingHours: openingHours,
					employeeAvailability: employeeAvailability,
					positions: positions
				);

				// Otwórz okno dialogowe
				dialog.ShowDialog();
				// Jeśli data już jest ustawiona, można ją ponownie przypisać, aby wymusić wywołanie zdarzenia
				DateTime currentDate = Calendar.SelectedDate.Value;
				Calendar.SelectedDate = null;
				Calendar.SelectedDate = currentDate;
			}
		}

		private async void Summary_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (LocationSummaryComboBox.SelectedItem is Location location &&
				YearComboBox.SelectedItem is int year &&
				MonthComboBox.SelectedItem is int month)
			{
				var summary = await _firestoreTest.GetMonthlySummaryAsync(location.Id, year, month);

				DaysWithShiftsText.Text = $"Dni z pracownikami: {summary.DaysWithShifts}";
				TotalHoursText.Text = $"Łączna liczba godzin: {summary.TotalHoursWorked:F2}";

				PositionsSummaryGrid.ItemsSource = summary.PositionSummaries;
				EmployeesSummaryGrid.ItemsSource = summary.EmployeeSummaries;
			}
		}

		private void InitializeSummarySelectors()
		{
			// Lata 2025–2030
			List<int> years = Enumerable.Range(2025, 6).ToList();
			YearComboBox.ItemsSource = years;
			YearComboBox.SelectedItem = DateTime.Now.Year;

			// Miesiące 1–12
			List<int> months = Enumerable.Range(1, 12).ToList();
			MonthComboBox.ItemsSource = months;
			MonthComboBox.SelectedItem = DateTime.Now.Month;

			
		}

	}
}