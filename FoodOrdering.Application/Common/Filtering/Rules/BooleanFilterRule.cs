using System.Linq.Expressions;

namespace FoodOrdering.Application.Common.Filtering.Rules
{
    public class BooleanFilterRule<TEntity> : FilterRule<TEntity>
    {
        private readonly Expression<Func<TEntity, bool>> _selector;
        private readonly bool? _value;

        public BooleanFilterRule(Expression<Func<TEntity, bool>> selector, bool? value)
        {
            _selector = selector;
            _value = value;
        }

        public override bool IsApplicable() => _value.HasValue;

        public override IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            bool expected = _value!.Value;

            if (expected)
                return query.Where(_selector);

            // e => !selector(e)
            var param = _selector.Parameters[0];
            var notBody = Expression.Not(_selector.Body);
            var predicate = Expression.Lambda<Func<TEntity, bool>>(notBody, param);
            return query.Where(predicate);
        }
    }
}