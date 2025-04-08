using FirebaseAdmin.Auth.Hash;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
using BCrypt.Net;

namespace EmployeeScheduleManager
{
	/// <summary>
	/// Logika interakcji dla klasy LoginWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{
		private readonly FirestoreDb _firestoreDb;

		public string LoggedInUser { get; private set; }

		public LoginWindow()
		{
			InitializeComponent();
			_firestoreDb = FirestoreDb.Create("employeemanagementsystem-132ea");
		}

		private async void LoginButton_Click(object sender, RoutedEventArgs e)
		{
			string username = UsernameTextBox.Text.Trim();
			string password = PasswordBox.Password;

			bool isAuthenticated = await AuthenticateUser(username, password);

			if (isAuthenticated)
			{
				LoggedInUser = username;
				DialogResult = true;
				Close();
			}
			else
			{
				MessageBox.Show("Nieprawidłowa nazwa użytkownika lub hasło.", "Błąd logowania", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		private async Task<bool> AuthenticateUser(string username, string password)
		{
			CollectionReference adminsRef = _firestoreDb.Collection("Logins");
			QuerySnapshot snapshot = await adminsRef.WhereEqualTo("username", username).GetSnapshotAsync();

			if (!snapshot.Any()) return false;

			var doc = snapshot.First();
			string storedHash = doc.GetValue<string>("passwordHash");
			string role = doc.GetValue<string>("role");

			if (role == "admin")
				return BCrypt.Net.BCrypt.Verify(password, storedHash);
			else
				return false;
		}
	}
}
