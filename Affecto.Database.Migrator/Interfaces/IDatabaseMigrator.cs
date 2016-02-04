using System.Collections.Generic;

namespace Affecto.Database.Migrator.Interfaces
{
    public interface IDatabaseMigrator
    {
        void MigrateUp();
        void MigrateUp(long targetVersion);
        void MigrateDown(long targetVersion);
    }
}
