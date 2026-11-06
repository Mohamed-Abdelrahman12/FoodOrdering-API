using FoodOrdering.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Application.Common.Filtering
{
    public static class PaginationHelper
    {
        public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
            IQueryable<TEntity> query,
            int page,
            int pageSize,
            Func<IEnumerable<TEntity>, IEnumerable<TDto>> mapper,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            CancellationToken cancellationToken = default)
        {
            var totalCount = await query.CountAsync(cancellationToken);

            var items = await orderBy(query)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<TDto>
            {
                Items = mapper(items),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
