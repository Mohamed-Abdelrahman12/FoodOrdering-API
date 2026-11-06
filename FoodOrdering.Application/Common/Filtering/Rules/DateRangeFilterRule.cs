using System.Linq.Expressions;

namespace FoodOrdering.Application.Common.Filtering.Rules
{
    public class DateRangeFilterRule<TEntity> : FilterRule<TEntity>
    {
        private readonly Expression<Func<TEntity, DateTime>> _dateSelector;
        private readonly DateOnly? _startDate;
        private readonly DateOnly? _endDate;

        public DateRangeFilterRule(
            Expression<Func<TEntity, DateTime>> dateSelector,
            DateOnly? startDate,
            DateOnly? endDate)
        {
            _dateSelector = dateSelector;
            _startDate = startDate;
            _endDate = endDate;
        }

        public override bool IsApplicable() => _startDate.HasValue || _endDate.HasValue;

        public override IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            var param = _dateSelector.Parameters[0];

            if (_startDate.HasValue)
            {
                var start = _startDate.Value.ToDateTime(TimeOnly.MinValue);
                var body = Expression.GreaterThanOrEqual(
                    _dateSelector.Body,
                    Expression.Constant(start, typeof(DateTime)));
                query = query.Where(Expression.Lambda<Func<TEntity, bool>>(body, param));
            }

            if (_endDate.HasValue)
            {
                var end = _endDate.Value.ToDateTime(TimeOnly.MinValue).AddDays(1);
                var body = Expression.LessThan(
                    _dateSelector.Body,
                    Expression.Constant(end, typeof(DateTime)));
                query = query.Where(Expression.Lambda<Func<TEntity, bool>>(body, param));
            }

            return query;
        }
    }
}