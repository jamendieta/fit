using Microsoft.Data.Sqlite;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _connectionString;

    public UserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Username, PasswordHash, Email FROM Users WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                Email = reader.GetString(3)
            };
        }
        return null;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Username, PasswordHash, Email FROM Users WHERE Username = @username";
        command.Parameters.AddWithValue("@username", username);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new User
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                PasswordHash = reader.GetString(2),
                Email = reader.GetString(3)
            };
        }
        return null;
    }

    public async System.Threading.Tasks.Task AddAsync(User user)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Users (Username, PasswordHash, Email) VALUES (@username, @passwordHash, @email); SELECT last_insert_rowid();";
        command.Parameters.AddWithValue("@username", user.Username);
        command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@email", user.Email);
        user.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async System.Threading.Tasks.Task UpdateAsync(User user)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Users SET Username = @username, PasswordHash = @passwordHash, Email = @email WHERE Id = @id";
        command.Parameters.AddWithValue("@id", user.Id);
        command.Parameters.AddWithValue("@username", user.Username);
        command.Parameters.AddWithValue("@passwordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@email", user.Email);
        await command.ExecuteNonQueryAsync();
    }

    public async System.Threading.Tasks.Task DeleteAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Users WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        await command.ExecuteNonQueryAsync();
    }
}