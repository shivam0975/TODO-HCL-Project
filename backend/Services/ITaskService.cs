using TodoApi.DTOs;

namespace TodoApi.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskResponseDto>> GetAllTasksAsync();
        Task<IEnumerable<TaskResponseDto>> GetAllTasksByUserAsync(string userId);
        Task<TaskResponseDto?> GetTaskByIdAsync(string id);
        Task<TaskResponseDto> CreateTaskAsync(string userId, CreateTaskDto dto);
        Task<TaskResponseDto?> UpdateTaskAsync(string id, UpdateTaskDto dto);
        Task<bool> DeleteTaskAsync(string id);
    }
}
