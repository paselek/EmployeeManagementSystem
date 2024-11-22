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
	/// Logika interakcji dla klasy ShiftDialog.xaml
	/// </summary>
	public partial class ShiftDialog : Window
	{
		public string StartTime { get; set; }
		public string EndTime { get; set; }
		public string EmployeeName { get; private set; } // Wybrany pracownik

		public ShiftDialog()
		{
			InitializeComponent();
		}

		public void InitializeData(string initialStartTime, string initialEndTime, List<Employee> availableEmployees)
		{
			StartTimeTextBox.Text = initialStartTime;
			EndTimeTextBox.Text = initialEndTime;

			EmployeeComboBox.ItemsSource = availableEmployees;
			EmployeeComboBox.SelectedIndex = 0; // Domyślny wybór
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			// Walidacja danych
			if (string.IsNullOrWhiteSpace(StartTimeTextBox.Text) || string.IsNullOrWhiteSpace(EndTimeTextBox.Text))
			{
				MessageBox.Show("Proszę wypełnić wszystkie pola!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			if (EmployeeComboBox.SelectedItem == null)
			{
				MessageBox.Show("Proszę wybrać pracownika!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			StartTime = StartTimeTextBox.Text;
			EndTime = EndTimeTextBox.Text;
			EmployeeName = EmployeeComboBox.SelectedItem as string;

			DialogResult = true;
			Close();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
