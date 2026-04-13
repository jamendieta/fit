using Microsoft.Data.Sqlite;
using TaskManagement.Core.Interfaces;
using TaskEntity = TaskManagement.Core.Entities.Task;

namespace TaskManagement.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly string _connectionString;

    public TaskRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<TaskEntity>> GetAllAsync()
    {
        var tasks = new List<TaskEntity>();
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Title, Description, Status, DueDate, UserId FROM Tasks";
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tasks.Add(new TaskEntity
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Description = reader.GetString(2),
                Status = (Core.Entities.TaskStatus)reader.GetInt32(3),
                DueDate = reader.GetDateTime(4),
                UserId = reader.GetInt32(5)
            });
        }
        return tasks;
    }

    public async Task<TaskEntity?> GetByIdAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Title, Description, Status, DueDate, UserId FROM Tasks WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new TaskEntity
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Description = reader.GetString(2),
                Status = (Core.Entities.TaskStatus)reader.GetInt32(3),
                DueDate = reader.GetDateTime(4),
                UserId = reader.GetInt32(5)
            };
        }
        return null;
    }

    public async Task<IEnumerable<TaskEntity>> GetByUserIdAsync(int userId)
    {
        var tasks = new List<TaskEntity>();
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Title, Description, Status, DueDate, UserId FROM Tasks WHERE UserId = @userId";
        command.Parameters.AddWithValue("@userId", userId);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tasks.Add(new TaskEntity
            {
                Id = reader.GetInt32(0),
                Title = reader.GetString(1),
                Description = reader.GetString(2),
                Status = (Core.Entities.TaskStatus)reader.GetInt32(3),
                DueDate = reader.GetDateTime(4),
                UserId = reader.GetInt32(5)
            });
        }
        return tasks;
    }

    public async Task AddAsync(TaskEntity task)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO Tasks (Title, Description, Status, DueDate, UserId) VALUES (@title, @description, @status, @dueDate, @userId); SELECT last_insert_rowid();";
        command.Parameters.AddWithValue("@title", task.Title);
        command.Parameters.AddWithValue("@description", task.Description);
        command.Parameters.AddWithValue("@status", (int)task.Status);
        command.Parameters.AddWithValue("@dueDate", task.DueDate);
        command.Parameters.AddWithValue("@userId", task.UserId);
        task.Id = Convert.ToInt32(await command.ExecuteScalarAsync());
    }

    public async Task UpdateAsync(TaskEntity task)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "UPDATE Tasks SET Title = @title, Description = @description, Status = @status, DueDate = @dueDate, UserId = @userId WHERE Id = @id";
        command.Parameters.AddWithValue("@id", task.Id);
        command.Parameters.AddWithValue("@title", task.Title);
        command.Parameters.AddWithValue("@description", task.Description);
        command.Parameters.AddWithValue("@status", (int)task.Status);
        command.Parameters.AddWithValue("@dueDate", task.DueDate);
        command.Parameters.AddWithValue("@userId", task.UserId);
        await command.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Tasks WHERE Id = @id";
        command.Parameters.AddWithValue("@id", id);
        await command.ExecuteNonQueryAsync();
    }
}