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
            ShowFighterWeightClass();
        }


        #region Division_Region
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

        private void AddDivisionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DivisionTB.Text.ToString()))
            {
                DivisionTB.Background = Brushes.Pink;

                if (MessageBox.Show("Please Enter the Division in the correct format", "Error", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    DivisionTB.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B8CFD9"));
                }

                return;
            }
            else
            {
                DivisionTB.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B8CFD9"));

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

            DivisionTB.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B8CFD9"));

            DataRowView selectedRowView = ListDivisions.SelectedItem as DataRowView;

            if (selectedRowView != null)
            {

                string selectedDivision = selectedRowView["weight-class"].ToString();
                if (MessageBox.Show($"Are you sure tou want to delete the division {selectedDivision}?", "Warning!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        string query = "DELETE FROM WeightClass WHERE Id = @Id";
                        SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                        sqlConnection.Open();
                        sqlCommand.Parameters.AddWithValue("@Id", ListDivisions.SelectedValue);
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
                        ShowAllFightersDb();
                        ShowFighterWeightClass();
                    }
                }
            }
        }

        private void ListDivisions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListDivisions.SelectedValue != null)
            {
                ShowFightersInDivision();
            }
        }

        private void ShowFightersInDivision()
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
        private void ClearDivisionBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear Division Table?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {

                try
                {
                    string query = @"DELETE FROM WeightClass";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                    sqlConnection.Open();

                    sqlCommand.ExecuteNonQuery();

                    string updateTable = @"DBCC CHECKIDENT('WeightClass', RESEED, 0);";
                    //SqlCommand command = new SqlCommand(updateTable, sqlConnection);
                    //command.ExecuteNonQuery();

                    sqlCommand = new SqlCommand(updateTable, sqlConnection);
                    sqlCommand.ExecuteNonQuery();

                }
                catch
                {
                    MessageBox.Show($"Error: Clearing Division Table Failed! Try Again!");
                }
                finally
                {
                    sqlConnection.Close();
                    ShowDivisions();
                    ShowFighterWeightClass();
                }
            }
            else
            {
                return;
            }
        }

        #endregion


        #region Fighter_Region
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

        private void AddFighter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTB.Text.ToString()))
            {
                NameTB.Background = Brushes.Pink;
                if (MessageBox.Show("Please Enter the fighter name in the correct format", "Error", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    NameTB.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B8CFD9"));
                }
            }
            else if (string.IsNullOrWhiteSpace(NicknameTB.Text.ToString()))
            {
                //NicknameTB.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#DADADA"));
                NicknameTB.Background = Brushes.Pink;
                if (MessageBox.Show("Please Enter the fighter nickname in the correct format", "Error", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    NicknameTB.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B8CFD9"));
                }
            }
            else if (string.IsNullOrWhiteSpace(SurenameTB.Text.ToString()))
            {
                //SurenameTB.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#DADADA"));
                SurenameTB.Background = Brushes.Pink;
                if (MessageBox.Show("Please Enter the fighter surename in the correct format", "Error", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    SurenameTB.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B8CFD9"));
                }
            }
            else if (string.IsNullOrWhiteSpace(NationalityTB.Text.ToString()))
            {
                //NationalityTB.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#DADADA"));
                NationalityTB.Background = Brushes.Pink;
                if (MessageBox.Show("Please Enter the fighter nationality in the correct format", "Error", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    NationalityTB.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B8CFD9"));
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

                    NameTB.Text = string.Empty;
                    NicknameTB.Text = string.Empty;
                    SurenameTB.Text = string.Empty;
                    NationalityTB.Text = string.Empty;
                }
            }
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
                ShowFighterWeightClass();
            }
        }

        private void ShowAllFighters_Click(object sender, RoutedEventArgs e)
        {
            ShowAllFightersDb();
        }

        private void ClearFighter_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear Fighter Table?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    string query = @"DELETE FROM Fighter";

                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();

                    sqlCommand.ExecuteNonQuery();

                    sqlCommand = new SqlCommand(@"DBCC CHECKIDENT('Fighter', RESEED, 0);", sqlConnection);
                    sqlCommand.ExecuteNonQuery();
                }
                catch
                {
                    MessageBox.Show($"Error: Clearing Fighter Table Failed! Try Again!");
                }
                finally
                {
                    sqlConnection.Close();
                    ShowAllFightersDb();
                    ShowFighterWeightClass();
                }
            }
            else
            {
                return;
            }
        }

        private void UpdateFighter(DataRowView fighter)
        {
            //MessageBox.Show("This is Ghazaaal");

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

        //UpdateFighter Table
        private void FighterDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Check if the cell edit is committed
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Get the column name
                string columnName = e.Column.Header.ToString();

                //MessageBox.Show($"ColumnName : {columnName}");

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
                        MessageBox.Show($"Error: Value in column '{columnName}' has not changed or it is a whiteSpace!");
                        UpdateFighter(selectedRow);
                    }
                }
            }
        }

        #endregion


        #region FighterWEightClass_Region
        private void ShowFighterWeightClass()
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

        private void AddFighterInDivisionButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FighterIdTB.Text.ToString()) || !FighterIdTB.Text.ToString().All(char.IsDigit) || FighterIdTB.Text.ToString().All(char.IsWhiteSpace))
            {
                FighterIdTB.Background = Brushes.Pink;
                if (MessageBox.Show("Please Enter the fighter Id in the correct format", "Error", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    FighterIdTB.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B8CFD9"));
                }
            }
            else if (string.IsNullOrWhiteSpace(DivisionIdTB.Text.ToString()) || !DivisionIdTB.Text.ToString().All(char.IsDigit) || DivisionIdTB.Text.ToString().All(char.IsWhiteSpace))
            {
                DivisionIdTB.Background = Brushes.Pink;
                if (MessageBox.Show("Please Enter the fighter Id in the correct format", "Error", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    DivisionIdTB.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#B8CFD9"));
                }
            }
            else
            {
                //Check if table is empty. If it is, then Id Identity should start from 1
                try
                {
                    string query = @"SELECT * FROM FighterWeightClass";
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                    using (sqlDataAdapter)
                    {
                        DataTable fighterWeightClassDataTable = new DataTable();
                        sqlDataAdapter.Fill(fighterWeightClassDataTable);

                        if (fighterWeightClassDataTable.Rows.Count == 0)
                        {
                            //DBCC CHECKIDENT('FighterWeightClass', RESEED, 0);
                            string uptadeTable = @"DBCC CHECKIDENT('FighterWeightClass', RESEED, 0);";

                            SqlCommand command = new SqlCommand(uptadeTable, sqlConnection);

                            sqlConnection.Open();
                            command.ExecuteNonQuery();
                            sqlConnection.Close();
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show($"Error: Table Refresh Failed!");
                }


                try
                {
                    string query = @"INSERT INTO [FighterWeightClass] ([FighterId], [WeightClassId]) VALUES (@FighterId, @WeightClassId)";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();

                    sqlCommand.Parameters.AddWithValue("@FighterId", int.Parse(FighterIdTB.Text.ToString()));
                    sqlCommand.Parameters.AddWithValue("@WeightClassId", int.Parse(DivisionIdTB.Text.ToString()));
                    sqlCommand.ExecuteScalar();
                }
                catch
                {
                    MessageBox.Show($"Error: FighterWeightClass Insertion Failed! Try Again!");
                }
                finally
                {
                    sqlConnection.Close();
                    ShowFighterWeightClass();
                }

            }

        }

        private void DeleteFigtherFromDivisionButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string query = @"DELETE FROM FighterWeightClass WHERE Id = @Id";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Id", FighterWeightClassDataGrid.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch
            {
                MessageBox.Show($"Error: FighterWeightClass Deletion Failed! Try Again!");
            }
            finally
            {
                sqlConnection.Close();
                ShowFighterWeightClass();
            }
        }

        private void UpdateFighterWeightClass(DataRowView fighterWeightClass)
        {

            try
            {
                int id = (int)fighterWeightClass["Id"];
                int fighterId = fighterWeightClass["FighterId"] != DBNull.Value ? (int)fighterWeightClass["FighterId"] : 0;
                int weightClassId = fighterWeightClass["WeightClassId"] != DBNull.Value ? (int)fighterWeightClass["WeightClassId"] : 0;
                string query = @"
                    UPDATE FighterWeightClass
                    SET FighterId = @FighterId, WeightClassId = @WeightClassId
                    WHERE Id = @Id
                ";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@Id", id);
                    sqlCommand.Parameters.AddWithValue("@FighterId", fighterId);
                    sqlCommand.Parameters.AddWithValue("@WeightClassId", weightClassId);
                    sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: FighterWeightClass update failed. {ex.Message}");
            }
            finally
            {
                sqlConnection?.Close();
                ShowFighterWeightClass();
            }
        }

        //UpdateFighterWeightClass Table
        private void FighterWeightClassDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // Check if the cell edit is committed
            if (e.EditAction == DataGridEditAction.Commit)
            {
                // Get the column name
                string columnName = e.Column.Header.ToString();

                //MessageBox.Show($"ColumnName : {columnName}");

                // Get the row being edited
                DataRowView selectedRow = e.Row.Item as DataRowView;

                if (selectedRow != null)
                {
                    // Get the new value from the editing element
                    var editingElement = e.EditingElement as TextBox;
                    bool newValueBoolean = int.TryParse(editingElement?.Text.ToString(), out int newValue);

                    // Get the old value from the DataRowView
                    bool oldValueBoolean = int.TryParse(selectedRow[columnName].ToString(), out int oldValue);

                    // Compare old and new values
                    if (newValue != oldValue && newValueBoolean && oldValueBoolean)
                    {
                        // The value has changed
                        MessageBox.Show($"Value in column '{columnName}' has changed from '{oldValue}' to '{newValue}'.");

                        // Optionally, update the DataRowView with the new value
                        selectedRow[columnName] = newValue;
                        UpdateFighterWeightClass(selectedRow);
                    }
                    else
                    {
                        // The value has not changed
                        MessageBox.Show($"Error: Value in column '{columnName}' has not changed or it is a whiteSpace!");
                        UpdateFighterWeightClass(selectedRow);
                    }
                }
            }
        }

        #endregion


        #region Button_UI_Region
        private void AddFighterInDivisionButton_MouseLeave(object sender, MouseEventArgs e)
        {
            AddFighterInDivisionButton.Cursor = Cursors.Arrow;
        }

        private void AddFighterInDivisionButton_MouseEnter(object sender, MouseEventArgs e)
        {
            AddFighterInDivisionButton.Cursor = Cursors.Hand;
        }

        private void DeleteFigtherFromDivisionButton_MouseLeave(object sender, MouseEventArgs e)
        {
            DeleteFigtherFromDivisionButton.Cursor = Cursors.Arrow;
        }

        private void DeleteFigtherFromDivisionButton_MouseEnter(object sender, MouseEventArgs e)
        {
            DeleteFigtherFromDivisionButton.Cursor = Cursors.Hand;
        }

        private void ClearDivisionBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            ClearDivisionBtn.Cursor = Cursors.Hand;
        }

        private void ClearDivisionBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            ClearDivisionBtn.Cursor = Cursors.Hand;
        }
        #endregion

    }
}

//string updateTable = @"DBCC CHECKIDENT('WeightClass', RESEED, 0);";