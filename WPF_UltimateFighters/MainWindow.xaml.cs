using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_UltimateFighters
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);

            ShowDivisions();
        }

        private void ShowDivisions()
        {
            try
            {
                string query = @"SELECT * FROM WeightClass";
                SqlDataAdapter adapter = new SqlDataAdapter(query, sqlConnection);

                using (adapter)
                {
                    DataTable divisionTable = new DataTable();
                    adapter.Fill(divisionTable);

                    ListDivisions.DisplayMemberPath = "weight-class";
                    ListDivisions.SelectedValuePath = "Id";
                    ListDivisions.ItemsSource = divisionTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ListDivisions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListDivisions.SelectedValue != null)
            {
                ShowFighters();
            }
        }

        private void ShowFighters()
        {
            try
            {
                string query = @"SELECT * FROM Fighter f INNER JOIN FighterWeightClass fwc ON f.Id = fwc.FighterId WHERE WeightClassId = @weightClassId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand);

                using (adapter)
                {
                    sqlCommand.Parameters.AddWithValue("@weightClassId", ListDivisions.SelectedValue);
                    DataTable fighterTable = new DataTable();
                    adapter.Fill(fighterTable);
                    FighterDataGrid.ItemsSource = fighterTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.ToString()}");
            }
        }

        private void AddDivisionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DivisionTB.Text.ToString()))
            {
                DivisionTB.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#DADADA"));
                return;
            }
            else
            {
                DivisionTB.Background = Brushes.Transparent;

                try
                {
                    string query = "INSERT INTO WeightClass ([weight-class]) VALUES (@division)";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@division", DivisionTB.Text.ToString());
                    sqlCommand.ExecuteScalar();
                }
                catch
                {
                    MessageBox.Show("Insertion Failed!");
                }
                finally
                {
                    sqlConnection.Close();
                    ShowDivisions();
                }

            }

        }

        private void DeleteDivisionBtn_Click(object sender, RoutedEventArgs e)
        {

            DivisionTB.Background = Brushes.Transparent;

            try
            {
                string query = "DELETE FROM WeightClass WHERE Id = @Id";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Id", Convert.ToInt32(ListDivisions.SelectedValue));
                sqlCommand.ExecuteScalar();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Deletion Failed! Try Again!");
            }
            finally
            {
                sqlConnection.Close();
                ShowDivisions();
            }
        }

        private void AddFighter_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
