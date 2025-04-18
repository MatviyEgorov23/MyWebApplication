using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace WebApplication_AuthenticationSystem_.Areas.Identity.Pages.Clients
{
    public class DeleteModel : PageModel
    {
        [BindProperty]
        public ClientInfo Client { get; set; }

        public IActionResult OnGet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToPage("/Clients/Index");
            }

            try
            {
                // Подключение к базе данных
                String connectionString = "Data Source=DESKTOP-JH1JF57\\MSSQLSERVER01;Initial Catalog=AuthSystemDB;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Получение информации о клиенте
                    String sql = "SELECT firstname, lastname, email, id, phone FROM [dbo].[Table (AuthenticationSystem)] WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Client = new ClientInfo
                                {
                                    firstname = reader.GetString(0),
                                    lastname = reader.GetString(1),
                                    email = reader.GetString(2),
                                    id = reader.GetString(3),
                                    phone = reader.GetString(4)
                                };
                            }
                            else
                            {
                                return RedirectToPage("/Clients/Index");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                return RedirectToPage("/Clients/Index");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(Client.id))
            {
                return RedirectToPage("/Clients/Index");
            }

            try
            {
                String connectionString = "Data Source=DESKTOP-JH1JF57\\MSSQLSERVER01;Initial Catalog=AuthSystemDB;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Удаление клиента
                    String sql = "DELETE FROM [dbo].[Table (AuthenticationSystem)] WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", Client.id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                ModelState.AddModelError("", "Ошибка удаления клиента.");
                return Page();
            }

            return RedirectToPage("/Clients/Index");
        }

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
