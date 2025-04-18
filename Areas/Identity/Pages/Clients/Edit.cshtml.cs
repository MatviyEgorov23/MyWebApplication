using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace WebApplication_AuthenticationSystem_.Areas.Identity.Pages.Clients
{
    public class EditModel : PageModel
    {

        public ClientInfo clientInfo = new ClientInfo();
        public String errorMessage = "";
        public String successMessage = "";

        public void OnGet()
        {
            String id = Request.Query["id"];

            try
            {
                String connectionString = "Data Source=DESKTOP-JH1JF57\\MSSQLSERVER01;Initial Catalog=AuthSystemDB;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    String sql = "SELECT firstname, lastname, email, id, phone FROM [dbo].[Table (AuthenticationSystem)] WHERE id = @id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                clientInfo.firstname = reader.GetString(0);
                                clientInfo.lastname = reader.GetString(1);
                                clientInfo.email = reader.GetString(2);
                                clientInfo.id = reader.GetString(3);
                                clientInfo.phone = reader.GetString(4);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
        }

        public void OnPost()
        {
            string id = Request.Query["id"];
            clientInfo.firstname = Request.Form["firstname"];
            clientInfo.lastname = Request.Form["lastname"];
            clientInfo.email = Request.Form["email"];
            clientInfo.phone = Request.Form["phone"];

            if (string.IsNullOrEmpty(clientInfo.firstname) ||
                string.IsNullOrEmpty(clientInfo.lastname) ||
                string.IsNullOrEmpty(clientInfo.email) ||
                string.IsNullOrEmpty(clientInfo.phone))
            {
                errorMessage = "All fields are required";
                return;
            }

            try
            {
                String connectionString = "Data Source=DESKTOP-JH1JF57\\MSSQLSERVER01;Initial Catalog=AuthSystemDB;Integrated Security=True;Trust Server Certificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    String sql = "UPDATE [dbo].[Table (AuthenticationSystem)] " +
                                 "SET firstname=@FirstName, lastname=@LastName, email=@Email, phone=@Phone " +
                                 "WHERE id=@Id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", clientInfo.firstname);
                        command.Parameters.AddWithValue("@LastName", clientInfo.lastname);
                        command.Parameters.AddWithValue("@Email", clientInfo.email);
                        command.Parameters.AddWithValue("@Id", id);
                        command.Parameters.AddWithValue("@Phone", clientInfo.phone);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            errorMessage = "Update failed. No records updated.";
                            return;
                        }
                        else
                        {
                            successMessage = "Client updated successfully.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return;
            }

            Response.Redirect("/Identity/Clients/Index");
        }
    }
}