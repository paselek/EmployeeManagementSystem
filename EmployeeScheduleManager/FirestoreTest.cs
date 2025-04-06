using System;
using System.Threading.Tasks;
using FirebaseAdmin;
using Google.Cloud.Firestore;
using EmployeeScheduleManager.Models;
using System.Windows;

namespace EmployeeScheduleManager
{
    public class FirestoreTest
    {
		private readonly FirestoreDb _firestoreDb;

		public FirestoreTest()
		{
			_firestoreDb = FirestoreDb.Create("employeemanagementsystem-132ea");
			System.Diagnostics.Debug.WriteLine("Połączono z Firestore!");
		}

		public async Task DodajDokumentPrzykladowy()
		{
			var kolekcja = _firestoreDb.Collection("Test_Polaczenia");
			var dokument = kolekcja.Document("Test_Dokument");

			await dokument.SetAsync(new { Name = "Test", CreatedAt = Timestamp.GetCurrentTimestamp() });
			System.Diagnostics.Debug.WriteLine("Dodano dokument do Firestore!");
		}

		public async Task<List<Employee>> GetEmployeesAsync()
		{
			var employeeCollection = _firestoreDb.Collection("Pracownicy");
			var snapshot = await employeeCollection.GetSnapshotAsync();
			return snapshot.Documents.Select(doc => doc.ConvertTo<Employee>()).ToList();
		}
		public async Task<List<Employee>> GetEmployeesFromLocation(string lokalizacjaId)
		{
			// Zapytanie do kolekcji "Pracownicy" z filtrem na pole "lokalizacja"
			var query = _firestoreDb.Collection("Pracownicy")
										.WhereEqualTo("lokalizacja", lokalizacjaId);
			var snapshot = await query.GetSnapshotAsync();
			return  snapshot.Documents.Select(doc => doc.ConvertTo<Employee>()).ToList();
		}

		public async Task<string> GetEmployeeFullNameAsync(string employeeId)
		{
			// Pobieramy dokument pracownika z kolekcji "Pracownicy"
			var docRef = _firestoreDb.Collection("Pracownicy").Document(employeeId);
			var snapshot = await docRef.GetSnapshotAsync();

			if (snapshot.Exists)
			{
				var data = snapshot.ToDictionary();
				// Zakładamy, że dane pracownika mają pola "imie" i "nazwisko"
				string imie = data.ContainsKey("imie") ? data["imie"].ToString() : "";
				string nazwisko = data.ContainsKey("nazwisko") ? data["nazwisko"].ToString() : "";
				return $"{imie} {nazwisko}";
			}
			return employeeId; // Jeśli nie znaleziono dokumentu, zwracamy sam identyfikator
		}

		public async Task<List<string>> GetPositionsFromDatabase(string locationId)
		{
			DocumentReference docRef = _firestoreDb.Collection("Lokalizacje").Document(locationId);
			DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

			if (snapshot.Exists)
			{
				List<string> stanowiska = snapshot.GetValue<List<string>>("stanowiska");
				return stanowiska ?? new List<string>(); // Zwróć pustą listę, jeśli `stanowiska` to `null`
			}
			return new List<string>();
		}

		public async Task AddEmployee(string firstName, string lastName,string location, Dictionary<string, string> unavailability)
		{
			var employeesCollection = _firestoreDb.Collection("Pracownicy");

			// Fetch all employee documents to find the next available ID
			var allEmployees = await employeesCollection.ListDocumentsAsync().ToListAsync();
			int nextId = 1;
			var existingIds = allEmployees.Select(doc => doc.Id).ToList();

			// Find the first available ID in the format 'pracownik_001', 'pracownik_002', etc.
			while (existingIds.Contains($"pracownik_{nextId:D3}"))
			{
				nextId++;
			}
			string newEmployeeId = $"pracownik_{nextId:D3}";

			// Create the employee data
			var employeeData = new Dictionary<string, object>
			{
				{ "imie", firstName },
				{ "nazwisko", lastName },
				{ "lokalizacja", location },
				{ "niedostepnosc", unavailability }
			};

			// Add the new employee document to Firestore
			await employeesCollection.Document(newEmployeeId).SetAsync(employeeData);

			System.Diagnostics.Debug.WriteLine($"Employee {firstName} {lastName} added with ID {newEmployeeId}");
		}
		public async Task DeleteEmployeeAsync(string employeeId)
		{
			// Get a reference to the Pracownicy collection
			var employeeRef = _firestoreDb.Collection("Pracownicy").Document(employeeId);

			// Delete the employee document
			await employeeRef.DeleteAsync();
		}
		public async Task UpdateEmployeeAsync(Employee employee)
		{
			var employeeRef = _firestoreDb.Collection("Pracownicy").Document(employee.Id);

			var updateData = new Dictionary<string, object>
			{
				{ "imie", employee.imie },
				{ "nazwisko", employee.nazwisko },
				{ "lokalizacja", employee.lokalizacja },
				{ "niedostepnosc", employee.niedostepnosc }
			};

			await employeeRef.UpdateAsync(updateData);
		}

