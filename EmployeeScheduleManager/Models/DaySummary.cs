using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeScheduleManager.Models
{
	public class DaySummary
	{
		public string Dzien { get; set; } 
		public double LacznaLiczbaGodzin { get; set; }
		public int LiczbaPracownikow { get; set; }
		public bool PokrycieOdOtwarciaDoZamkniecia { get; set; }
		public Dictionary<string, double> GodzinyNaStanowiskach { get; set; } // Nazwa stanowiska -> liczba godzin
	}

	public class DetailedMonthSummary
	{
		public List<DaySummary> Dni { get; set; }
		public HashSet<string> Stanowiska { get; set; }
	}
}
