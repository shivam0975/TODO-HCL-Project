using MongoDB.Driver;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(string id);
        Task<User> CreateUserAsync(User user);
        Task<bool> UserExistsAsync(string email);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IMongoDbContext _context;

        public UserRepository(IMongoDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            return await _context.Users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            return await _context.Users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            await _context.Users.InsertOneAsync(user);
            // Retrieve the user from database to get the generated Id
            return await GetUserByEmailAsync(user.Email) ?? user;
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email);
            var count = await _context.Users.CountDocumentsAsync(filter);
            return count > 0;
        }
    }
}