		public async Task<List<Location>> GetLocationsAsync()
		{
			var locations = new List<Location>();
			var snapshot = await _firestoreDb.Collection("Lokalizacje").GetSnapshotAsync();

			foreach (var document in snapshot.Documents)
			{
				var location = document.ConvertTo<Location>(); // Zakładając, że masz klasę Location
				locations.Add(location);
			}

			return locations;
		}
		public async Task CountEmployees(Location lokalizacja)
		{
			var pracownicyQuery = _firestoreDb.Collection("Pracownicy")
				.WhereEqualTo("lokalizacja", lokalizacja.Id);

			var snapshot = await pracownicyQuery.GetSnapshotAsync();

			// Zaktualizuj licznik pracowników
			lokalizacja.liczbaPracownikow = snapshot.Count;

			// Zaktualizuj bazę danych (dokument lokalizacji)
			var lokalizacjaDocument = _firestoreDb.Collection("Lokalizacje").Document(lokalizacja.Id);
			await lokalizacjaDocument.UpdateAsync(new Dictionary<string, object>
			{
				{ "liczbaPracownikow", lokalizacja.liczbaPracownikow }
			});
		}
		public async Task CountEmployessAll()
		{
			var lokalizacjeQuery = await _firestoreDb.Collection("Lokalizacje").GetSnapshotAsync();

			foreach (var lokalizacjaDoc in lokalizacjeQuery.Documents)
			{
				var lokalizacja = lokalizacjaDoc.ConvertTo<Location>();
				await CountEmployees(lokalizacja);
			}
		}
		public async Task AddLocation(string nazwa, string adres, Dictionary<string, string> openTime, string[] listaStanowisk)
		{
			var locationCollection = _firestoreDb.Collection("Lokalizacje");

			// Fetch all employee documents to find the next available ID
			var allLocations = await locationCollection.ListDocumentsAsync().ToListAsync();
			int nextId = 1;
			var existingIds = allLocations.Select(doc => doc.Id).ToList();

			// Find the first available ID in the format 'pracownik_001', 'pracownik_002', etc.
			while (existingIds.Contains($"lokalizacja_{nextId:D3}"))
			{
				nextId++;
			}
			string newLocationId = $"lokalizacja_{nextId:D3}";

			// Create the employee data
			var locationData = new Dictionary<string, object>
			{
				{ "nazwa", nazwa },
				{ "adres", adres },
				{ "godzinyOtwarcia", openTime },
				{ "stanowiska", listaStanowisk }
			};

			// Add the new employee document to Firestore
			await locationCollection.Document(newLocationId).SetAsync(locationData);

			System.Diagnostics.Debug.WriteLine($"Location {nazwa}  added with ID {newLocationId}");
		}
		public async Task DeleteLocationAsync(string locationId)
		{
			// Get a reference to the Pracownicy collection
			var locationRef = _firestoreDb.Collection("Lokalizacje").Document(locationId);

			// Delete the employee document
			await locationRef.DeleteAsync();
		}
		public async Task UpdateLocationAsync(Location location)
		{
			var locationRef = _firestoreDb.Collection("Lokalizacje").Document(location.Id);

			var updateData = new Dictionary<string, object>
			{
				{ "nazwa", location.nazwa },
				{ "adres", location.adres },
				{ "godzinyOtwarcia", location.godzinyOtwarcia },
				{ "stanowiska", location.stanowiska }
			};

			await locationRef.UpdateAsync(updateData);
		}

