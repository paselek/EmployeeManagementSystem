using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeScheduleManager.Models
{
	/*class ScheduleEntry
    {
		[FirestoreDocumentId]
		public string Id { get; set; } = "";
		[FirestoreProperty]
		public string Start { get; set; } = "";
		[FirestoreProperty]
		public string End { get; set; } = "";
		[FirestoreProperty]
		public string Position { get; set; } = "";
	}*/
	public class ScheduleEntry
	{
		public string EmployeeName { get; set; }
		public string Position { get; set; }
		public int StartHour { get; set; }
		public int StartMinute { get; set; }
		public int EndHour { get; set; }
		public int EndMinute { get; set; }
	}
}
