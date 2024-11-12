using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EmployeeScheduleManager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly FirestoreTest _firestoreTest;
		private void AddEmployeeButton_Click(object sender, RoutedEventArgs e)
		{
			AddEmployeeWindow addEmployeeWindow = new AddEmployeeWindow();
			addEmployeeWindow.ShowDialog();
		}
		public MainWindow()
		{
			_firestoreTest = new FirestoreTest();
			InitializeComponent();
		}


		private async void RefreshEmployeeList_Click(object sender, RoutedEventArgs e)
		{
			// Pobieranie pracowników z Firestore
			var employees = await _firestoreTest.GetEmployeesAsync();

			// Przypisanie listy pracowników do DataGrid
			EmployeeListDataGrid.ItemsSource = employees;
		}
	}
}