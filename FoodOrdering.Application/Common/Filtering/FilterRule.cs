

namespace FoodOrdering.Application.Common.Filtering
{
    public abstract class FilterRule<TEntity> : IFilterRule<TEntity>
    {
        public abstract IQueryable<TEntity> Apply(IQueryable<TEntity> query);
        public abstract bool IsApplicable();
    }
}
