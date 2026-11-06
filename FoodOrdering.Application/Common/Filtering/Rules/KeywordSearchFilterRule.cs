using System.Linq.Expressions;

namespace FoodOrdering.Application.Common.Filtering.Rules
{
    public class KeywordSearchFilterRule<TEntity> : FilterRule<TEntity>
    {
        private readonly string? _searchTerm;
        private readonly Func<string, Expression<Func<TEntity, bool>>> _predicateFactory;
        public KeywordSearchFilterRule(
            string? searchTerm,
            Func<string, Expression<Func<TEntity, bool>>> predicateFactory)
        {
            _searchTerm = searchTerm;
            _predicateFactory = predicateFactory;
        }

        public override bool IsApplicable() => !string.IsNullOrWhiteSpace(_searchTerm);

        public override IQueryable<TEntity> Apply(IQueryable<TEntity> query)
        {
            var words = _searchTerm!
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.ToLower());

            foreach (var word in words)
                query = query.Where(_predicateFactory(word));

            return query;
        }
    }
}