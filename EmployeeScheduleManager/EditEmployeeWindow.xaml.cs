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
	/// Logika interakcji dla klasy EditEmployeeWindow.xaml
	/// </summary>
	public partial class EditEmployeeWindow : Window
	{
		private Employee _employee;
		public EditEmployeeWindow(Employee employee)
		{
			InitializeComponent();
			_employee = employee;

			// Wypełnij pola edycji danymi pracownika
			ImieTextBox.Text = _employee.imie;
			NazwiskoTextBox.Text = _employee.nazwisko;
			// XDDDDDD
			if (_employee.niedostepnosc["Poniedzialek"].Length==11)
			{
				PoniedzialekStartTextBox.Text = _employee.niedostepnosc["Poniedzialek"].Substring(0, 5);
				PoniedzialekEndTextBox.Text = _employee.niedostepnosc["Poniedzialek"].Substring(6,5);
			}
			if (_employee.niedostepnosc["Wtorek"].Length == 11)
			{
				WtorekStartTextBox.Text = _employee.niedostepnosc["Wtorek"].Substring(0, 5);
				WtorekEndTextBox.Text = _employee.niedostepnosc["Wtorek"].Substring(6, 5);
			}
			if (_employee.niedostepnosc["Sroda"].Length == 11)
			{
				SrodaStartTextBox.Text = _employee.niedostepnosc["Sroda"].Substring(0, 5);
				SrodaEndTextBox.Text = _employee.niedostepnosc["Sroda"].Substring(6, 5);
			}
			if (_employee.niedostepnosc["Czwartek"].Length == 11)
			{
				CzwartekStartTextBox.Text = _employee.niedostepnosc["Czwartek"].Substring(0, 5);
				CzwartekEndTextBox.Text = _employee.niedostepnosc["Czwartek"].Substring(6, 5);
			}
			if (_employee.niedostepnosc["Piatek"].Length == 11)
			{
				PiatekStartTextBox.Text = _employee.niedostepnosc["Piatek"].Substring(0, 5);
				PiatekEndTextBox.Text = _employee.niedostepnosc["Piatek"].Substring(6, 5);
			}
			if (_employee.niedostepnosc["Sobota"].Length == 11)
			{
				SobotaStartTextBox.Text = _employee.niedostepnosc["Sobota"].Substring(0, 5);
				SobotaEndTextBox.Text = _employee.niedostepnosc["Sobota"].Substring(6, 5);
			}
			if (_employee.niedostepnosc["Niedziela"].Length == 11)
			{
				NiedzielaStartTextBox.Text = _employee.niedostepnosc["Niedziela"].Substring(0, 5);
				NiedzielaEndTextBox.Text = _employee.niedostepnosc["Niedziela"].Substring(6, 5);
			}
			LoadLocations();

		}
		private async void LoadLocations()
		{
			FirestoreTest firestoreTest = new FirestoreTest();
			var locations = await firestoreTest.GetLocationsAsync(); 

			// Przypisz lokalizacje do ComboBox
			LocationComboBox.ItemsSource = locations;
			LocationComboBox.DisplayMemberPath = "nazwa";

			string employeeLocationId = _employee.lokalizacja;  // Przykładowe ID lokalizacji przypisanego pracownika

			// Znajdź lokalizację o tym ID w liście lokalizacji
			if (locations!=null && employeeLocationId != "")
			{ 
				Location? selectedLocation = locations.FirstOrDefault(l => l.Id == employeeLocationId);
				LocationComboBox.SelectedItem = selectedLocation;
			}

		}
		private async void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			_employee.imie = ImieTextBox.Text;
			_employee.nazwisko = NazwiskoTextBox.Text;

			

			// Collect unavailability data
			_employee.niedostepnosc = new Dictionary<string, string>
			{
				{ "Poniedzialek", PoniedzialekStartTextBox.Text + "-" + PoniedzialekEndTextBox.Text },
				{ "Wtorek", WtorekStartTextBox.Text + "-" + WtorekEndTextBox.Text },
				{ "Sroda", SrodaStartTextBox.Text + "-" + SrodaEndTextBox.Text },
				{ "Czwartek", CzwartekStartTextBox.Text + "-" + CzwartekEndTextBox.Text },
				{ "Piatek", PiatekStartTextBox.Text + "-" + PiatekEndTextBox.Text },
				{ "Sobota", SobotaStartTextBox.Text + "-" + SobotaEndTextBox.Text },
				{ "Niedziela", NiedzielaStartTextBox.Text + "-" + NiedzielaEndTextBox.Text }
			};
			var selectedLocation = LocationComboBox.SelectedItem as Location;
			// Sprawdzenie, czy wybrany element istnieje
			if (selectedLocation != null)
			{
				_employee.lokalizacja = selectedLocation.Id;  

			}

			if (IsValidInput(_employee.imie, _employee.nazwisko, _employee.niedostepnosc))
			{
				FirestoreTest firestoreTest = new FirestoreTest();
				await firestoreTest.UpdateEmployeeAsync(_employee);
				MessageBox.Show("Employee updated successfully!");
				this.DialogResult = true;
				this.Close();
			}
			else
			{
				MessageBox.Show("Please fill all fields.");
			}
		}

		private static bool IsValidInput(string firstName, string lastName, Dictionary<string, string> unavailability)
		{
			if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
				return false;

			foreach (var day in unavailability.Values)
			{
				//if (string.IsNullOrWhiteSpace(day))
				//return false;
			}

			return true;
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
