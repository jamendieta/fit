using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace TaskManagement.Application.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    public async System.Threading.Tasks.Task RegisterUserAsync(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Username))
            throw new ArgumentException("Username is required");

        if (string.IsNullOrWhiteSpace(user.Email))
            throw new ArgumentException("Email is required");

        if (string.IsNullOrWhiteSpace(user.PasswordHash))
            throw new ArgumentException("Password is required");

        var existing = await _userRepository.GetByUsernameAsync(user.Username);
        if (existing != null)
            throw new InvalidOperationException("Username already exists");

        user.PasswordHash = HashPassword(user.PasswordHash);
        await _userRepository.AddAsync(user);
    }

    public async Task<User?> ValidateUserAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
            return null;

        var hashedPassword = HashPassword(password);
        return user.PasswordHash == hashedPassword ? user : null;
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}