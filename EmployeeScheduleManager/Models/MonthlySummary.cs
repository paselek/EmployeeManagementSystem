using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeScheduleManager.Models
{
	public class MonthlySummary
	{
		public int DaysWithShifts { get; set; }
		public double TotalHoursWorked { get; set; }
		public List<PositionSummary>? PositionSummaries { get; set; }
		public List<EmployeeSummary>? EmployeeSummaries { get; set; }
	}

	public class PositionSummary
	{
		public string? Position { get; set; }
		public double TotalHours { get; set; }
	}

	public class EmployeeSummary
	{
		public string? EmployeeName { get; set; }
		public double TotalHours { get; set; }
		public int DaysWorked { get; set; }
		public int DaysOff { get; set; }
	}

}
