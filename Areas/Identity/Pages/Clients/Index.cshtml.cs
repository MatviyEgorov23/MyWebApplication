using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace WebApplication_AuthenticationSystem_.Areas.Identity.Pages.Clients
{
    [Authorize] // Список користувачів доступний лише авторизованому користувачу
    public class IndexModel : PageModel
    {
        public List<ClientInfo> listClients { get; set; } = new List<ClientInfo>();
        public void OnGet()
        {
            try
            {
                String connectionString = "Data Source=DESKTOP-JH1JF57\\MSSQLSERVER01;Initial Catalog=AuthSystemDB;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "SELECT firstname, lastname, email, id, phone FROM [dbo].[Table (AuthenticationSystem)]";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ClientInfo clientInfo = new ClientInfo { 
                                firstname = reader.GetString(0),
                                lastname = reader.GetString(1),
                                email = reader.GetString(2),
                                id = reader.GetString(3),
                                phone = reader.GetString(4)
                                };
                                listClients.Add(clientInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
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