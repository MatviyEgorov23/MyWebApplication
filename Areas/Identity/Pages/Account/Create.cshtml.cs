using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using WebApplication_AuthenticationSystem_.Areas.Identity.Pages.Clients;

public class CreateModel : PageModel
{
    public ClientInfo clientInfo = new ClientInfo();
    public String errorMessage = "";
    public String successMessage = "";

    public void OnGet()
    {

    }

    public void OnPost()
    {
        clientInfo.firstname = Request.Form["firstname"]!;
        clientInfo.lastname = Request.Form["lastname"]!;
        clientInfo.email = Request.Form["Email"]!;
        clientInfo.id = Request.Form["ID"]!;
        clientInfo.phone = Request.Form["phone"]!;

        if (clientInfo.firstname.Length == 0 || clientInfo.lastname.Length == 0 ||
            clientInfo.email.Length == 0 || clientInfo.id.Length == 0 || clientInfo.phone.Length == 0)
        {
            errorMessage = "All the fields are required";
            return;
        }

        clientInfo.firstname = ""; clientInfo.lastname = ""; clientInfo.email = ""; clientInfo.id = ""; clientInfo.phone = "";
        successMessage = "New user added correctly";
    }
}