using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Ardalis.Specification.EntityFrameworkCore
{
    public class SpecificationEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, BaseSpecification<T> specification)
        {
            var query = inputQuery;

            // modify the IQueryable using the specification's criteria expression
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Includes all expression-based includes
            query = specification.Includes.Aggregate(query,
                                    (current, include) => current.Include(include));

            // Include any string-based include statements
            query = specification.IncludeStrings.Aggregate(query,
                                    (current, include) => current.Include(include));

            // Apply ordering if expressions are set
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            // Apply paging if enabled
            if (specification.isPagingEnabled)
            {
                query = query.Skip(specification.Skip)
                             .Take(specification.Take);
            }
            return query;
        }
    }
}

namespace Ardalis.Specification
{
    public class BaseSpecification<T>
    {
        protected Expression<Func<T, bool>> _criteria;
        public Expression<Func<T, bool>> Criteria => _criteria;

        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public void AddInclude(Expression<Func<T, object>> includeExpression) => Includes.Add(includeExpression);
        public List<string> IncludeStrings { get; } = new List<string>();
        protected virtual void AddInclude(string includeString) => IncludeStrings.Add(includeString);

        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool isPagingEnabled { get; private set; } = false;


        public string CacheKey { get; protected set; }
        public bool CacheEnabled { get; private set; }

        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            _criteria = criteria;
        }

        protected virtual void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            isPagingEnabled = true;
        }

        protected virtual void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected virtual void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        /// <summary>
        /// Must be called after specifying criteria
        /// </summary>
        /// <param name="specificationName"></param>
        /// <param name="args">Any arguments used in defining the specification</param>
        protected void EnableCache(string specificationName, params object[] args)
        {
            Guard.Against.NullOrEmpty(specificationName, nameof(specificationName));
            Guard.Against.Null(Criteria, nameof(Criteria));

            CacheKey = $"{specificationName}-{string.Join("-", args)}";

            CacheEnabled = true;
        }
    }
}
