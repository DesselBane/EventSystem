using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Common.Test
{
    public static class MockExtensions
    {
        public static void SetUpIQueryable <TClass>(this Mock<DbSet<TClass>> setMock, params TClass[] data)
            where TClass : class
        {
            SetUpIQueryable(setMock, data.AsQueryable());
        }

        public static void SetUpIQueryable <TClass>(this Mock<DbSet<TClass>> setMock, IQueryable<TClass> data)
            where TClass : class
        {
            setMock.As<IAsyncEnumerable<TClass>>()
                   .Setup(m => m.GetEnumerator())
                   .Returns(new TestAsyncEnumerator<TClass>(data.GetEnumerator()));

            setMock.As<IQueryable<TClass>>()
                   .Setup(m => m.Provider)
                   .Returns(new TestAsyncQueryProvider<TClass>(data.Provider));

            setMock.As<IQueryable<TClass>>()
                   .Setup(x => x.Expression)
                   .Returns(data.Expression);

            setMock.As<IQueryable<TClass>>()
                   .Setup(x => x.ElementType)
                   .Returns(data.ElementType);

            setMock.As<IQueryable<TClass>>()
                   .Setup(x => x.GetEnumerator())
                   .Returns(data.GetEnumerator);
        }
    }
}