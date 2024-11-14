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
		public List<string> grafiki { get; set; }= new List<string>(); // Grafiki (Array)
		[FirestoreProperty]
		public string nazwa { get; set; } = ""; // Nazwa lokalizacji
		[FirestoreProperty]
		public List<string> pracownicy { get; set; } = new List<string>(); // Pracownicy (Array)
		[FirestoreProperty]
		public List<string> stanowiska { get; set; } = new List<string>();
	}
}
