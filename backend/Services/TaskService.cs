using TodoApi.DTOs;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Services
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _repository;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ITaskRepository repository, ILogger<TaskService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync()
        {
            try
            {
                var tasks = await _repository.GetAllAsync();
                return tasks.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all tasks");
                throw;
            }
        }

        public async Task<IEnumerable<TaskResponseDto>> GetAllTasksByUserAsync(string userId)
        {
            try
            {
                var tasks = await _repository.GetAllByUserIdAsync(userId);
                return tasks.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks for user {UserId}", userId);
                throw;
            }
        }

        public async Task<TaskResponseDto?> GetTaskByIdAsync(string id)
        {
            try
            {
                var task = await _repository.GetByIdAsync(id);
                return task == null ? null : MapToDto(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task {TaskId}", id);
                throw;
            }
        }

        public async Task<TaskResponseDto> CreateTaskAsync(string userId, CreateTaskDto dto)
        {
            try
            {
                var task = new TodoTask
                {
                    UserId = userId,
                    Title = dto.Title,
                    Description = dto.Description,
                    Priority = dto.Priority,
                    Category = dto.Category,
                    DueDate = dto.DueDate,
                    Status = Models.TaskStatus.Pending
                };

                var created = await _repository.CreateAsync(task);
                return MapToDto(created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task for user {UserId}", userId);
                throw;
            }
        }

        public async Task<TaskResponseDto?> UpdateTaskAsync(string id, UpdateTaskDto dto)
        {
            try
            {
                var task = new TodoTask
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Status = dto.Status,
                    Priority = dto.Priority,
                    Category = dto.Category,
                    DueDate = dto.DueDate
                };

                var updated = await _repository.UpdateAsync(id, task);
                return updated == null ? null : MapToDto(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task {TaskId}", id);
                throw;
            }
        }

        public async Task<bool> DeleteTaskAsync(string id)
        {
            try
            {
                return await _repository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task {TaskId}", id);
                throw;
            }
        }

        private static TaskResponseDto MapToDto(TodoTask task)
        {
            return new TaskResponseDto
            {
                Id = task.Id,
                UserId = task.UserId,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status.ToString(),
                Priority = task.Priority.ToString(),
                Category = task.Category,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }
}
