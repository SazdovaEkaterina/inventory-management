using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventoryManagement.API.Models;
using InventoryManagement.API.Models.Entities;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManagement.API.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AuthenticationController(
        IConfiguration configuration,
        UserManager<User> userManager, 
        RoleManager<IdentityRole> roleManager)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("first_name", user.FirstName));
            claimsForToken.Add(new Claim("last_name", user.LastName));
            
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claimsForToken.Add(new Claim(ClaimTypes.Role, role));
            }
        
            var jwtSecurityToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                expiration = jwtSecurityToken.ValidTo
            });
        }

        return Unauthorized();
    }
    
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        // If the user already exists.
        var userExists = await _userManager.FindByNameAsync(model.UserName);
        if (userExists != null)
        {
            // The request could not be completed due to a conflict with the current state of the resource.
            return Conflict();
        }

        var user = new User()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            UserName = model.UserName
        };
        
        var createdUser = await _userManager.CreateAsync(user, model.Password);
        if (!createdUser.Succeeded)
        {
            return BadRequest();
        }
        
        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))  
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));  
        if (!await _roleManager.RoleExistsAsync(UserRoles.User))  
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));  
        
        if (await _roleManager.RoleExistsAsync(UserRoles.User))  
        {  
            await _userManager.AddToRoleAsync(user, UserRoles.User);  
        }

        return Ok();
    }

    [HttpPost]
    [Route("register-admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.UserName);
        if (userExists != null)
        {
            return Conflict();
        }
        
        var user = new User()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            UserName = model.UserName
        };
        
        var createdUser = await _userManager.CreateAsync(user, model.Password);
        if (!createdUser.Succeeded)
        {
            return BadRequest();
        }
        
        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))  
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));  
        if (!await _roleManager.RoleExistsAsync(UserRoles.User))  
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));  
  
        if (await _roleManager.RoleExistsAsync(UserRoles.Admin))  
        {  
            await _userManager.AddToRoleAsync(user, UserRoles.Admin);  
        }

        return Ok();
    }
}