using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoApi.DTOs;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        private string? GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        /// <summary>
        /// Get all tasks for the current user
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            try
            {
                var tasks = await _taskService.GetAllTasksByUserAsync(userId);
                return Ok(tasks ?? new List<TaskResponseDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks for user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while retrieving tasks" });
            }
        }

        /// <summary>
        /// Get task by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { message = "Task ID is required" });

            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                    return NotFound(new { message = $"Task with ID {id} not found" });

                if (task.UserId != userId)
                    return Unauthorized(new { message = "You are not allowed to access this task" });

                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the task" });
            }
        }

        /// <summary>
        /// Create a new task
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var created = await _taskService.CreateTaskAsync(userId, dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task for user {UserId}", userId);
                return StatusCode(500, new { message = "An error occurred while creating the task" });
            }
        }

        /// <summary>
        /// Update an existing task
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateTaskDto dto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { message = "Task ID is required" });

            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                    return NotFound(new { message = $"Task with ID {id} not found" });

                if (task.UserId != userId)
                    return Unauthorized(new { message = "You are not allowed to update this task" });

                var updated = await _taskService.UpdateTaskAsync(id, dto);
                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating the task" });
            }
        }

        /// <summary>
        /// Delete a task
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User ID not found in token" });

            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { message = "Task ID is required" });

            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                if (task == null)
                    return NotFound(new { message = $"Task with ID {id} not found" });

                if (task.UserId != userId)
                    return Unauthorized(new { message = "You are not allowed to delete this task" });

                var deleted = await _taskService.DeleteTaskAsync(id);
                if (!deleted)
                    return StatusCode(500, new { message = $"Failed to delete task {id}" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the task" });
            }
        }
    }
}