using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Test
{
    internal class TestAsyncEnumerator <T> : IAsyncEnumerator<T>
    {
        #region Vars

        private readonly IEnumerator<T> _inner;

        #endregion

        #region Properties

        public T Current => _inner.Current;

        #endregion

        #region Constructors

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        #endregion

        public void Dispose()
        {
            _inner.Dispose();
        }

        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext());
        }
    }
}