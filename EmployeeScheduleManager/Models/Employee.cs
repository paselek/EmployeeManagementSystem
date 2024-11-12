using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeScheduleManager.Models
{
	[FirestoreData]
	public class Employee
	{
		[FirestoreDocumentId]
		public string Id { get; set; }
		[FirestoreProperty]
		public string imie { get; set; }
		[FirestoreProperty]
		public string nazwisko { get; set; } = "";
		//public string? Lokalizacja { get; set; }
		[FirestoreProperty]
		public Dictionary<string, string> niedostepnosc { get; set; } = new Dictionary<string, string>();
	}
}
