using FoodOrdering.Domain.Entities;
using FoodOrdering.Domain.Interfaces;
using FoodOrdering.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace FoodOrdering.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public IRepository<Category> Categories { get; }
        public IRepository<Product> Products { get; }
        public IRepository<Order> Orders { get; }
        public IRepository<OrderItem> OrderItems { get; }
        public IRepository<Cart> Carts { get; }
        public IRepository<CartItem> CartItems { get; }
        public IRepository<Payment> Payments { get; }
        public IRepository<RefreshToken> RefreshTokens { get; }
        public IRepository<Review> Reviews { get; }
        public IRepository<Contact> Contacts { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Categories = new Repository<Category>(context);
            Products = new Repository<Product>(context);
            Orders = new Repository<Order>(context);
            OrderItems = new Repository<OrderItem>(context);
            Carts = new Repository<Cart>(context);
            CartItems = new Repository<CartItem>(context);
            Payments = new Repository<Payment>(context);
            RefreshTokens = new Repository<RefreshToken>(context);
            Reviews = new Repository<Review>(context);
            Contacts = new Repository<Contact>(context);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync(cancellationToken);

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
            => _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
                await _transaction.CommitAsync(cancellationToken);
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
                await _transaction.RollbackAsync(cancellationToken);
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
