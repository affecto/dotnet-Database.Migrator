using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Affecto.Database.Migrator.Interfaces;
using FluentMigrator;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors.SqlServer;

namespace Affecto.Database.Migrator
{
    public class DatabaseMigrator : IDatabaseMigrator
    {
        private readonly string connectionStringKey;
        private readonly Assembly assembly;
        protected readonly MigrationOptions options;
        private readonly List<string> migrationTags;
        private IMigrationRunner runner;

        public DatabaseMigrator(string connectionStringKey, Assembly assembly, MigrationOptions options, List<string> migrationTags = null)
        {
            this.connectionStringKey = connectionStringKey;
            this.assembly = assembly;
            this.options = options;
            this.migrationTags = migrationTags;
        }

        public void MigrateUp()
        {
            ConfigureRunner();
            
            runner.MigrateUp();
        }

        public void MigrateUp(long targetVersion)
        {
            ConfigureRunner();

            runner.MigrateUp(targetVersion);
        }

        public void MigrateDown(long targetVersion)
        {
           ConfigureRunner();

           runner.MigrateDown(targetVersion);
        }

        protected virtual IAnnouncer GetAnnouncer()
        {
            var announcer = new ConsoleAnnouncer() { ShowElapsedTime = true, ShowSql = true };

            return announcer;
        }

        private void ConfigureRunner()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringKey].ConnectionString;
            
            var announcer = GetAnnouncer();
            
            var migrationContext = new RunnerContext(announcer);

            if (migrationTags != null)
            {
                migrationContext.Tags = migrationTags;
            }

            var sqlProcessor = GetMigrationProcessor(connectionString, announcer);

            this.runner = new MigrationRunner(assembly, migrationContext, sqlProcessor);
        }

        protected virtual IMigrationProcessor GetMigrationProcessor(string connectionString, IAnnouncer announcer)
        {
            var sqlProcessor = new SqlServer2008ProcessorFactory().Create(connectionString, announcer, options);

            return sqlProcessor;
        }
    }
}
