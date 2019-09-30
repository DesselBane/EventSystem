using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Common.Test
{
    internal class TestAsyncEnumerable <T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        #region Properties

        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);

        #endregion

        #region Constructors

        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        {
        }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        {
        }

        #endregion

        public IAsyncEnumerator<T> GetEnumerator()
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }
    }
}