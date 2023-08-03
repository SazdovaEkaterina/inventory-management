using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.API.Models.Entities;

public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public List<Product> Products { get; set; } = new List<Product>();

    public User(string userName, string firstName, string lastName) 
        : base(userName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public User() : base()
    {
        
    }
}