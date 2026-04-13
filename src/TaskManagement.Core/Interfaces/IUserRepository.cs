using TaskManagement.Core.Entities;

namespace TaskManagement.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    System.Threading.Tasks.Task AddAsync(User user);
    System.Threading.Tasks.Task UpdateAsync(User user);
    System.Threading.Tasks.Task DeleteAsync(int id);
}