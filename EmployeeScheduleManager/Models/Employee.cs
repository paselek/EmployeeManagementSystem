using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EmployeeScheduleManager.Models
{
	[FirestoreData]
	public class Employee
	{
		[FirestoreDocumentId]
		public string Id { get; set; } = "";
		[FirestoreProperty]
		public string imie { get; set; } = "";
		[FirestoreProperty]
		public string nazwisko { get; set; } = "";
		[FirestoreProperty]
		public string lokalizacja { get; set; } = "";
		[FirestoreProperty]
		public Dictionary<string, string> niedostepnosc { get; set; } = new Dictionary<string, string>();
		[JsonIgnore]
		public string FullName => $"{imie} {nazwisko}";
	}
}
