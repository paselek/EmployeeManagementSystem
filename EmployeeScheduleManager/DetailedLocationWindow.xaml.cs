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
using Google.Cloud.Firestore;

namespace EmployeeScheduleManager
{
	/// <summary>
	/// Logika interakcji dla klasy DetailedLocationWindow.xaml
	/// </summary>
	public partial class DetailedLocationWindow : Window
	{
		private Location _location;
		private readonly FirestoreTest _firestoreDb;
		public DetailedLocationWindow(Location location )
		{
			InitializeComponent();
			_firestoreDb = new FirestoreTest();
			_location = location;
			NazwaTextBlock.Text = _location.nazwa;
			AdresTextBlock.Text = _location.adres;

			var wynik = new StringBuilder();

			foreach (var para in _location.godzinyOtwarcia)
			{
				wynik.AppendLine($"{para.Key}: {para.Value}");
			}

			var OtwarcieString = wynik.ToString();
			GodzinyOtwarciaTextBlock.Text = OtwarcieString;
			StanowiskaListBox.ItemsSource = _location.stanowiska;
			LoadEmployees();
		}
		private async void LoadEmployees()
		{
			var employees = await _firestoreDb.GetEmployeesFromLocation(_location.Id);
			PracownicyDataGrid.ItemsSource = employees;
		}
	}
}
