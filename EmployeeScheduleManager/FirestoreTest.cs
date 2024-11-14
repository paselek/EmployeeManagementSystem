using System;
using System.Threading.Tasks;
using FirebaseAdmin;
using Google.Cloud.Firestore;
using EmployeeScheduleManager.Models;

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
			var kolekcja = _firestoreDb.Collection("PrzykladowaKolekcja");
			var dokument = kolekcja.Document("PrzykladowyDokument");

			await dokument.SetAsync(new { Name = "Test", CreatedAt = Timestamp.GetCurrentTimestamp() });
			System.Diagnostics.Debug.WriteLine("Dodano dokument do Firestore!");
		}

		public async Task<List<Employee>> GetEmployeesAsync()
		{
			var employees = new List<Employee>();

			// Pobieranie dokumentów z kolekcji 'Pracownicy'
			var snapshot = await _firestoreDb.Collection("Pracownicy").GetSnapshotAsync();

			foreach (var document in snapshot.Documents)
			{
				Employee employee = document.ConvertTo<Employee>();
				employees.Add(employee);
			}

			return employees;
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
	}
}
