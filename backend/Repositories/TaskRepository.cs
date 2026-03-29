using MongoDB.Bson;
using MongoDB.Driver;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IMongoDbContext _context;

        public TaskRepository(IMongoDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TodoTask>> GetAllAsync()
        {
            var tasks = await _context.Tasks
                .Find(Builders<TodoTask>.Filter.Empty)
                .SortByDescending(t => t.CreatedAt)
                .ToListAsync();
            return tasks;
        }

        public async Task<IEnumerable<TodoTask>> GetAllByUserIdAsync(string userId)
        {
            var filter = Builders<TodoTask>.Filter.Eq(t => t.UserId, userId);
            var tasks = await _context.Tasks
                .Find(filter)
                .SortByDescending(t => t.CreatedAt)
                .ToListAsync();
            return tasks;
        }

        public async Task<TodoTask?> GetByIdAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return null;

            var filter = Builders<TodoTask>.Filter.Eq("_id", objectId);
            return await _context.Tasks.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<TodoTask> CreateAsync(TodoTask task)
        {
            task.CreatedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.Tasks.InsertOneAsync(task);
            return task;
        }

        public async Task<TodoTask?> UpdateAsync(string id, TodoTask updatedTask)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return null;

            var filter = Builders<TodoTask>.Filter.Eq("_id", objectId);
            var update = Builders<TodoTask>.Update
                .Set(t => t.Title, updatedTask.Title)
                .Set(t => t.Description, updatedTask.Description)
                .Set(t => t.Status, updatedTask.Status)
                .Set(t => t.Priority, updatedTask.Priority)
                .Set(t => t.Category, updatedTask.Category)
                .Set(t => t.DueDate, updatedTask.DueDate)
                .Set(t => t.UpdatedAt, DateTime.UtcNow);

            var options = new FindOneAndUpdateOptions<TodoTask> { ReturnDocument = ReturnDocument.After };
            return await _context.Tasks.FindOneAndUpdateAsync(filter, update, options);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return false;

            var filter = Builders<TodoTask>.Filter.Eq("_id", objectId);
            var result = await _context.Tasks.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
    }
}
