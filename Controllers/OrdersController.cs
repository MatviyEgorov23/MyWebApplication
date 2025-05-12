using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebApplication_AuthenticationSystem_.Models;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace WebApplication_AuthenticationSystem_.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IConfiguration _configuration;

        public OrdersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Authorize] // чтобы только авторизованные пользователи могли видеть заказы
        public IActionResult Index()
        {
            List<Order> orders = new List<Order>();
            string conn = _configuration.GetConnectionString("AuthDbContextConnection");

            // Получение email текущего пользователя
            string userEmail = User.FindFirstValue(ClaimTypes.Email);

            using (SqlConnection connection = new SqlConnection(conn))
            {
                string sql = @"
            SELECT o.*, p.Name, p.Price
            FROM Orders o
            JOIN Products p ON o.ProductId = p.Id
            WHERE o.UserEmail = @UserEmail";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserEmail", userEmail);

                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        orders.Add(new Order
                        {
                            Id = (int)reader["Id"],
                            ProductId = (int)reader["ProductId"],
                            Product = new Product
                            {
                                Name = reader["Name"].ToString(),
                                Price = (decimal)reader["Price"]
                            },
                            UserEmail = reader["UserEmail"].ToString(),
                            PostalCode = reader["PostalCode"].ToString(),
                            PaymentMethod = reader["PaymentMethod"].ToString(),
                            OrderDate = (DateTime)reader["OrderDate"]
                        });
                    }
                }
            }

            return View("Orders", orders);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Cancel(int id)
        {
            string conn = _configuration.GetConnectionString("AuthDbContextConnection");

            using (SqlConnection connection = new SqlConnection(conn))
            {
                string sql = "DELETE FROM Orders WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            return RedirectToAction("Index");
        }

    }
}
