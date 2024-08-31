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
            FighterWeightClass();
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

        private void ShowAllFightersDb()
        {
            try
            {
                string query = @"SELECT * FROM Fighter";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable fighterTable = new DataTable();
                    sqlDataAdapter.Fill(fighterTable);

                    FighterDataGrid.SelectedValuePath = "Id";
                    FighterDataGrid.ItemsSource = fighterTable.DefaultView;
                }

            }
            catch
            {
                MessageBox.Show("Error: Showing all fighters Failed! Try Again!");
            }
        }

        private void AddDivisionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DivisionTB.Text.ToString()))
            {
                DivisionTB.Background = Brushes.Pink;
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
            catch
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
            if (string.IsNullOrWhiteSpace(NameTB.Text.ToString()))
            {
                NameTB.Background = Brushes.Pink;
                if (MessageBox.Show("Please Enter the fighter name in the correct format", "Error", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    NameTB.Background = Brushes.Transparent;
                }
            }
            else if (string.IsNullOrWhiteSpace(NicknameTB.Text.ToString()))
            {
                //NicknameTB.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#DADADA"));
                NicknameTB.Background = Brushes.Pink;
                if (MessageBox.Show("Please Enter the fighter nickname in the correct format", "Error", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    NicknameTB.Background = Brushes.Transparent;
                }
            }
            else if (string.IsNullOrWhiteSpace(SurenameTB.Text.ToString()))
            {
                //SurenameTB.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#DADADA"));
                SurenameTB.Background = Brushes.Pink;
                if (MessageBox.Show("Please Enter the fighter surename in the correct format", "Error", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    SurenameTB.Background = Brushes.Transparent;
                }
            }
            else if (string.IsNullOrWhiteSpace(NationalityTB.Text.ToString()))
            {
                //NationalityTB.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#DADADA"));
                NationalityTB.Background = Brushes.Pink;
                if (MessageBox.Show("Please Enter the fighter nationality in the correct format", "Error", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    NationalityTB.Background = Brushes.Transparent;
                }
            }
            else
            {
                try
                {
                    string name = NameTB.Text.ToString();
                    string nickname = NicknameTB.Text.ToString();
                    string surename = SurenameTB.Text.ToString();
                    string nationality = NationalityTB.Text.ToString();

                    string query = @"INSERT INTO Fighter 
                                    (name, nickname, surename, nationality)
                                    VALUES(@name, @nickname, @surename, @nationality) ";

                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                    sqlConnection.Open();
                    sqlCommand.Parameters.Add("@name", name);
                    sqlCommand.Parameters.Add("@nickname", nickname);
                    sqlCommand.Parameters.Add("@surename", surename);
                    sqlCommand.Parameters.Add("@nationality", nationality);
                    sqlCommand.ExecuteScalar();

                }
                catch
                {
                    MessageBox.Show($"Fighter Insertion Failed! Try Again!");
                }
                finally
                {
                    sqlConnection.Close();
                    ShowAllFightersDb();
                }
            }
        }

        private void ShowAllFighters_Click(object sender, RoutedEventArgs e)
        {
            ShowAllFightersDb();
        }

        private void DeleteFighter_Click(object sender, RoutedEventArgs e)
        {

            //MessageBox.Show($"From Fighter Delete button! Fighter Id:{FighterDataGrid.SelectedValue}");
            try
            {
                string query = @"DELETE FROM Fighter WHERE Id = @Id";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Id", FighterDataGrid.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch
            {
                MessageBox.Show($"Fighter Deletion Failed! Try Again!");
            }
            finally
            {
                sqlConnection.Close();
                ShowAllFightersDb();
            }
        }

        private void FighterWeightClass()
        {
            try
            {
                string query = @"SELECT * FROM FighterWeightClass";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable fighterWeightClassTable = new DataTable();
                    sqlDataAdapter.Fill(fighterWeightClassTable);

                    FighterWeightClassDataGrid.SelectedValuePath = "Id";
                    FighterWeightClassDataGrid.ItemsSource = fighterWeightClassTable.DefaultView;
                }

            }
            catch (Exception)
            {
                MessageBox.Show($"Error: FighterWeightClass");
            }
        }

        private void UpdateFighter(DataRowView fighter)
        {
            //MessageBox.Show("This is Ghazaaal");

            //string info = $"Fighter: {fighter["Id"].ToString()} {fighter["Name"].ToString()} {fighter["Nickname"].ToString()} {fighter["Surename"].ToString()} {fighter["Nationality"].ToString()}";
            //MessageBox.Show(info);

            try
            {
                int fighterId = (int)fighter["Id"];

                string name = fighter["Name"] != DBNull.Value ? fighter["Name"].ToString() : string.Empty;
                string nickname = fighter["Nickname"] != DBNull.Value ? fighter["Nickname"].ToString() : string.Empty;
                string surename = fighter["Surename"] != DBNull.Value ? fighter["Surename"].ToString() : string.Empty;
                string nationality = fighter["Nationality"] != DBNull.Value ? fighter["Nationality"].ToString() : string.Empty;

                string query = @"
                    UPDATE Fighter
                    SET name = @Name, nickname = @Nickname, surename = @Surename, nationality = @Nationality
                    WHERE Id = @Id
                ";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@Id", fighterId);
                    sqlCommand.Parameters.AddWithValue("@Name", name);
                    sqlCommand.Parameters.AddWithValue("@Nickname", nickname);
                    sqlCommand.Parameters.AddWithValue("@Surename", surename);
                    sqlCommand.Parameters.AddWithValue("@Nationality", nationality);

                    sqlCommand.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: Fighter update failed. {ex.Message}");
            }
            finally
            {
                sqlConnection.Close();
                ShowAllFightersDb();
            }
        }


        private void FighterDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Check if the cell edit is committed
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Get the column name
                string columnName = e.Column.Header.ToString();

                MessageBox.Show($"ColumnName : {columnName}");

                // Get the row being edited
                DataRowView selectedRow = e.Row.Item as DataRowView;

                if (selectedRow != null)
                {
                    // Get the new value from the editing element
                    var editingElement = e.EditingElement as TextBox;
                    string newValue = editingElement?.Text;

                    // Get the old value from the DataRowView
                    string oldValue = selectedRow[columnName].ToString();

                    // Compare old and new values
                    if (newValue != oldValue && !string.IsNullOrWhiteSpace(newValue))
                    {
                        // The value has changed
                        MessageBox.Show($"Value in column '{columnName}' has changed from '{oldValue}' to '{newValue}'.");

                        // Optionally, update the DataRowView with the new value
                        selectedRow[columnName] = newValue;
                        UpdateFighter(selectedRow);
                    }
                    else
                    {
                        // The value has not changed
                        MessageBox.Show($"Value in column '{columnName}' has not changed or it is a whiteSpace.");
                        UpdateFighter(selectedRow);
                    }
                }
            }
        }

    }
}
