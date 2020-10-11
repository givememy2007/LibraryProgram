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
        //Поля для хранения переданных данных о выдаваемой книге
        private int bookId;
        private string bookName;
        private string autorName;

        //При стандартном вызове ПРОСМОТРА кнопка выдачи - неактивна
        public Students()
        {
            InitializeComponent();
            LoadData();
            button1.Hide();
        }

        //Перегрузка конструктора. Получение и запись данных о книге
        public Students(int newBookId, string newBookName, string newAutorName)
        {
            InitializeComponent();
            LoadData();
            button1.Show();
            bookId = newBookId;
            bookName = newBookName;
            autorName = newAutorName;
        }
        
        //Запись данных в DGV
        private void LoadData()
        {
            string connectString = ConfigurationManager.ConnectionStrings["LibraryProgram"].ConnectionString;
            Console.WriteLine(connectString);
            SqlConnection myConnection = new SqlConnection(connectString);
            myConnection.Open();
            
            //Запрос "все студенты" сортировка по ФИО
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

        //По кнопке "Выдача" (уже имеются данные книги) продолжаем алгоритм выдачи
        private void button1_Click(object sender, EventArgs e)
        {
            string message = "Выберите одного студента для выдачи";
            string caption = "Ошибка при выдаче";
            int rowCount = dataGridView1.SelectedCells.Count;
            if (rowCount != 1)
                MessageBox.Show(message, caption); //Выбрали неправильно
            else
            {
                //Получение ФИО из активной строки
                int selectedRowIndex = dataGridView1.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];
                string studentName = Convert.ToString(selectedRow.Cells[1].Value);

                //Создание подключения и команд для выполнения
                string connectString = ConfigurationManager.ConnectionStrings["LibraryProgram"].ConnectionString;
                Console.WriteLine(connectString);
                SqlConnection myConnection = new SqlConnection(connectString);
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;

                //Формируем новую запись в таблице HandedOutBooks о взятой книге из полученных ранее данных
                cmd.CommandText = "INSERT HandedOutBooks " +
                    "([BookId], [BookName], [AutorName], [StudentName], [TimeOfIssue]) " +
                    "VALUES (@bookId, @bookName, @autorName, @studentName, @TimeOfIssue)";
                cmd.Parameters.AddWithValue("@bookId", bookId);
                cmd.Parameters.AddWithValue("@bookName", bookName);
                cmd.Parameters.AddWithValue("@autorName", autorName);
                cmd.Parameters.AddWithValue("@studentName", studentName);
                cmd.Parameters.AddWithValue("@TimeOfIssue", DateTime.Today);
                cmd.Connection = myConnection;

                //Открыли, записали, закрыли
                myConnection.Open();
                cmd.ExecuteNonQuery();
                myConnection.Close();
                
                //Команда для того, чтобы выданную книгу сделать неактивной
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = " UPDATE Books " +
                    " SET [Avaliability] = [Avaliability] - 1 " +
                    " WHERE [Id] = @bookId ";
                cmd.Connection = myConnection;

                myConnection.Open();
                cmd.ExecuteNonQuery();
                myConnection.Close();

                //Закрытие фармы выдачи
                this.Close();
            }
        }
    }
}
