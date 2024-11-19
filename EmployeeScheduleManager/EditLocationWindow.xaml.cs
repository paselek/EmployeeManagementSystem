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
	/// Logika interakcji dla klasy EditLocationWindow.xaml
	/// </summary>
	public partial class EditLocationWindow : Window
	{
		private Location _location;
		public EditLocationWindow(Location location)
		{
			InitializeComponent();
			_location = location;

			// Wypełnij pola edycji danymi pracownika
			NazwaTextBox.Text = _location.nazwa;
			AdresTextBox.Text = _location.adres;
			// XDDDDDD
			if (_location.godzinyOtwarcia["poniedzialek"].Length == 11)
			{
				PoniedzialekStartTextBox.Text = _location.godzinyOtwarcia["poniedzialek"].Substring(0, 5);
				PoniedzialekEndTextBox.Text = _location.godzinyOtwarcia["poniedzialek"].Substring(6, 5);
			}
			if (_location.godzinyOtwarcia["wtorek"].Length == 11)
			{
				WtorekStartTextBox.Text = _location.godzinyOtwarcia["wtorek"].Substring(0, 5);
				WtorekEndTextBox.Text = _location.godzinyOtwarcia["wtorek"].Substring(6, 5);
			}
			if (_location.godzinyOtwarcia["sroda"].Length == 11)
			{
				SrodaStartTextBox.Text = _location.godzinyOtwarcia["sroda"].Substring(0, 5);
				SrodaEndTextBox.Text = _location.godzinyOtwarcia["sroda"].Substring(6, 5);
			}
			if (_location.godzinyOtwarcia["czwartek"].Length == 11)
			{
				CzwartekStartTextBox.Text = _location.godzinyOtwarcia["czwartek"].Substring(0, 5);
				CzwartekEndTextBox.Text = _location.godzinyOtwarcia["czwartek"].Substring(6, 5);
			}
			if (_location.godzinyOtwarcia["piatek"].Length == 11)
			{
				PiatekStartTextBox.Text = _location.godzinyOtwarcia["piatek"].Substring(0, 5);
				PiatekEndTextBox.Text = _location.godzinyOtwarcia["piatek"].Substring(6, 5);
			}
			if (_location.godzinyOtwarcia["sobota"].Length == 11)
			{
				SobotaStartTextBox.Text = _location.godzinyOtwarcia["sobota"].Substring(0, 5);
				SobotaEndTextBox.Text = _location.godzinyOtwarcia["sobota"].Substring(6, 5);
			}
			if (_location.godzinyOtwarcia["niedziela"].Length == 11)
			{
				NiedzielaStartTextBox.Text = _location.godzinyOtwarcia["niedziela"].Substring(0, 5);
				NiedzielaEndTextBox.Text = _location.godzinyOtwarcia["niedziela"].Substring(6, 5);
			}
			string stanowiska = string.Join(",", _location.stanowiska);
			StanowiskaTextBox.Text = stanowiska;
		}

		private async void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			_location.nazwa = NazwaTextBox.Text;
			_location.adres = AdresTextBox.Text;

			// Collect unavailability data
			_location.godzinyOtwarcia = new Dictionary<string, string>
			{
				{ "poniedzialek", PoniedzialekStartTextBox.Text + "-" + PoniedzialekEndTextBox.Text },
				{ "wtorek", WtorekStartTextBox.Text + "-" + WtorekEndTextBox.Text },
				{ "sroda", SrodaStartTextBox.Text + "-" + SrodaEndTextBox.Text },
				{ "czwartek", CzwartekStartTextBox.Text + "-" + CzwartekEndTextBox.Text },
				{ "piatek", PiatekStartTextBox.Text + "-" + PiatekEndTextBox.Text },
				{ "sobota", SobotaStartTextBox.Text + "-" + SobotaEndTextBox.Text },
				{ "niedziela", NiedzielaStartTextBox.Text + "-" + NiedzielaEndTextBox.Text }
			};
			_location.stanowiska = StanowiskaTextBox.Text.Split(',').ToList();

			if (IsValidInput(_location))
			{
				FirestoreTest firestoreTest = new FirestoreTest();
				await firestoreTest.UpdateLocationAsync(_location);
				MessageBox.Show("Location edited successfully!");
				this.DialogResult = true;
				this.Close();
			}
			else
			{
				MessageBox.Show("Please fill all fields.");
			}
		}

		private static bool IsValidInput(Location location)
		{

			foreach (var day in location.godzinyOtwarcia.Values)
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
