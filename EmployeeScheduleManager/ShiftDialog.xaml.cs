using EmployeeScheduleManager.Models;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EmployeeScheduleManager
{
	/// <summary>
	/// Logika interakcji dla klasy ShiftDialog.xaml
	/// </summary>
	public partial class ShiftDialog : Window
	{
		public string StartTime { get; set; }
		public string EndTime { get; set; }
		public string EmployeeName { get; private set; } // Wybrany pracownik
		private string SelectedLocationId;
		private DateTime SelectedDate;
		public Dictionary<string,string> empAva;

		public ShiftDialog()
		{
			InitializeComponent();
		}

		public void InitializeData(string initialStartTime, string initialEndTime, List<Employee> availableEmployees, string locationId, DateTime selectedDay, string openingHours, Dictionary<string, string> employeeAvailability, List<string> positions)
		{
			StartTimeTextBox.Text = initialStartTime;
			EndTimeTextBox.Text = initialEndTime;
			EmployeeComboBox.ItemsSource = availableEmployees;
			SelectedLocationId = locationId;
			DayLabel.Content = selectedDay.ToString("dddd", new System.Globalization.CultureInfo("pl-PL")); // np. "Poniedziałek";
			OpeningHoursLabel.Content = openingHours;
			empAva = employeeAvailability;
			PositionComboBox.ItemsSource = positions;
			SelectedDate = selectedDay;

			// Split the opening hours if they are in the format "HH:mm-HH:mm"
			if (!string.IsNullOrWhiteSpace(openingHours) && openingHours.Contains("-"))
			{
				string[] hours = openingHours.Split('-');
				if (hours.Length == 2)
				{
					StartTimeTextBox.Text = hours[0].Trim();
					EndTimeTextBox.Text = hours[1].Trim();
				}
			}
		}

		private void EndTimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Pozwalamy tylko na cyfry
			e.Handled = !Regex.IsMatch(e.Text, "^[0-9]$");
		}

		private void EndTimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox == null) return;

			string text = textBox.Text.Replace(":", ""); // Usuwamy istniejące dwukropki
			if (text.Length > 4) text = text.Substring(0, 4); // Ograniczamy do 4 znaków


			if (text.Length >= 2)
			{
				text = text.Insert(2, ":"); // Wstawiamy dwukropek po dwóch cyfrach
			}

			// Walidacja minut (akceptowane: 00, 15, 30, 45)
			if (text.Length == 5) // Pełny format xx:xx
			{
				string minutes = text.Substring(3, 2);
				if (minutes != "00" && minutes != "15" && minutes != "30" && minutes != "45")
				{
					text = text.Substring(0, 3); // Usunięcie błędnych minut
				}
			}

			// Pozycja kursora
			int cursorPosition = textBox.CaretIndex;
			if (cursorPosition == 2 ) { cursorPosition++;	}
		

			textBox.Text = text;
			textBox.CaretIndex = cursorPosition;
		}

		private void StartTimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			// Pozwalamy tylko na cyfry
			e.Handled = !Regex.IsMatch(e.Text, "^[0-9]$");
		}

		private void StartTimeTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox == null) return;

			string text = textBox.Text.Replace(":", ""); // Usuwamy istniejące dwukropki
			if (text.Length > 4) text = text.Substring(0, 4); // Ograniczamy do 4 znaków


			if (text.Length >= 2)
			{
				text = text.Insert(2, ":"); // Wstawiamy dwukropek po dwóch cyfrach
			}

			// Walidacja minut (akceptowane: 00, 15, 30, 45)
			if (text.Length == 5) // Pełny format xx:xx
			{
				string minutes = text.Substring(3, 2);
				if (minutes != "00" && minutes != "15" && minutes != "30" && minutes != "45")
				{
					text = text.Substring(0, 3); // Usunięcie błędnych minut
				}
			}

			// Pozycja kursora
			int cursorPosition = textBox.CaretIndex;
			if (cursorPosition == 2) { cursorPosition++; }


			textBox.Text = text;
			textBox.CaretIndex = cursorPosition;
		}

		private async void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			// Walidacja danych
			if (string.IsNullOrWhiteSpace(StartTimeTextBox.Text) || string.IsNullOrWhiteSpace(EndTimeTextBox.Text))
			{
				MessageBox.Show("Proszę wypełnić wszystkie pola!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (EmployeeComboBox.SelectedItem == null)
			{
				MessageBox.Show("Proszę wybrać pracownika!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			// Parsowanie godzin pracy
			if (!TimeSpan.TryParse(StartTimeTextBox.Text, out TimeSpan shiftStart) ||
				!TimeSpan.TryParse(EndTimeTextBox.Text, out TimeSpan shiftEnd))
			{
				MessageBox.Show("Nieprawidłowy format godzin!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			// Sprawdzenie, czy godziny pracy mieszczą się w godzinach otwarcia
			if (OpeningHoursLabel.Content is string openingHours && openingHours.Contains("-"))
			{
				string[] hours = openingHours.Split('-');
				if (hours.Length == 2 &&
					TimeSpan.TryParse(hours[0].Trim(), out TimeSpan openTime) &&
					TimeSpan.TryParse(hours[1].Trim(), out TimeSpan closeTime))
				{
					if (shiftStart < openTime || shiftEnd > closeTime)
					{
						MessageBox.Show("Godziny pracy muszą mieścić się w zakresie godzin otwarcia!",
							"Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
						return; // Blokowanie zapisu
					}
				}
			}

		
			//SPRAWDZ CZY POZYCJA WYBRANA
			if (PositionComboBox.SelectedItem == null)
			{
				MessageBox.Show("Proszę wybrać stanowisko!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (EmployeeComboBox.SelectedItem is Employee selectedEmployee)
			{
				string employeeId = selectedEmployee.Id; // Pobieramy ID pracownika

				if (empAva.TryGetValue(employeeId, out var availabilityData) && !string.IsNullOrWhiteSpace(availabilityData))
				{
					if (IsOverlappingWithAvailability(StartTimeTextBox.Text, EndTimeTextBox.Text, availabilityData))
					{
						MessageBoxResult result = MessageBox.Show(
							"Godziny pracy pokrywają się z niedostępnością pracownika!\nCzy chcesz kontynuować?",
							"Ostrzeżenie",
							MessageBoxButton.YesNo,
							MessageBoxImage.Warning);

						if (result == MessageBoxResult.No)
						{
							return; // Anuluj zapis
						}
					}
				}
			}

			StartTime = StartTimeTextBox.Text;
			EndTime = EndTimeTextBox.Text;
			EmployeeName = EmployeeComboBox.SelectedItem as string;

			if (EmployeeComboBox.SelectedItem is Employee selectedEmployee_)
			{
				string employeeId = selectedEmployee_.Id; // Pobieramy ID pracownika
				string selectedPosition = PositionComboBox.SelectedItem.ToString();
				await SaveShiftToFirestore(SelectedLocationId, SelectedDate, employeeId, selectedPosition);
			}

			DialogResult = true;
			Close();
		}


		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		public void EmployeeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (EmployeeComboBox.SelectedItem is Employee selectedEmployee)
			{
				string employeeId = selectedEmployee.Id; // Pobieramy ID pracownika
				if (empAva.TryGetValue(employeeId, out var availabilityData))
				{

						UnavaTextBox.Content = availabilityData; // Przypisanie godzin niedostępności
				}
				else
				{
					UnavaTextBox.Content = "Brak danych"; // Jeśli brak wpisu dla pracownika
				}
			}
		}

		private bool IsOverlappingWithAvailability(string startTime, string endTime, string availability)
		{
			if (string.IsNullOrWhiteSpace(availability))
				return false;

			TimeSpan shiftStart = TimeSpan.Parse(startTime);
			TimeSpan shiftEnd = TimeSpan.Parse(endTime);

			string[] unavailablePeriods = availability.Split(',');

			foreach (var period in unavailablePeriods)
			{
				if (period.Contains("-"))
				{
					string[] times = period.Split('-');
					if (times.Length == 2)
					{
						TimeSpan unavaStart = TimeSpan.Parse(times[0].Trim());
						TimeSpan unavaEnd = TimeSpan.Parse(times[1].Trim());

						// Sprawdzenie kolizji przedziałów czasowych
						if ((shiftStart < unavaEnd && shiftEnd > unavaStart))
						{
							return true; // Kolizja wykryta
						}
					}
				}
			}

			return false;
		}

		//zapis do firestore
		private async Task SaveShiftToFirestore(string locationId, DateTime selectedDate, string employeeId, string position)
		{
			try
			{
				FirestoreDb db = FirestoreDb.Create("employeemanagementsystem-132ea");

				string year = selectedDate.Year.ToString();
				string month = selectedDate.Month.ToString("D2"); // Format: 01, 02...
				string day = selectedDate.Day.ToString("D2"); // Format: 01, 02...
				string schedulePath = $"Lokalizacje/{locationId}/Grafik_{year}/Month_{month}/Days";

				// Referencja do dokumentu Day_{day}
				var dayDocRef = db.Collection(schedulePath).Document($"Day_{day}");

				// Pobranie istniejącego dokumentu
				DocumentSnapshot snapshot = await dayDocRef.GetSnapshotAsync();

				Dictionary<string, object> shiftData = new Dictionary<string, object>
				{
					{ "startHour", int.Parse(StartTimeTextBox.Text.Split(':')[0]) },
					{ "startMinute", int.Parse(StartTimeTextBox.Text.Split(':')[1]) },
					{ "endHour", int.Parse(EndTimeTextBox.Text.Split(':')[0]) },
					{ "endMinute", int.Parse(EndTimeTextBox.Text.Split(':')[1]) },
					{ "position", position }
				};

				if (snapshot.Exists)
				{
					await dayDocRef.UpdateAsync(new Dictionary<string, object>
					{
						{ employeeId, shiftData }
					});

				}
				else
				{
					// Jeśli dokument nie istnieje, tworzymy go od nowa
					Dictionary<string, object> newDocumentData = new Dictionary<string, object>
					{
						{ employeeId, shiftData }
					};

					await dayDocRef.SetAsync(newDocumentData);
				}

				MessageBox.Show("Zmiana została zapisana!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Błąd zapisu do Firestore: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}


	}
}
