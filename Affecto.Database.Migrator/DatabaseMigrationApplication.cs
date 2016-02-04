using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Affecto.Database.Migrator.Interfaces;
using FluentMigrator.Runner.Initialization;

namespace Affecto.Database.Migrator
{
    public class DatabaseMigrationApplication : IDatabaseMigrationApplication
    {
        private readonly Assembly assembly;
        private const string DirectionUp = "up";
        private const string DirectionDown = "down";

        public DatabaseMigrationApplication(Assembly assembly)
        {
            this.assembly = assembly;
        }

        public void StartWithPrompt()
        {
            try
            {
                Execute();

                Console.ReadLine();

                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in database migration: " + e.Message);

                Console.ReadLine();

                Environment.Exit(1);
            }
        }

        public void Start(string connectionStringName)
        {
            try
            {
                List<string> migrationTags = GetMigrationTags();

                var runner = new DatabaseMigrator(connectionStringName, assembly, new MigrationOptions() { PreviewOnly = false, Timeout = 0 }, migrationTags);

                runner.MigrateUp();

                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error in database migration: {0}", e.Message);
                
                Environment.Exit(1);
            }
        }

        private void Execute()
        {
            var connectionStringName = GetConnectionStringName();
            var direction = GetDirection();
            long? targetVersion = GetTargetVersion(direction);

            List<string> migrationTags = GetMigrationTags();

            IDatabaseMigrator runner;

            if (direction.Equals(DirectionUp))
            {
                string runPopuleateScriptsTag = GetRunPopulateScripts();
                if (runPopuleateScriptsTag != null)
                {
                    migrationTags.Add(runPopuleateScriptsTag);
                }

                runner = GetRunnerProperties(connectionStringName, migrationTags);

                if (targetVersion.HasValue)
                {
                    runner.MigrateUp(targetVersion.Value);
                }
                else
                {
                    runner.MigrateUp();
                }
            }

            else if (direction.Equals(DirectionDown))
            {
                runner = GetRunnerProperties(connectionStringName, migrationTags);

                runner.MigrateDown(targetVersion.Value);
            }
        }

        private IDatabaseMigrator GetRunnerProperties(string connectionStringName, List<string> migrationTags = null)
        {
            string outputToFile = null;

            while (string.IsNullOrEmpty(outputToFile) || (!outputToFile.Equals("yes") && !outputToFile.Equals("no")))
            {
                Console.Write("Log output as .sql file (yes/no)? ");

                outputToFile = Console.ReadLine();
            }

            IDatabaseMigrator runner;

            if (outputToFile.Equals("yes"))
            {
                runner = new LoggingDatabaseMigrator(connectionStringName, assembly, new MigrationOptions() { PreviewOnly = false, Timeout = 0 });
            }
            else
            {
                runner = new DatabaseMigrator(connectionStringName, assembly, new MigrationOptions() { PreviewOnly = false, Timeout = 0 }, migrationTags);
            }

            return runner;
        }

        private string GetRunPopulateScripts()
        {
            bool? runPopulateScripts = null;
            
            while (runPopulateScripts == null)
            {
                Console.Write("Run data population scripts as well (yes/no)? ");

                var run = Console.ReadLine();

                if (run != null && run.Equals("yes"))
                {
                    return  MigrationTags.Populate;
                }

                if (run != null && run.Equals("no"))
                {
                    return null;
                }
            }

            return null;
        }

        private static long? GetTargetVersion(string direction)
        {
            if (direction.Equals(DirectionUp))
            {
                return  AskUserForVersion(true);
            }

            return AskUserForVersion(false);
        }

        private static long? AskUserForVersion(bool allowBlank)
        {
            long version = 0;
            bool valid = false;

            while (!valid)
            {
                Console.Write("Enter target version (hit enter for latest): ");

                string givenVersion = Console.ReadLine();

                if (givenVersion.Equals(string.Empty) && allowBlank)
                {
                    return null;
                }

                valid = long.TryParse(givenVersion, out version);
            }

            return version;
        }

        private static string GetDirection()
        {
            string direction = string.Empty;

            while (!(direction.Equals(DirectionUp) || direction.Equals(DirectionDown)))
            {
                Console.Write("Enter direction (up/down): ");

                direction = Console.ReadLine();
            }

            return direction;
        }

        private static string GetConnectionStringName()
        {
            Console.WriteLine("Available connection strings: ");

            var connectionStringNames = new List<string>();

            for (var i = 0; i < ConfigurationManager.ConnectionStrings.Count; i++)
            {
                var connectionStringName = ConfigurationManager.ConnectionStrings[i].Name;

                if (!connectionStringName.Equals("LocalSqlServer"))
                {
                    connectionStringNames.Add(connectionStringName);
                }
            }

            foreach (var name in connectionStringNames)
            {
                Console.WriteLine(name);
            }

            string givenConnectionString = null;

            while (!connectionStringNames.Contains(givenConnectionString))
            {
                Console.Write("Enter one of the available connection strings: ");

                givenConnectionString = Console.ReadLine();
            }

            return givenConnectionString;
        }

        private static List<string> GetMigrationTags()
        {
            List<string> migrationTags = null;
            string tags = ConfigurationManager.AppSettings["MigrationTags"];

            if (!string.IsNullOrEmpty(tags))
            {
                migrationTags = tags.Split(',').ToList();
            }
            return migrationTags;
        }
    }
}
