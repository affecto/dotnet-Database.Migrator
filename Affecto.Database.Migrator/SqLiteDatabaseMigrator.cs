using System.Reflection;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Processors.SQLite;

namespace Affecto.Database.Migrator
{
    public class SqLiteDatabaseMigrator : DatabaseMigrator
    {
        public SqLiteDatabaseMigrator(string connectionStringKey, Assembly assembly, MigrationOptions options) : base(connectionStringKey, assembly, options)
        {
        }

        protected override IMigrationProcessor GetMigrationProcessor(string connectionString, IAnnouncer announcer)
        {
            var sqliteFactory = new SQLiteProcessorFactory();
            
            return sqliteFactory.Create(connectionString, announcer, base.options);
        }
    }
}
