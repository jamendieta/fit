using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Services;
using TaskEntity = TaskManagement.Core.Entities.Task;

namespace TaskManagement.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly TaskService _taskService;

    public TasksController(TaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var tasks = await _taskService.GetTasksByUserIdAsync(userId.Value);
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var task = await _taskService.GetTaskByIdAsync(id);
        if (task == null)
            return NotFound();
        if (task.UserId != userId.Value)
            return Forbid();

        return Ok(task);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var currentUserId = GetCurrentUserId();
        if (currentUserId == null)
            return Unauthorized();
        if (currentUserId.Value != userId)
            return Forbid();

        var tasks = await _taskService.GetTasksByUserIdAsync(userId);
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskEntity task)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        task.UserId = userId.Value;

        try
        {
            await _taskService.AddTaskAsync(task);
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskEntity task)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        if (id != task.Id)
            return BadRequest();

        var existing = await _taskService.GetTaskByIdAsync(id);
        if (existing == null)
            return NotFound();
        if (existing.UserId != userId.Value)
            return Forbid();

        task.UserId = userId.Value;

        try
        {
            await _taskService.UpdateTaskAsync(task);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
            return Unauthorized();

        var existing = await _taskService.GetTaskByIdAsync(id);
        if (existing == null)
            return NotFound();
        if (existing.UserId != userId.Value)
            return Forbid();

        try
        {
            await _taskService.DeleteTaskAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    private int? GetCurrentUserId()
    {
        var value = User.FindFirstValue("userId");
        return int.TryParse(value, out var id) ? id : null;
    }
}