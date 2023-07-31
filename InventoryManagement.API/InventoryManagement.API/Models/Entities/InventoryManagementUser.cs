using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.API.Models.Entities;

public class InventoryManagementUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public InventoryManagementUser(string userName, string firstName, string lastName) 
        : base(userName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public InventoryManagementUser() : base()
    {
        
    }
}