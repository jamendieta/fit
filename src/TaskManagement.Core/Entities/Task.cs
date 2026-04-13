namespace TaskManagement.Core.Entities;

public class Task
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TaskStatus Status { get; set; } = TaskStatus.Pending;
    public DateTime DueDate { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
}

public enum TaskStatus
{
    Pending,
    InProgress,
    Completed
}