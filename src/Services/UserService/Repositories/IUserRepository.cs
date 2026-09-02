using UserService.Models;

namespace UserService.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();

    Task<User?> GetByIdAsync(int id);

    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByStudentNumberAsync(string studentNumber);

    Task<User> CreateAsync(User user);

    Task<bool> UpdateAsync(int id, User user);

    Task<bool> DeleteAsync(int id);
}