using System;
using Infrastructure.DataModel;
using Infrastructure.Options;
using Microsoft.EntityFrameworkCore.Design;

namespace MySqlContext
{
    public class MySqlDataContextFactory : DataContextFactory, IDesignTimeDbContextFactory<MySqlDataContext>
    {
        private MySqlDataContext CreateContext(DatabaseOptions options)
        {
            if (options.Provider != DatabaseProviders.MySql)
                throw new InvalidOperationException("This project is only for MySql use MsSqlContext Project for an MsSql Connection");

            return new MySqlDataContext(options.BuildOptions());
        }

        #region Implementation of IDesignTimeDbContextFactory<out MySqlDataContext>

        public MySqlDataContext CreateDbContext(string[] args)
        {
            return CreateContext(GetConfiguration("MySql"));
        }

        #endregion
    }
}