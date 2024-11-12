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

		public async Task AddEmployee(string firstName, string lastName, Dictionary<string, string> unavailability)
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
				{ "niedostepnosc", unavailability }
			};

			// Add the new employee document to Firestore
			await employeesCollection.Document(newEmployeeId).SetAsync(employeeData);

			System.Diagnostics.Debug.WriteLine($"Employee {firstName} {lastName} added with ID {newEmployeeId}");
		}
	}
}
