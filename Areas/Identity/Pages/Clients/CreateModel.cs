using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace WebApplication_AuthenticationSystem_.Areas.Identity.Pages.Clients 
{
    public class CreateModel : PageModel 
    { 
        // Свойства для информации о клиенте
        [BindProperty]
        public ClientInfo clientInfo { get; set; } = new ClientInfo();

        // Сообщения об ошибках и успешных действиях
        public string? errorMessage { get; set; }
        public string? successMessage { get; set; }

        // Метод для обработки данных формы (POST-запрос)
        public IActionResult OnPost()
        {
            // Проверка на обязательное заполнение полей
            if (string.IsNullOrEmpty(clientInfo.firstname) || string.IsNullOrEmpty(clientInfo.lastname) ||
            string.IsNullOrEmpty(clientInfo.email) || string.IsNullOrEmpty(clientInfo.id) || string.IsNullOrEmpty(clientInfo.phone))
            {
                errorMessage = "All fields must be completed.";
                return Page();
            }

            // Валидация email
            if (!clientInfo.email.Contains("@"))
            {
                errorMessage = "Please enter a valid email address.";
                return Page();
            }

            try
            {
                // Сохранение данных в базу данных
                String connectionString = "Data Source=DESKTOP-JH1JF57\\MSSQLSERVER01;Initial Catalog=AuthSystemDB;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "INSERT INTO [dbo].[Table (AuthenticationSystem)] (firstname, lastname, email, id, phone) VALUES (@FirstName, @LastName, @Email, @Id, @Phone)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", clientInfo.firstname);
                        command.Parameters.AddWithValue("@LastName", clientInfo.lastname);
                        command.Parameters.AddWithValue("@Email", clientInfo.email);
                        command.Parameters.AddWithValue("@Id", clientInfo.id);
                        command.Parameters.AddWithValue("@Phone", clientInfo.phone);

                        command.ExecuteNonQuery();
                    }
                }

                // Сообщение об успешном создании пользователя
                successMessage = "New user added correctly";
                errorMessage = null;  // Сброс ошибки
            }
            catch (Exception ex)
            {
                errorMessage = "Error adding user: " + ex.Message;
            }

            return Page();
        }
        // Класс, содержащий информацию о клиенте
        public class ClientInfo
        {
            public string? firstname { get; set; }
            public string? lastname { get; set; }
            public string? email { get; set; }
            public string? id { get; set; }
            public string? phone { get; set; }
        }
    }
}