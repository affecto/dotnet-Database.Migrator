using System.Collections.Generic;

namespace Affecto.Database.Migrator.Interfaces
{
    public interface IDatabaseMigrationApplication
    {
        void StartWithPrompt();
        void Start(string connectionStringName);
    }
}
