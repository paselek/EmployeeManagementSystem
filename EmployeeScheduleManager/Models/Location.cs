using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeScheduleManager.Models
{
	[FirestoreData]
	public class Location
	{
		[FirestoreDocumentId]
		public string Id { get; set; } = "";
		[FirestoreProperty]
		public string adres { get; set; } = ""; // Adres (string) 
		[FirestoreProperty]
		public Dictionary<string, string> godzinyOtwarcia { get; set; } = new Dictionary<string, string>(); // Godziny otwarcia (Map)
		[FirestoreProperty]
		public string nazwa { get; set; } = ""; // Nazwa lokalizacji
		[FirestoreProperty]
		public int liczbaPracownikow { get; set; } = 0;
		[FirestoreProperty]
		public List<string> stanowiska { get; set; } = new List<string>();
	}
}
