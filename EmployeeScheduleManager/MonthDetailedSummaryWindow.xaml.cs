using EmployeeScheduleManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EmployeeScheduleManager
{
	/// <summary>
	/// Logika interakcji dla klasy Window1.xaml
	/// </summary>
	public partial class MonthDetailedSummaryWindow : Window
	{
		public MonthDetailedSummaryWindow(List<DaySummary> dane, HashSet<string> stanowiska)
		{
			InitializeComponent();

			// Dodajemy dynamiczne kolumny dla stanowisk
			foreach (var stanowisko in stanowiska)
			{
				PodsumowanieDataGrid.Columns.Add(new DataGridTextColumn
				{
					Header = stanowisko,
					Binding = new System.Windows.Data.Binding($"GodzinyNaStanowiskach[{stanowisko}]")
				});
			}

			PodsumowanieDataGrid.ItemsSource = dane;

		}
	}
}
