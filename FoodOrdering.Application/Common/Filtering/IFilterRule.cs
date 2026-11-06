

namespace FoodOrdering.Application.Common.Filtering
{
    public interface IFilterRule<TEntity>
    {
        IQueryable<TEntity> Apply(IQueryable<TEntity> query);
        bool IsApplicable();
    }
}
