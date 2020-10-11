using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace LibraryProgram
{
    public partial class HandedOutBooks : Form
    {
        public HandedOutBooks()
        {
            InitializeComponent();
            LoadData();
        }

        //Запись данных в DGV
        private void LoadData()
        {

            string connectString = ConfigurationManager.ConnectionStrings["LibraryProgram"].ConnectionString;
            Console.WriteLine(connectString);
            SqlConnection myConnection = new SqlConnection(connectString);
            myConnection.Open();

            //Все по ID
            string query = "SELECT * FROM HandedOutBooks ORDER BY [Id]";

            SqlCommand command = new SqlCommand(query, myConnection);
            SqlDataReader reader = command.ExecuteReader();
            List<string[]> data = new List<string[]>();

            while (reader.Read())
            {
                data.Add(new string[7]);

                data[data.Count - 1][0] = reader[0].ToString();
                data[data.Count - 1][1] = reader[1].ToString();
                data[data.Count - 1][2] = reader[2].ToString();
                data[data.Count - 1][3] = reader[3].ToString();
                data[data.Count - 1][4] = reader[4].ToString();
                data[data.Count - 1][5] = reader[5].ToString();
                data[data.Count - 1][6] = reader[6].ToString();
            }

            reader.Close();

            myConnection.Close();

            foreach (string[] s in data)
                dataGridView1.Rows.Add(s);
        }

        //По нажатию на возврат выбранной книги
        private void button1_Click(object sender, EventArgs e)
        {
            string message = "Выберите одну книгу для возврата";
            string caption = "Ошибка при возврате";
            int rowCount = dataGridView1.SelectedCells.Count;
            if (rowCount != 1)
                MessageBox.Show(message, caption); //не то количество строк выбрали
            else
            {
                //Получение Данных о возвращаемой книге и возвращающем студенте
                int selectedRowIndex = dataGridView1.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];
                int id = Convert.ToInt32(selectedRow.Cells[0].Value);
                int bookId = Convert.ToInt32(selectedRow.Cells[1].Value);
                string bookName = Convert.ToString(selectedRow.Cells[2].Value);
                string studentName = Convert.ToString(selectedRow.Cells[4].Value);
                DateTime date = Convert.ToDateTime(selectedRow.Cells[5].Value);

                int delta = DateTime.Today.Day - date.Day;//Условие просрочки. По умолчанию сделал этот срок - 7 дней

                //MessBox с подтверждением
                message = "Возвращаемая книга: " + bookName + "\nВозвращающий студент: " + studentName;
                caption = "Подтверждение";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;
                result = MessageBox.Show(message, caption, buttons);

                if (result == DialogResult.Yes)
                {
                    string connectString = ConfigurationManager.ConnectionStrings["LibraryProgram"].ConnectionString;
                    Console.WriteLine(connectString);
                    SqlConnection myConnection = new SqlConnection(connectString);
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    //Обновление таблицы ВЫДАННЫХ книг. Добавления даты сдачи
                    cmd.CommandText = " UPDATE HandedOutBooks " +
                        " SET [ReturnTime] = @ReturnTime " +
                        " WHERE [Id] = @Id ";
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@ReturnTime", DateTime.Today);
                    cmd.Connection = myConnection;

                    myConnection.Open();
                    cmd.ExecuteNonQuery();
                    myConnection.Close();

                    //Обновление таблицы КНИГ. Возвращение конкретной книге состояния "можно взять" для отображения
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = " UPDATE Books " +
                        " SET [Avaliability] = 1 " +
                        " WHERE [Id] = @bookId ";
                    cmd.Parameters.AddWithValue("@bookId", bookId);
                    cmd.Connection = myConnection;

                    myConnection.Open();
                    cmd.ExecuteNonQuery();
                    myConnection.Close();

                    //Если студент просрочил сдачу
                    if (delta > 7)
                    {
                        //Обновление таблицы СТУДЕНТОВ. Добавление 1 к количеству его просрочек.
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = " UPDATE Students " +
                            " SET [Delays] = [Delays] + 1 " +
                            " WHERE [StudentName] = @studentName ";
                        cmd.Parameters.AddWithValue("@studentName", studentName);
                        cmd.Connection = myConnection;

                        myConnection.Open();
                        cmd.ExecuteNonQuery();
                        myConnection.Close();
                    }
                }
                //Обновить данные
                dataGridView1.Rows.Clear();
                LoadData();
            } 
        }
    }
}