		// Metoda pobierająca "surowy" harmonogram z Firestore dla wybranego dnia
		public async Task<List<ScheduleEntry>> GetDailyScheduleAsync(string locationId, DateTime selectedDate)
		{
			// Przykładowy format dokumentu dla dnia: "2024-04-25"
			string yearDocId = "Grafik_" + selectedDate.ToString("yyyy");
			string monthDocId = "Month_" + selectedDate.ToString("MM");
			string dayDocId = "Day_" + selectedDate.ToString("dd");

			DocumentReference docRef = _firestoreDb
				.Collection("Lokalizacje")
				.Document(locationId)
				.Collection(yearDocId)
				.Document(monthDocId)
				.Collection("Days")
				.Document(dayDocId);

			DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
			List<ScheduleEntry> rawSchedule = new List<ScheduleEntry>();

			if (snapshot.Exists)
			{
				Dictionary<string, object> data = snapshot.ToDictionary();
				foreach (var kvp in data)
				{
					// Klucz to identyfikator pracownika, np. "pracownik_001"
					string employeeId = kvp.Key;

					if (kvp.Value is Dictionary<string, object> scheduleData)
					{
						int startHour = Convert.ToInt32(scheduleData["startHour"]);
						int startMinute = Convert.ToInt32(scheduleData["startMinute"]);
						int endHour = Convert.ToInt32(scheduleData["endHour"]);
						int endMinute = Convert.ToInt32(scheduleData["endMinute"]);
						string position = scheduleData["position"].ToString();

						rawSchedule.Add(new ScheduleEntry
						{
							EmployeeName = employeeId, // Początkowo identyfikator
							Position = position,
							StartHour = startHour,
							StartMinute = startMinute,
							EndHour = endHour,
							EndMinute = endMinute
						});
					}
				}
			}
			return rawSchedule;
		}

		// Metoda uzupełniająca harmonogram o pełne imiona pracowników
		public async Task<List<ScheduleEntry>> GetDailyScheduleWithNamesAsync(string locationId, DateTime selectedDate)
		{
			var rawSchedule = await GetDailyScheduleAsync(locationId, selectedDate);
			var finalSchedule = new List<ScheduleEntry>();

			foreach (var entry in rawSchedule)
			{
				// Metoda GetEmployeeFullNameAsync pobiera pełne imię i nazwisko dla danego identyfikatora
				entry.EmployeeId = entry.EmployeeName;
				string fullName = await GetEmployeeFullNameAsync(entry.EmployeeName);
				entry.EmployeeName = fullName;
				finalSchedule.Add(entry);
			}
			return finalSchedule;
		}

		//Nowe
		private static readonly Dictionary<string, string> PolishDaysToDb = new()
		{
			{ "poniedziałek", "poniedzialek" },
			{ "wtorek", "wtorek" },
			{ "środa", "sroda" },
			{ "czwartek", "czwartek" },
			{ "piątek", "piatek" },
			{ "sobota", "sobota" },
			{ "niedziela", "niedziela" }
		};

		public async Task<string> GetOpeningHours(string locationId, string dayOfWeek)
		{
			if (!PolishDaysToDb.ContainsKey(dayOfWeek))
				return "-"; // Niepoprawny dzień tygodnia

			string dbDayOfWeek = PolishDaysToDb[dayOfWeek]; // Zamiana na wersję bez polskich znaków

			DocumentReference docRef = _firestoreDb.Collection("Lokalizacje").Document(locationId);
			DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

			if (snapshot.Exists && snapshot.ContainsField("godzinyOtwarcia"))
			{
				Dictionary<string, object> openingHours = snapshot.GetValue<Dictionary<string, object>>("godzinyOtwarcia");
				return openingHours.ContainsKey(dbDayOfWeek) ? openingHours[dbDayOfWeek].ToString() : "-";
			}
			return "-"; // Jeśli brak danych, traktujemy jako zamknięte
		}

