using Microsoft.AspNetCore.Mvc;
using UserService.DTOs;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _repository;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IUserRepository repository,
        ILogger<AuthController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterUserDto registerUserDto)
    {
        var existingEmail =
            await _repository.GetByEmailAsync(registerUserDto.Email);

        if (existingEmail != null)
        {
            _logger.LogWarning(
                "Registration failed. Email already exists: {Email}",
                registerUserDto.Email);

            return Conflict(
                "A user with this email already exists.");
        }

        var existingStudentNumber =
            await _repository.GetByStudentNumberAsync(
                registerUserDto.StudentNumber);

        if (existingStudentNumber != null)
        {
            _logger.LogWarning(
                "Registration failed. Student number already exists: {StudentNumber}",
                registerUserDto.StudentNumber);

            return Conflict(
                "A user with this student number already exists.");
        }

        var user = new User
        {
            StudentNumber = registerUserDto.StudentNumber,
            FirstName = registerUserDto.FirstName,
            LastName = registerUserDto.LastName,
            Email = registerUserDto.Email,
            Password = registerUserDto.Password,
            Role = registerUserDto.Role,
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _repository.CreateAsync(user);

        _logger.LogInformation(
            "User registered successfully. UserId: {UserId}, StudentNumber: {StudentNumber}, Role: {Role}",
            createdUser.Id,
            createdUser.StudentNumber,
            createdUser.Role);

        return Ok(createdUser);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginDto loginDto)
    {
        var user =
            await _repository.GetByEmailAsync(loginDto.Email);

        if (user == null || user.Password != loginDto.Password)
        {
            _logger.LogWarning(
                "Login failed for email: {Email}",
                loginDto.Email);

            return Unauthorized(
                "Invalid email or password.");
        }

        _logger.LogInformation(
            "User login successful. UserId: {UserId}, Role: {Role}",
            user.Id,
            user.Role);

        return Ok(new
        {
            Message = "Login successful",
            User = user
        });
    }
}