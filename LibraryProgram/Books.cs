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
    public partial class Books : Form
    {

        public Books()
        {
            InitializeComponent();
            LoadData();
            students = new Students();
            handedOutBooks = new HandedOutBooks();
        }
        Students students;
        HandedOutBooks handedOutBooks;

        private void LoadData()
        {

            string connectString = ConfigurationManager.ConnectionStrings["LibraryProgram"].ConnectionString;
            Console.WriteLine(connectString);
            SqlConnection myConnection = new SqlConnection(connectString);
            myConnection.Open();

            string query = "SELECT * FROM Books " +
                "WHERE [Avaliability] > 0 " +
                 "ORDER BY [BookName]";

            SqlCommand command = new SqlCommand(query, myConnection);
            SqlDataReader reader = command.ExecuteReader();
            List<string[]> data = new List<string[]>();

            while (reader.Read())
            {
                data.Add(new string[6]);

                data[data.Count - 1][0] = reader[0].ToString();
                data[data.Count - 1][1] = reader[1].ToString();
                data[data.Count - 1][2] = reader[2].ToString();
                data[data.Count - 1][3] = reader[3].ToString();
                data[data.Count - 1][4] = reader[4].ToString();
                data[data.Count - 1][5] = reader[5].ToString();
            }

            reader.Close();
            myConnection.Close();

            foreach (string[] s in data)
                dataGridView1.Rows.Add(s);
        }

        private void Books_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            students = new Students();
            students.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            handedOutBooks = new HandedOutBooks();
            handedOutBooks.Show();
            dataGridView1.Rows.Clear();
            LoadData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string message = "Выберите одну книгу для выдачи";
            string caption = "Ошибка при выдаче";
            int rowCount = dataGridView1.SelectedCells.Count;
            if (rowCount != 1)
                MessageBox.Show(message, caption);
            else
            {
                int selectedRowIndex = dataGridView1.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];
                
                int bookId = Convert.ToInt32(selectedRow.Cells[0].Value);
                string bookName = Convert.ToString(selectedRow.Cells[1].Value);
                string autorName = Convert.ToString(selectedRow.Cells[2].Value);
  /*              
                message = bookName + " " + autorName;
                                caption = bookId.ToString();

                                MessageBox.Show(message, caption);
    */            

                students = new Students(bookId, bookName, autorName);
                students.Show();
                
/*
                string connectString = ConfigurationManager.ConnectionStrings["Library_Test_program_for_work"].ConnectionString;
                Console.WriteLine(connectString);

                SqlConnection myConnection = new SqlConnection(connectString);
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT Handed_out_books " +
                    "([Название книги], [ФИО студента], [Дата выдачи]) " +
                    "VALUES (@bookName, @studentName, @handDate)";
                cmd.Parameters.AddWithValue("@bookName", bookName);
                cmd.Parameters.AddWithValue("@studentName", studentName);
                cmd.Parameters.AddWithValue("@handDate", DateTime.Today);
                cmd.Connection = myConnection;

                myConnection.Open();
                cmd.ExecuteNonQuery();
                myConnection.Close();


                cmd.CommandType = CommandType.Text;
                cmd.CommandText = " UPDATE Books " +
                    " SET [Количество] = [Количество] - 1 " +
                    " WHERE [Название книги] = @bookName1 ";
                cmd.Parameters.AddWithValue("@bookName1", bookName);
                cmd.Connection = myConnection;

                myConnection.Open();
                cmd.ExecuteNonQuery();
                myConnection.Close();
*/


            }
            
                
        }

        
    }
}
