using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private static readonly List<User> Users = new();

    [HttpPost("register")]
    public IActionResult Register(RegisterUserDto registerUserDto)
    {
        var user = new User
        {
            Id = Users.Count + 1,
            StudentNumber = registerUserDto.StudentNumber,
            FirstName = registerUserDto.FirstName,
            LastName = registerUserDto.LastName,
            Email = registerUserDto.Email,
            Password = registerUserDto.Password,
            Role = registerUserDto.Role
        };

        Users.Add(user);

        return Ok(user);
    }

    [HttpPost("login")]
    public IActionResult Login(LoginDto loginDto)
    {
        var user = Users.FirstOrDefault(u =>
            u.Email == loginDto.Email &&
            u.Password == loginDto.Password);

        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        return Ok(new
        {
            Message = "Login successful",
            User = user
        });
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        return Ok(Users);
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        return Ok(user);
    }
}