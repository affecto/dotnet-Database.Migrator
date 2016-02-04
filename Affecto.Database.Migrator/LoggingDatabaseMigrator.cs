using System;
using System.IO;
using System.Reflection;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Announcers;

namespace Affecto.Database.Migrator
{
    public class LoggingDatabaseMigrator : DatabaseMigrator
    {
        private readonly string FilePath;

        public LoggingDatabaseMigrator(string connectionStringKey, Assembly assembly, MigrationOptions options) : base(connectionStringKey, assembly, options)
        {
            FilePath = string.Format(".\\sql_log_{0}.sql", DateTime.Now.ToString("yyyyMMdd-Hmmss"));
        }

        protected override IAnnouncer GetAnnouncer()
        {
            var logAnnouncer = new TextWriterAnnouncer(s =>
            {
                using (var writer = new StreamWriter(FilePath,true))
                {
                    writer.Write(s);

                    writer.Flush();
                } 
            })
            {
                ShowSql = true,
                ShowElapsedTime = true
            };

            var consoleAnnouncer = base.GetAnnouncer();

            return new CompositeAnnouncer(consoleAnnouncer, logAnnouncer);
        }
    }
}
