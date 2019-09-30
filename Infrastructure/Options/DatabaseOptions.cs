using System;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Options
{
    public sealed class DatabaseOptions
    {
        #region Properties

        public string Hostname { get; set; } = "localhost";
        public short? Port { get; set; }
        public DatabaseProviders Provider { get; set; } = DatabaseProviders.MySql;
        public string Databasename { get; set; } = "db_EventSystem";
        public bool UseWindowsAuthentication { get; set; } = false;
        public string SqlUsername { get; set; }
        public string SqlPassword { get; set; }

        #endregion

        public DbContextOptions BuildOptions()
        {
            var builder = new DbContextOptionsBuilder();
            BuildOptions(builder);
            return builder.Options;
        }

        public void BuildOptions(DbContextOptionsBuilder builder)
        {
            ThrowIfMissconfigured();

            switch (Provider)
            {
                case DatabaseProviders.MySql:
                    builder.UseMySql($"server={Hostname};userid={SqlUsername};pwd={SqlPassword};port={Port};database={Databasename};sslmode=none");
                    break;
                case DatabaseProviders.MsSql:
                    var hostname = Hostname + (Port != null ? ":" + Port : "");
                    builder.UseSqlServer($"Server={hostname};Database={Databasename};Integrated Security={UseWindowsAuthentication};User ID={SqlUsername};Password={SqlPassword};MultipleActiveResultSets=true");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ThrowIfMissconfigured()
        {
            if (!UseWindowsAuthentication && (string.IsNullOrWhiteSpace(SqlPassword) ||
                                              string.IsNullOrWhiteSpace(SqlUsername)))
                throw new InvalidOperationException(
                    "No Authentication Methode defined. Use WindowsAuthentication or SqlUsername and SqlPassword");
            if (Provider == DatabaseProviders.MySql && (string.IsNullOrWhiteSpace(SqlPassword) ||
                                                        string.IsNullOrWhiteSpace(SqlUsername)))
                throw new InvalidOperationException("A SqlUsername and SqlPassword have to be defined");
        }

        public override string ToString()
        {
            switch (Provider)
            {
                case DatabaseProviders.MySql:
                    return
                        $"server={Hostname};userid={SqlUsername};pwd={SqlPassword};port={Port};database={Databasename};sslmode=none";
                case DatabaseProviders.MsSql:
                    var hostname = Hostname + (Port != null ? ":" + Port : "");
                    return
                        $"Server={hostname};Database={Databasename};Integrated Security={UseWindowsAuthentication};User ID={SqlUsername};Password={SqlPassword};MultipleActiveResultSets=true";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}