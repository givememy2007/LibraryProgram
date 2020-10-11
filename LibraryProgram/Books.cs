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

        //Формы для новых окон
        Students students;
        HandedOutBooks handedOutBooks;

        //Обновление таблицы с данными при актикации окна
        protected override void OnActivated(EventArgs e)
        {
            dataGridView1.Rows.Clear();
            LoadData();
        }

        //Загрузка данных в DataGridView
        private void LoadData()
        {
            //Подключение
            string connectString = ConfigurationManager.ConnectionStrings["LibraryProgram"].ConnectionString;
            Console.WriteLine(connectString);
            SqlConnection myConnection = new SqlConnection(connectString);
            myConnection.Open();
            
            //Запрос "Только АКТИВНЫЕ книги"
            string query = "SELECT * FROM Books " +
                "WHERE [Avaliability] > 0 " + 
                 "ORDER BY [BookName]";

            SqlCommand command = new SqlCommand(query, myConnection);
            SqlDataReader reader = command.ExecuteReader();
            List<string[]> data = new List<string[]>();

            while (reader.Read())
            {
                data.Add(new string[5]);

                data[data.Count - 1][0] = reader[0].ToString();
                data[data.Count - 1][1] = reader[1].ToString();
                data[data.Count - 1][2] = reader[2].ToString();
                data[data.Count - 1][3] = reader[3].ToString();
                data[data.Count - 1][4] = reader[4].ToString();
            }

            reader.Close();
            myConnection.Close();

            foreach (string[] s in data)
                dataGridView1.Rows.Add(s);
        }

        private void Books_Load(object sender, EventArgs e)
        {

        }

        //Просмотр студентов
        private void button3_Click(object sender, EventArgs e)
        {
            students = new Students();
            students.Show();
        }

        //Переход на форму для просмотра выданных книг/возврата книг
        private void button2_Click(object sender, EventArgs e)
        {
            handedOutBooks = new HandedOutBooks();
            handedOutBooks.Show();
            dataGridView1.Rows.Clear();
            LoadData();
        }

        //Выдача книги
        private void button1_Click(object sender, EventArgs e)
        {

            string message = "Выберите одну книгу для выдачи";
            string caption = "Ошибка при выдаче";
            int rowCount = dataGridView1.SelectedCells.Count;
            if (rowCount != 1)
                MessageBox.Show(message, caption); //если выбрали НЕ 1 книгу
            else
            {
                //Получение данных о книге из выбранной строки
                int selectedRowIndex = dataGridView1.SelectedCells[0].RowIndex;
                DataGridViewRow selectedRow = dataGridView1.Rows[selectedRowIndex];
                int bookId = Convert.ToInt32(selectedRow.Cells[0].Value);
                string bookName = Convert.ToString(selectedRow.Cells[1].Value);
                string autorName = Convert.ToString(selectedRow.Cells[2].Value);

                //Вызов окна для выбора студента, которому выдаём книгу
                //В конструктор передаём данные книги для составления записи
                students = new Students(bookId, bookName, autorName);
                students.Show();
                
            }  
        }
    }
}
