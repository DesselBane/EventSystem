using Autofac;

namespace Common.Test
{
    public abstract class TestBase
    {
        #region Vars

        private readonly IContainer _container;

        #endregion

        #region Properties

        protected ILifetimeScope CurrentLifetime { get; private set; }

        #endregion

        #region Constructors

        protected TestBase()
        {
            var builder = new ContainerBuilder();

            // ReSharper disable once VirtualMemberCallInConstructor
            BuildUp(builder);

            _container = builder.Build();
        }

        #endregion

        protected void StartLifetime()
        {
            CurrentLifetime?.Dispose();
            CurrentLifetime = _container.BeginLifetimeScope();
        }

        protected abstract void BuildUp(ContainerBuilder builder);
    }
}