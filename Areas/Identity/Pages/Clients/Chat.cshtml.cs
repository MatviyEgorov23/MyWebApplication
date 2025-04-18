using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using WebApplication_AuthenticationSystem_.Models;

namespace WebApplication_AuthenticationSystem_.Areas.Identity.Pages.Clients
{
    [Authorize]
    public class ChatModel : PageModel
    {
        public List<ChatMessage> Messages { get; set; } = new();

        [BindProperty]
        public string NewMessage { get; set; }

        [BindProperty]
        public int EditMessageId { get; set; }

        [BindProperty]
        public string EditMessageText { get; set; }

        private readonly string connectionString = "Data Source=DESKTOP-JH1JF57\\MSSQLSERVER01;Initial Catalog=AuthSystemDB;Integrated Security=True;Trust Server Certificate=True";

        public void OnGet()
        {
            LoadMessages();
        }

        public IActionResult OnPostSend()
        {
            if (!string.IsNullOrWhiteSpace(NewMessage))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                using SqlConnection connection = new(connectionString);
                connection.Open();

                string insertQuery = "INSERT INTO ChatMessages (UserId, MessageText) VALUES (@UserId, @MessageText)";
                using SqlCommand cmd = new(insertQuery, connection);
                cmd.Parameters.AddWithValue("@UserId", userId);
                cmd.Parameters.AddWithValue("@MessageText", NewMessage);
                cmd.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "DELETE FROM ChatMessages WHERE Id = @Id AND UserId = @UserId";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = "UPDATE ChatMessages SET MessageText = @MessageText, UpdatedAt = GETDATE() WHERE Id = @Id AND UserId = @UserId";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@MessageText", EditMessageText);
                    cmd.Parameters.AddWithValue("@Id", EditMessageId);
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage();
        }

        public void LoadMessages()
        {
            using SqlConnection connection = new(connectionString);
            connection.Open();

            string sql = "SELECT Id, UserId, MessageText, CreatedAt, UpdatedAt FROM ChatMessages ORDER BY CreatedAt DESC";
            using SqlCommand cmd = new(sql, connection);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Messages.Add(new ChatMessage
                {
                    Id = reader.GetInt32(0),
                    UserId = reader.GetString(1),
                    MessageText = reader.GetString(2),
                    CreatedAt = reader.GetDateTime(3),
                    UpdatedAt = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
                });
            }
        }
    }
}
