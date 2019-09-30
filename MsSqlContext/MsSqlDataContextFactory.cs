using System;
using Infrastructure.DataModel;
using Infrastructure.Options;
using Microsoft.EntityFrameworkCore.Design;

namespace MsSqlContext
{
    public class MsSqlDataContextFactory : DataContextFactory, IDesignTimeDbContextFactory<MsSqlDataContext>
    {
        private MsSqlDataContext CreateContext(DatabaseOptions options)
        {
            if (options.Provider != DatabaseProviders.MsSql)
                throw new InvalidOperationException("This Project can only handly MsSql Connections");

            return new MsSqlDataContext(options.BuildOptions());
        }

        #region Implementation of IDesignTimeDbContextFactory<out MsSqlDataContext>

        public MsSqlDataContext CreateDbContext(string[] args)
        {
            return CreateContext(GetConfiguration("MsSql"));
        }

        #endregion
    }
}