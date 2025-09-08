using System;
using System.Linq.Expressions;
using Domain.Models;
using FilterBooks;

namespace Application.Filters
{
    public static class BookFilterExtensions
    {
        public static BookFilter ToBookFilter(this FilterBooksQueryInput? input)
        {
            if (input is null) return new BookFilter();

            return new BookFilter
            {
                Genre = string.IsNullOrWhiteSpace(input.Genre) ? null : input.Genre
            };
        }

        public static Expression<Func<Book, bool>>? ToExpression(this BookFilter? filter)
        {
            if (filter is null) return null;

            Expression<Func<Book, bool>> expr = b => true;

            if (!string.IsNullOrWhiteSpace(filter.Title))
            {
                Expression<Func<Book, bool>> t = b => b.Title != null && b.Title.ToLower().Contains(filter.Title.ToLower());
                expr = expr.AndAlso(t);
            }

            if (!string.IsNullOrWhiteSpace(filter.AuthorId))
            {
                Expression<Func<Book, bool>> a = b => b.AuthorId == filter.AuthorId;
                expr = expr.AndAlso(a);
            }

            if (!string.IsNullOrWhiteSpace(filter.Isbn))
            {
                Expression<Func<Book, bool>> i = b => b.ISBN != null && b.ISBN == filter.Isbn;
                expr = expr.AndAlso(i);
            }

            if (filter.PublishedYear.HasValue)
            {
                Expression<Func<Book, bool>> py = b => b.PublishedYear == filter.PublishedYear.Value;
                expr = expr.AndAlso(py);
            }

            if (filter.Available.HasValue)
            {
                if (filter.Available.Value)
                {
                    Expression<Func<Book, bool>> av = b => b.CopiesAvailable > 0;
                    expr = expr.AndAlso(av);
                }
                else
                {
                    Expression<Func<Book, bool>> av = b => b.CopiesAvailable <= 0;
                    expr = expr.AndAlso(av);
                }
            }

            if (!string.IsNullOrWhiteSpace(filter.Genre))
            {
                Expression<Func<Book, bool>> g = b => b.Genre != null && b.Genre.ToLower().Contains(filter.Genre.ToLower());
                expr = expr.AndAlso(g);
            }

            return expr;
        }

        private static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null) return right;
            if (right == null) return left;

            var parameter = Expression.Parameter(typeof(T));

            var leftBody = new ParameterReplacer(parameter).Visit(left.Body);
            var rightBody = new ParameterReplacer(parameter).Visit(right.Body);

            var body = Expression.AndAlso(leftBody!, rightBody!);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _parameter;

            public ParameterReplacer(ParameterExpression parameter)
            {
                _parameter = parameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return _parameter;
            }
        }
    }
}