		public async Task<Dictionary<string, string>> GetEmployeeAvailability(string locationId, string dayOfWeek)
		{
			Dictionary<string, string> employeeAvailability = new();

			// Konwersja nazwy dnia na wersję zgodną z bazą danych
			Dictionary<string, string> dayMapping = new()
			{
			{ "poniedziałek", "Poniedzialek" },
			{ "wtorek", "Wtorek" },
			{ "środa", "Sroda" },
			{ "czwartek", "Czwartek" },
			{ "piątek", "Piatek" },
			{ "sobota", "Sobota" },
			{ "niedziela", "Niedziela" }
			};

			if (!dayMapping.TryGetValue(dayOfWeek, out string? dbDay))
			{
				Console.WriteLine($"Nieprawidłowy dzień tygodnia: {dayOfWeek}");
				return employeeAvailability; // Zwracamy pusty słownik, jeśli dzień nie jest poprawny
			}

			try
			{
				QuerySnapshot employeesSnapshot = await _firestoreDb
					.Collection("Pracownicy")
					.WhereEqualTo("lokalizacja", locationId)
					.GetSnapshotAsync();

				foreach (DocumentSnapshot employeeDoc in employeesSnapshot.Documents)
				{
					string employeeId = employeeDoc.Id;
					string availability = "Brak danych"; // Domyślna wartość

					if (employeeDoc.ContainsField("niedostepnosc"))
					{
						Dictionary<string, object> availabilityMap = employeeDoc.GetValue<Dictionary<string, object>>("niedostepnosc");

						if (availabilityMap.ContainsKey(dbDay))
						{
							availability = availabilityMap[dbDay].ToString();

							if (availability == "-")
							{
								availability = "Dostępny"; // Jeśli "-" → pracownik jest dostępny cały dzień
							}
						}
					}

					employeeAvailability[employeeId] = availability;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Błąd pobierania niedostępności: {ex.Message}");
			}

			return employeeAvailability;
		}

		public async Task DeleteShiftFromFirestore(string locationId, DateTime selectedDate, string employeeId)
		{
			try
			{
				string year = selectedDate.Year.ToString();
				string month = selectedDate.Month.ToString("D2");
				string day = selectedDate.Day.ToString("D2");
				string schedulePath = $"Lokalizacje/{locationId}/Grafik_{year}/Month_{month}/Days";
				var dayDocRef = _firestoreDb.Collection(schedulePath).Document($"Day_{day}");

				// Pobranie istniejącego dokumentu
				DocumentSnapshot snapshot = await dayDocRef.GetSnapshotAsync();

				if (snapshot.Exists)
				{
					await dayDocRef.UpdateAsync(new Dictionary<string, object>
					{
						{ employeeId, FieldValue.Delete }
					});

					MessageBox.Show("Zmiana została usunięta!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else
				{
					MessageBox.Show("Brak danych do usunięcia.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Information);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Błąd podczas usuwania zmiany: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		public async Task<MonthlySummary> GetMonthlySummaryAsync(string locationId, int year, int month)
		{
			

			string monthPath = $"Lokalizacje/{locationId}/Grafik_{year}/Month_{month:D2}/Days";
			CollectionReference daysRef = _firestoreDb.Collection(monthPath);
			QuerySnapshot dayDocs = await daysRef.GetSnapshotAsync();

			var employeeHours = new Dictionary<string, double>();
			var employeeWorkDays = new Dictionary<string, int>();
			var positionHours = new Dictionary<string, double>();
			int daysWithShifts = 0;

			foreach (DocumentSnapshot doc in dayDocs.Documents)
			{
				bool anyShiftThisDay = false;

				foreach (var kvp in doc.ToDictionary())
				{
					string employeeId = kvp.Key;
					if (kvp.Value is Dictionary<string, object> shift)
					{
						int startHour = Convert.ToInt32(shift["startHour"]);
						int startMinute = Convert.ToInt32(shift["startMinute"]);
						int endHour = Convert.ToInt32(shift["endHour"]);
						int endMinute = Convert.ToInt32(shift["endMinute"]);
						string position = shift["position"].ToString();

						double hours = (endHour + endMinute / 60.0) - (startHour + startMinute / 60.0);
						anyShiftThisDay = true;

						// Sumuj dla pracownika
						if (!employeeHours.ContainsKey(employeeId)) employeeHours[employeeId] = 0;
						employeeHours[employeeId] += hours;

						if (!employeeWorkDays.ContainsKey(employeeId)) employeeWorkDays[employeeId] = 0;
						employeeWorkDays[employeeId] += 1;

						// Sumuj dla stanowiska
						if (!positionHours.ContainsKey(position)) positionHours[position] = 0;
						positionHours[position] += hours;
					}
				}

				if (anyShiftThisDay) daysWithShifts++;
			}

			// Oblicz liczbę dni w miesiącu
			int daysInMonth = DateTime.DaysInMonth(year, month);

			// Pobierz dane pracowników
			var employeesSnapshot = await _firestoreDb.Collection("Pracownicy").GetSnapshotAsync();
			var employeeNameMap = new Dictionary<string, string>();
			foreach (var emp in employeesSnapshot.Documents)
			{
				string fullName = $"{emp.GetValue<string>("imie")} {emp.GetValue<string>("nazwisko")}";
				employeeNameMap[emp.Id] = fullName;
			}

			var employeeSummaries = employeeHours.Select(kvp => new EmployeeSummary
			{
				EmployeeName = employeeNameMap.ContainsKey(kvp.Key) ? employeeNameMap[kvp.Key] : kvp.Key,
				TotalHours = kvp.Value,
				DaysWorked = employeeWorkDays[kvp.Key],
				DaysOff = daysInMonth - employeeWorkDays[kvp.Key]
			}).ToList();

			var positionSummaries = positionHours.Select(kvp => new PositionSummary
			{
				Position = kvp.Key,
				TotalHours = kvp.Value
			}).ToList();

			return new MonthlySummary
			{
				DaysWithShifts = daysWithShifts,
				TotalHoursWorked = employeeHours.Values.Sum(),
				EmployeeSummaries = employeeSummaries,
				PositionSummaries = positionSummaries
			};
		}



	}
}
