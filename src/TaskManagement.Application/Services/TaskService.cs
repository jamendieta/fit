using TaskManagement.Core.Entities;
using TaskManagement.Core.Interfaces;
using TaskEntity = TaskManagement.Core.Entities.Task;

namespace TaskManagement.Application.Services;

public class TaskService
{
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async System.Threading.Tasks.Task<IEnumerable<TaskEntity>> GetAllTasksAsync()
    {
        return await _taskRepository.GetAllAsync();
    }

    public async System.Threading.Tasks.Task<TaskEntity?> GetTaskByIdAsync(int id)
    {
        return await _taskRepository.GetByIdAsync(id);
    }

    public async System.Threading.Tasks.Task<IEnumerable<TaskEntity>> GetTasksByUserIdAsync(int userId)
    {
        return await _taskRepository.GetByUserIdAsync(userId);
    }

    public async System.Threading.Tasks.Task AddTaskAsync(TaskEntity task)
    {
        if (string.IsNullOrWhiteSpace(task.Title))
            throw new ArgumentException("Title is required");

        if (task.DueDate < DateTime.Now)
            throw new ArgumentException("Due date cannot be in the past");

        await _taskRepository.AddAsync(task);
    }

    public async System.Threading.Tasks.Task UpdateTaskAsync(TaskEntity task)
    {
        var existing = await _taskRepository.GetByIdAsync(task.Id);
        if (existing == null)
            throw new KeyNotFoundException("Task not found");

        if (string.IsNullOrWhiteSpace(task.Title))
            throw new ArgumentException("Title is required");

        await _taskRepository.UpdateAsync(task);
    }

    public async System.Threading.Tasks.Task DeleteTaskAsync(int id)
    {
        var existing = await _taskRepository.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException("Task not found");

        await _taskRepository.DeleteAsync(id);
    }
}