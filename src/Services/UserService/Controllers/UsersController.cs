using Microsoft.AspNetCore.Mvc;
using UserService.Models;
using UserService.Repositories;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserRepository repository,
        ILogger<UsersController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _repository.GetAllAsync();

        _logger.LogInformation(
            "Retrieved {UserCount} users",
            users.Count());

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _repository.GetByIdAsync(id);

        if (user == null)
        {
            _logger.LogWarning(
                "User lookup failed. UserId: {UserId}",
                id);

            return NotFound("User not found.");
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(User user)
    {
        var existingEmail =
            await _repository.GetByEmailAsync(user.Email);

        if (existingEmail != null)
        {
            _logger.LogWarning(
                "User creation failed. Email already exists: {Email}",
                user.Email);

            return Conflict(
                "A user with this email already exists.");
        }

        var existingStudentNumber =
            await _repository.GetByStudentNumberAsync(
                user.StudentNumber);

        if (existingStudentNumber != null)
        {
            _logger.LogWarning(
                "User creation failed. Student number already exists: {StudentNumber}",
                user.StudentNumber);

            return Conflict(
                "A user with this student number already exists.");
        }

        user.Id = 0;
        user.CreatedAt = DateTime.UtcNow;

        var createdUser = await _repository.CreateAsync(user);

        _logger.LogInformation(
            "User created. UserId: {UserId}",
            createdUser.Id);

        return CreatedAtAction(
            nameof(GetUserById),
            new { id = createdUser.Id },
            createdUser);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(
        int id,
        User user)
    {
        var updated =
            await _repository.UpdateAsync(id, user);

        if (!updated)
        {
            _logger.LogWarning(
                "User update failed. UserId: {UserId}",
                id);

            return NotFound("User not found.");
        }

        _logger.LogInformation(
            "User updated. UserId: {UserId}",
            id);

        var updatedUser =
            await _repository.GetByIdAsync(id);

        return Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var deleted =
            await _repository.DeleteAsync(id);

        if (!deleted)
        {
            _logger.LogWarning(
                "User deletion failed. UserId: {UserId}",
                id);

            return NotFound("User not found.");
        }

        _logger.LogInformation(
            "User deleted. UserId: {UserId}",
            id);

        return NoContent();
    }
}