using Microsoft.AspNetCore.Mvc;
using WebApplication_AuthenticationSystem_.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace WebApplication_AuthenticationSystem_.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public ProductsController(IWebHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            string fileName = "";

            if (product.ImageFile != null)
            {
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFile.FileName);
                string path = Path.Combine(_environment.WebRootPath, "products", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(fileStream);
                }
            }

            String connectionString = "Data Source=DESKTOP-JH1JF57\\MSSQLSERVER01;Initial Catalog=AuthSystemDB;Integrated Security=True;Trust Server Certificate=True";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = @"
                    INSERT INTO Products (Name, Brand, Category, Price, Description, ImageFileName, CreatedAt)
                    VALUES (@Name, @Brand, @Category, @Price, @Description, @ImageFileName, @CreatedAt)";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Brand", product.Brand);
                    command.Parameters.AddWithValue("@Category", product.Category);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@Description", product.Description);
                    command.Parameters.AddWithValue("@ImageFileName", fileName);
                    command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }

            return RedirectToAction("Index");
        }

        public IActionResult Index()
        {
            List<Product> products = new List<Product>();
            string connectionString = _configuration.GetConnectionString("AuthDbContextConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM Products";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Brand = reader["Brand"].ToString(),
                            Category = reader["Category"].ToString(),
                            Price = (decimal)reader["Price"],
                            Description = reader["Description"].ToString(),
                            CreatedAt = (DateTime)reader["CreatedAt"],
                            ImageFileName = reader["ImageFileName"].ToString(),
                            ImageFile = null 
                        });
                    }
                    connection.Close();
                }
            }

            return View(products);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            string connectionString = _configuration.GetConnectionString("AuthDbContextConnection");

            Product product = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM Products WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    var reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        product = new Product
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Brand = reader["Brand"].ToString(),
                            Category = reader["Category"].ToString(),
                            Price = (decimal)reader["Price"],
                            Description = reader["Description"].ToString(),
                            ImageFileName = reader["ImageFileName"].ToString()
                        };
                    }

                    connection.Close();
                }
            }

            if (product == null)
            {
                return RedirectToAction("Index");
            }

            return View("EditProduct", product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            string fileName = product.ImageFileName;

            // Если новое изображение не выбрано и старое имя пустое — достаём старое из БД
            if (product.ImageFile == null && string.IsNullOrEmpty(product.ImageFileName))
            {
                string conn = _configuration.GetConnectionString("AuthDbContextConnection");

                using (SqlConnection connection = new SqlConnection(conn))
                {
                    string sql = "SELECT ImageFileName FROM Products WHERE Id = @Id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", product.Id);
                        connection.Open();
                        var reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            fileName = reader["ImageFileName"].ToString();
                        }
                        connection.Close();
                    }
                }
            }

            if (product.ImageFile != null)
            {
                fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFile.FileName);
                string path = Path.Combine(_environment.WebRootPath, "products", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(fileStream);
                }
            }

            string connectionString = _configuration.GetConnectionString("AuthDbContextConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = @"
            UPDATE Products
            SET Name = @Name, Brand = @Brand, Category = @Category, Price = @Price, Description = @Description, ImageFileName = @ImageFileName
            WHERE Id = @Id";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", product.Id);
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Brand", product.Brand);
                    command.Parameters.AddWithValue("@Category", product.Category);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@Description", product.Description ?? "");
                    command.Parameters.AddWithValue("@ImageFileName", fileName ?? ""); 

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("AuthDbContextConnection");

            Product product = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT * FROM Products WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    var reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        product = new Product
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Brand = reader["Brand"].ToString(),
                            Category = reader["Category"].ToString(),
                            Price = (decimal)reader["Price"],
                            Description = reader["Description"].ToString(),
                            ImageFileName = reader["ImageFileName"].ToString()
                        };
                    }
                }
            }

            if (product == null)
            {
                return RedirectToAction("Index");
            }

            return View("DeleteProduct", product);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Product product)
        {
            string connectionString = _configuration.GetConnectionString("AuthDbContextConnection");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM Products WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", product.Id);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }

            return RedirectToAction("Index");
        }
    }
}