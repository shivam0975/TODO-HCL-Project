using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface ITaskRepository
    {
        Task<IEnumerable<TodoTask>> GetAllAsync();
        Task<IEnumerable<TodoTask>> GetAllByUserIdAsync(string userId);
        Task<TodoTask?> GetByIdAsync(string id);
        Task<TodoTask> CreateAsync(TodoTask task);
        Task<TodoTask?> UpdateAsync(string id, TodoTask task);
        Task<bool> DeleteAsync(string id);
    }
}
