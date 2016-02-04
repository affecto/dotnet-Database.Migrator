using FluentMigrator;

namespace Affecto.Database.Migrator
{
    public class MigrationOptions : IMigrationProcessorOptions
    {
        public bool PreviewOnly { get; set; }
        public int Timeout { get; set; }
        public string ProviderSwitches { get; set; }
    }
}
