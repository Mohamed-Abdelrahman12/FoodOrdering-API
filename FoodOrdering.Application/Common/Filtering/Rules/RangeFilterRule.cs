using System.Linq.Expressions;

namespace FoodOrdering.Application.Common.Filtering.Rules
{
    public class RangeFilterRule<TEntity, TValue> : FilterRule<TEntity>
        where TValue : struct, IComparable<TValue>
    {
        private readonly Expression<Func<TEntity, TValue>> _selector;
        private readonly TValue? _min;
        private readonly TValue? _max;

        public RangeFilterRule(
            Expression<Func<TEntity, TValue>> selector,
            TValue? min,
            TValue? max)
        {
            _selector = selector;
            _min = min;
            _max = max;
        }

        public override bool IsApplicable() => _min.HasValue || _max.HasValue;

        public override IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            var param = _selector.Parameters[0];

            if (_min.HasValue)
            {
                var body = Expression.GreaterThanOrEqual(
                    _selector.Body,
                    Expression.Constant(_min.Value, typeof(TValue)));
                query = query.Where(Expression.Lambda<Func<TEntity, bool>>(body, param));
            }

            if (_max.HasValue)
            {
                var body = Expression.LessThanOrEqual(
                    _selector.Body,
                    Expression.Constant(_max.Value, typeof(TValue)));
                query = query.Where(Expression.Lambda<Func<TEntity, bool>>(body, param));
            }

            return query;
        }
    }
}