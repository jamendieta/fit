using TaskManagement.Application.Services;
using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;

namespace TaskManagement.Application.Tests;

public class UnitTest1
{
    [Fact]
    public async System.Threading.Tasks.Task AddTaskAsync_WithEmptyTitle_ThrowsArgumentException()
    {
        var service = new TaskService(new InMemoryTaskRepository());
        var task = new TaskManagement.Core.Entities.Task
        {
            Title = "",
            Description = "demo",
            DueDate = DateTime.UtcNow.AddDays(1),
            UserId = 1
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.AddTaskAsync(task));
    }

    [Fact]
    public async System.Threading.Tasks.Task RegisterAndValidateUserAsync_ReturnsUser_WhenPasswordIsCorrect()
    {
        var repo = new InMemoryUserRepository();
        var service = new UserService(repo);
        var user = new User
        {
            Username = "john",
            Email = "john@example.com",
            PasswordHash = "secret123"
        };

        await service.RegisterUserAsync(user);
        var validated = await service.ValidateUserAsync("john", "secret123");

        Assert.NotNull(validated);
        Assert.Equal("john", validated!.Username);
        Assert.NotEqual("secret123", validated.PasswordHash);
    }

    private sealed class InMemoryTaskRepository : ITaskRepository
    {
        private readonly List<TaskManagement.Core.Entities.Task> _tasks = [];
        private int _nextId = 1;

        public System.Threading.Tasks.Task<IEnumerable<TaskManagement.Core.Entities.Task>> GetAllAsync() =>
            System.Threading.Tasks.Task.FromResult<IEnumerable<TaskManagement.Core.Entities.Task>>(_tasks);

        public System.Threading.Tasks.Task<TaskManagement.Core.Entities.Task?> GetByIdAsync(int id) =>
            System.Threading.Tasks.Task.FromResult(_tasks.FirstOrDefault(x => x.Id == id));

        public System.Threading.Tasks.Task<IEnumerable<TaskManagement.Core.Entities.Task>> GetByUserIdAsync(int userId) =>
            System.Threading.Tasks.Task.FromResult<IEnumerable<TaskManagement.Core.Entities.Task>>(_tasks.Where(x => x.UserId == userId));

        public System.Threading.Tasks.Task AddAsync(TaskManagement.Core.Entities.Task task)
        {
            task.Id = _nextId++;
            _tasks.Add(task);
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public System.Threading.Tasks.Task UpdateAsync(TaskManagement.Core.Entities.Task task)
        {
            var idx = _tasks.FindIndex(x => x.Id == task.Id);
            if (idx >= 0) _tasks[idx] = task;
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public System.Threading.Tasks.Task DeleteAsync(int id)
        {
            _tasks.RemoveAll(x => x.Id == id);
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }

    private sealed class InMemoryUserRepository : IUserRepository
    {
        private readonly List<User> _users = [];
        private int _nextId = 1;

        public System.Threading.Tasks.Task<User?> GetByIdAsync(int id) =>
            System.Threading.Tasks.Task.FromResult(_users.FirstOrDefault(x => x.Id == id));

        public System.Threading.Tasks.Task<User?> GetByUsernameAsync(string username) =>
            System.Threading.Tasks.Task.FromResult(_users.FirstOrDefault(x => x.Username == username));

        public System.Threading.Tasks.Task AddAsync(User user)
        {
            user.Id = _nextId++;
            _users.Add(user);
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public System.Threading.Tasks.Task UpdateAsync(User user)
        {
            var idx = _users.FindIndex(x => x.Id == user.Id);
            if (idx >= 0) _users[idx] = user;
            return System.Threading.Tasks.Task.CompletedTask;
        }

        public System.Threading.Tasks.Task DeleteAsync(int id)
        {
            _users.RemoveAll(x => x.Id == id);
            return System.Threading.Tasks.Task.CompletedTask;
        }
    }
}
