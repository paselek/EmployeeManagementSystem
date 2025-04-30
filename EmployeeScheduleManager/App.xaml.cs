using System;
using System.Windows;
using FirebaseAdmin;
using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;

namespace EmployeeScheduleManager
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	/// 
	public partial class App : Application
	{
		protected override async void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// Inicjalizacja Firebase
			string path = AppDomain.CurrentDomain.BaseDirectory + @"firebase-config.json";
			Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
			// Utworzenie instancji FirestoreTest
			FirestoreTest firestoreTest = new FirestoreTest();
			// Wywołanie asynchronicznej metody do dodania dokumentu
			await firestoreTest.DodajDokumentPrzykladowy();

			var loginWindow = new LoginWindow();
			Current.MainWindow = loginWindow;
			bool? result = loginWindow.ShowDialog();

			if (result == true)
			{
				MainWindow mainWindow = new MainWindow();
				Current.MainWindow = mainWindow;
				mainWindow.Show();
			}
			else
			{
				Application.Current.Shutdown(); // zamknij aplikację jeśli logowanie nie powiodło się
			}
		}


	}

}
