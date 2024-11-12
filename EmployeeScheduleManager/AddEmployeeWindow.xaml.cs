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
	/// Logika interakcji dla klasy AddEmployeeWindow.xaml
	/// </summary>
	/// 
	
	public partial class AddEmployeeWindow : Window
	{
		public AddEmployeeWindow()
		{
			InitializeComponent();
		}

		private async void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			string firstName = ImieTextBox.Text;
			string lastName = NazwiskoTextBox.Text;

			// Collect unavailability data
			var unavailability = new Dictionary<string, string>
			{
				{ "Poniedzialek", PoniedzialekStartTextBox.Text + "-" + PoniedzialekEndTextBox.Text },
				{ "Wtorek", WtorekStartTextBox.Text + "-" + WtorekEndTextBox.Text },
				{ "Sroda", SrodaStartTextBox.Text + "-" + SrodaEndTextBox.Text },
				{ "Czwartek", CzwartekStartTextBox.Text + "-" + CzwartekEndTextBox.Text },
				{ "Piatek", PiatekStartTextBox.Text + "-" + PiatekEndTextBox.Text },
				{ "Sobota", SobotaStartTextBox.Text + "-" + SobotaEndTextBox.Text },
				{ "Niedziela", NiedzielaStartTextBox.Text + "-" + NiedzielaEndTextBox.Text }
			};

			if (IsValidInput(firstName, lastName, unavailability))
			{
				FirestoreTest firestoreTest = new FirestoreTest();
				await firestoreTest.AddEmployee(firstName, lastName, unavailability);
				MessageBox.Show("Employee added successfully!");
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
