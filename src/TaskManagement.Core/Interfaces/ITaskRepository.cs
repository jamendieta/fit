using TaskEntity = TaskManagement.Core.Entities.Task;

namespace TaskManagement.Core.Interfaces;

public interface ITaskRepository
{
    Task<IEnumerable<TaskEntity>> GetAllAsync();
    Task<TaskEntity?> GetByIdAsync(int id);
    Task<IEnumerable<TaskEntity>> GetByUserIdAsync(int userId);
    Task AddAsync(TaskEntity task);
    Task UpdateAsync(TaskEntity task);
    Task DeleteAsync(int id);
}