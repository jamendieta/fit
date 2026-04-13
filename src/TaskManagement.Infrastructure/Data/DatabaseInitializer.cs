using Microsoft.Data.Sqlite;

namespace TaskManagement.Infrastructure.Data;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task InitializeAsync()
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var createTablesCommand = connection.CreateCommand();
        createTablesCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL,
                Email TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Tasks (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Description TEXT NOT NULL,
                Status INTEGER NOT NULL DEFAULT 0,
                DueDate TEXT NOT NULL,
                UserId INTEGER NOT NULL,
                FOREIGN KEY (UserId) REFERENCES Users(Id)
            );
        ";
        await createTablesCommand.ExecuteNonQueryAsync();

        // Seed data
        await SeedDataAsync(connection);
    }

    private async Task SeedDataAsync(SqliteConnection connection)
    {
        var adminHash = "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9";

        // Ensure demo admin keeps the documented credentials.
        var updateAdminCommand = connection.CreateCommand();
        updateAdminCommand.CommandText = "UPDATE Users SET PasswordHash = @hash WHERE Username = 'admin'";
        updateAdminCommand.Parameters.AddWithValue("@hash", adminHash);
        await updateAdminCommand.ExecuteNonQueryAsync();

        // Check if users exist
        var checkUsersCommand = connection.CreateCommand();
        checkUsersCommand.CommandText = "SELECT COUNT(*) FROM Users";
        var userCount = Convert.ToInt32(await checkUsersCommand.ExecuteScalarAsync());

        if (userCount == 0)
        {
            // Insert sample user
            var insertUserCommand = connection.CreateCommand();
            insertUserCommand.CommandText = "INSERT INTO Users (Username, PasswordHash, Email) VALUES ('admin', @hash, 'admin@example.com')";
            insertUserCommand.Parameters.AddWithValue("@hash", adminHash);
            await insertUserCommand.ExecuteNonQueryAsync();

            // Insert sample tasks
            var insertTaskCommand = connection.CreateCommand();
            insertTaskCommand.CommandText = @"
                INSERT INTO Tasks (Title, Description, Status, DueDate, UserId) VALUES
                ('Complete project', 'Finish the task management app', 0, '2026-05-01', 1),
                ('Review code', 'Code review for the backend', 1, '2026-04-20', 1);
            ";
            await insertTaskCommand.ExecuteNonQueryAsync();
        }
    }
}