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
    public partial class Students : Form
    {
        private int bookId;
        private string bookName;
        private string autorName;

        public Students()
        {
            InitializeComponent();
            LoadData();
            button1.Hide();
        }

        public Students(int newBookId, string newBookName, string newAutorName)
        {
            InitializeComponent();
            LoadData();
            button1.Show();
            bookId = newBookId;
            bookName = newBookName;
            autorName = newAutorName;

        }
        
        private void LoadData()
        {

            string connectString = ConfigurationManager.ConnectionStrings["LibraryProgram"].ConnectionString;
            Console.WriteLine(connectString);

            SqlConnection myConnection = new SqlConnection(connectString);

            myConnection.Open();
            

            string query = "SELECT * FROM Students ORDER BY [StudentName]";

            SqlCommand command = new SqlCommand(query, myConnection);

            SqlDataReader reader = command.ExecuteReader();

            List<string[]> data = new List<string[]>();

            while (reader.Read())
            {
                data.Add(new string[3]);

                data[data.Count - 1][0] = reader[0].ToString();
                data[data.Count - 1][1] = reader[1].ToString();
                data[data.Count - 1][2] = reader[2].ToString();
            }

            reader.Close();

            myConnection.Close();

            foreach (string[] s in data)
                dataGridView1.Rows.Add(s);
        }

        private void Students_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string message = "Выберите одного студента для выдачи";
            string caption = "Ошибка при выдаче";
            int rowCount = dataGridView1.SelectedCells.Count;
            if (rowCount != 1)
                MessageBox.Show(message, caption);
            else
            {
                int selectedRowIndex = dataGridView1.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];
                
                string studentName = Convert.ToString(selectedRow.Cells[1].Value);

                /*
                                message = bookName + " " + autorName + " " + studentName;
                                caption = bookId.ToString();

                                MessageBox.Show(message, caption);

                      */

                string connectString = ConfigurationManager.ConnectionStrings["LibraryProgram"].ConnectionString;
                Console.WriteLine(connectString);

                SqlConnection myConnection = new SqlConnection(connectString);
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT HandedOutBooks " +
                    "([BookId], [BookName], [AutorName], [StudentName], [TimeOfIssue]) " +
                    "VALUES (@bookId, @bookName, @autorName, @studentName, @TimeOfIssue)";
                cmd.Parameters.AddWithValue("@bookId", bookId);
                cmd.Parameters.AddWithValue("@bookName", bookName);
                cmd.Parameters.AddWithValue("@autorName", autorName);
                cmd.Parameters.AddWithValue("@studentName", studentName);
                cmd.Parameters.AddWithValue("@TimeOfIssue", DateTime.Today);
                cmd.Connection = myConnection;

                myConnection.Open();
                cmd.ExecuteNonQuery();
                myConnection.Close();

                
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = " UPDATE Books " +
                    " SET [Avaliability] = [Avaliability] - 1 " +
                    " WHERE [Id] = @bookId ";
             //   cmd.Parameters.AddWithValue("@bookId", bookId);
                cmd.Connection = myConnection;

                myConnection.Open();
                cmd.ExecuteNonQuery();
                myConnection.Close();
                


                this.Close();




            }
        }
    }
}
