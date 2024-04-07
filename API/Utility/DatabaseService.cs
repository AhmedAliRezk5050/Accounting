namespace API.Utility;

using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

public class DatabaseService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly string _backupDirectory;
    
    public DatabaseService(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
        
        _backupDirectory = Path.Combine(_environment.WebRootPath, "backup");
    }

    public async Task<string> BackupDatabase()
    {
        Directory.CreateDirectory(_backupDirectory); // Ensure the backup directory exists

        var now = DateTimeOffset.UtcNow;
        var unixTime = now.ToUnixTimeSeconds();
        
        
        var backupFileName = $"{unixTime}";

        var commandText =
            $@"BACKUP DATABASE [Accounting] TO DISK = '{BackupFilePath(backupFileName)}' WITH FORMAT;";

        await ExecuteSqlCommand(commandText, _connectionString);

        return backupFileName;
    }

    public async Task RestoreDatabase(string backupFileName)
    {
        var masterConnectionString =
            new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = "master"
            }.ConnectionString;


        await SetDatabaseToSingleUserMode(masterConnectionString);
        var restoreCommand =
            $@"RESTORE DATABASE [Accounting] FROM DISK = '{BackupFilePath(backupFileName)}' WITH REPLACE;";
        await ExecuteSqlCommand(restoreCommand, masterConnectionString);
        await SetDatabaseToMultiUserMode(masterConnectionString);
    }

    private async Task SetDatabaseToSingleUserMode(string connectionString)
    {
        var commandText = @"ALTER DATABASE [Accounting] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
        await ExecuteSqlCommand(commandText, connectionString);
    }

    private async Task SetDatabaseToMultiUserMode(string connectionString)
    {
        const string commandText = @"ALTER DATABASE [Accounting] SET MULTI_USER;";
        await ExecuteSqlCommand(commandText, connectionString);
    }


    private async Task ExecuteSqlCommand(string commandText, string connectionString)
    {
        await using var connection = new SqlConnection(connectionString);

        await using var command = new SqlCommand(commandText, connection);

        await connection.OpenAsync();

        await command.ExecuteNonQueryAsync();
    }

    private string BackupFilePath(string fileName) =>
        Path.Combine(_environment.WebRootPath, "backup", $"{fileName}.bak");
}