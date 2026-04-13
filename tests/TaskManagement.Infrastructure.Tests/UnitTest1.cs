using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Repositories;
using TaskStatusEntity = TaskManagement.Core.Entities.TaskStatus;

namespace TaskManagement.Infrastructure.Tests;

public class UnitTest1
{
    [Fact]
    public async Task UserRepository_AddAndGetByUsername_Works()
    {
        var connectionString = $"Data Source={Path.GetTempFileName()}";
        var initializer = new DatabaseInitializer(connectionString);
        await initializer.InitializeAsync();

        var repo = new UserRepository(connectionString);
        await repo.AddAsync(new Core.Entities.User
        {
            Username = "maria",
            Email = "maria@example.com",
            PasswordHash = "hash"
        });

        var user = await repo.GetByUsernameAsync("maria");

        Assert.NotNull(user);
        Assert.Equal("maria@example.com", user!.Email);
    }

    [Fact]
    public async Task TaskRepository_AddAndReadByUser_Works()
    {
        var connectionString = $"Data Source={Path.GetTempFileName()}";
        var initializer = new DatabaseInitializer(connectionString);
        await initializer.InitializeAsync();

        var taskRepo = new TaskRepository(connectionString);

        await taskRepo.AddAsync(new Core.Entities.Task
        {
            Title = "Prepare demo",
            Description = "Get presentation ready",
            DueDate = DateTime.UtcNow.AddDays(2),
            Status = TaskStatusEntity.Pending,
            UserId = 1
        });

        var tasks = (await taskRepo.GetByUserIdAsync(1)).ToList();

        Assert.NotEmpty(tasks);
        Assert.Contains(tasks, x => x.Title == "Prepare demo");
    }
}
