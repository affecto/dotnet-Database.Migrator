namespace Affecto.Database.Migrator
{
    public class MigrationAttribute : FluentMigrator.MigrationAttribute
    {
        private const int MajorVersionMultiplier = 1000;

        /// <summary>
        /// Creates migration with version major * 1000 + minor
        /// e.g. 2.23 -> 2023
        /// </summary>
        public MigrationAttribute(int majorVersion, int minorVersion)
            : base(majorVersion * MajorVersionMultiplier + minorVersion)
        {
        }
    }
}
