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
using EmployeeScheduleManager.Models;

namespace EmployeeScheduleManager
{
	/// <summary>
	/// Logika interakcji dla klasy AddLocationWindow.xaml
	/// </summary>
	public partial class AddLocationWindow : Window
	{
		public AddLocationWindow()
		{
			InitializeComponent();
		}

		private async void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			string nazwa = NazwaTextBox.Text;
			string adres = AdresTextBox.Text;

			// Collect unavailability data
			var openTime = new Dictionary<string, string>
			{
				{ "poniedzialek", PoniedzialekStartTextBox.Text + "-" + PoniedzialekEndTextBox.Text },
				{ "wtorek", WtorekStartTextBox.Text + "-" + WtorekEndTextBox.Text },
				{ "sroda", SrodaStartTextBox.Text + "-" + SrodaEndTextBox.Text },
				{ "czwartek", CzwartekStartTextBox.Text + "-" + CzwartekEndTextBox.Text },
				{ "piatek", PiatekStartTextBox.Text + "-" + PiatekEndTextBox.Text },
				{ "sobota", SobotaStartTextBox.Text + "-" + SobotaEndTextBox.Text },
				{ "niedziela", NiedzielaStartTextBox.Text + "-" + NiedzielaEndTextBox.Text }
			};
			string[] listaStanowisk = StanowiskaTextBox.Text.Split(',');


			if (IsValidInput(openTime))
			{
				FirestoreTest firestoreTest = new FirestoreTest();
				await firestoreTest.AddLocation(nazwa, adres, openTime,listaStanowisk);
				MessageBox.Show("Location added successfully!");
				this.DialogResult = true;
				this.Close();
			}
			else
			{
				MessageBox.Show("Please fill all fields.");
			}
		}

		private static bool IsValidInput(Dictionary<string, string> openTime)
		{

			foreach (var day in openTime.Values)
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
