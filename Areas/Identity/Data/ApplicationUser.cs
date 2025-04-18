using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication_AuthenticationSystem_.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string FirstName { get; set; } = null!;

    [PersonalData]
    [Column(TypeName = "nvarchar(100)")]
    public string LastName { get; set; } = null!;
}

public class UserStore
{
    public static List<ApplicationUser> Users { get; } = new List<ApplicationUser>();
}
