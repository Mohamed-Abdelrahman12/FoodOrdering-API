using System.Linq.Expressions;

namespace FoodOrdering.Application.Common.Filtering.Rules
{
    public class EqualityFilterRule<TEntity, TValue> : FilterRule<TEntity>
        where TValue : struct
    {
        private readonly Expression<Func<TEntity, TValue>> _selector;
        private readonly TValue? _value;

        public EqualityFilterRule(Expression<Func<TEntity, TValue>> selector, TValue? value)
        {
            _selector = selector;
            _value = value;
        }

        public override bool IsApplicable() => _value.HasValue;

        public override IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            var expected = _value!.Value;

            var param = _selector.Parameters[0];
            var body = Expression.Equal(
                _selector.Body,
                Expression.Constant(expected, typeof(TValue)));
            var predicate = Expression.Lambda<Func<TEntity, bool>>(body, param);

            return query.Where(predicate);
        }
    }
}