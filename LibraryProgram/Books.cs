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

            string query = "SELECT * FROM Books ORDER BY [BookName]";
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
        }
    }
}
