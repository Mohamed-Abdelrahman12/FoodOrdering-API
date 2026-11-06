using FoodOrdering.Domain.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace FoodOrdering.Infrastructure.Repositories
{
    public interface IRedisCartRepository
    {
        Task<Cart?> GetCartAsync(string userId);
        Task<Cart> UpdateCartAsync(Cart cart);
        Task<bool> DeleteCartAsync(string userId);
    }

    public class RedisCartRepository : IRedisCartRepository
    {
        private readonly IDatabase _database;
        private static readonly TimeSpan CartExpiry = TimeSpan.FromDays(3);

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
        };

        public RedisCartRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<Cart?> GetCartAsync(string userId)
        {
            var redisKey = BuildKey(userId);
            var data = await _database.StringGetAsync(redisKey);

            if (data.IsNullOrEmpty)
                return null;

            return JsonSerializer.Deserialize<Cart>(data!, SerializerOptions);
        }

        public async Task<Cart> UpdateCartAsync(Cart cart)
        {
            var redisKey = BuildKey(cart.UserId);
            var serialized = JsonSerializer.Serialize(cart, SerializerOptions);

            var saved = await _database.StringSetAsync(redisKey, serialized, CartExpiry);
            if (!saved)
                throw new InvalidOperationException($"Failed to save cart for user '{cart.UserId}' in Redis.");

            return (await GetCartAsync(cart.UserId))!;
        }

        public Task<bool> DeleteCartAsync(string userId)
            => _database.KeyDeleteAsync(BuildKey(userId));

        private static string BuildKey(string userId) => $"cart:{userId}";
    }
}
