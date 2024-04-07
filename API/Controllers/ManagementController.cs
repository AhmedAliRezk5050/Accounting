using API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class ManagementController : ApiController
{
    private readonly IWebHostEnvironment _environment;
    private readonly DatabaseService _databaseService;
    private readonly string _backupDirectory;
    
    public ManagementController(DatabaseService databaseService, IWebHostEnvironment environment)
    {
        _databaseService = databaseService;
        _environment = environment;
        _backupDirectory = Path.Combine(_environment.WebRootPath, "backup");
    }

    [HttpPost("Backup")]
    public async Task<IActionResult> Backup()
    {
        var backupFileName = await _databaseService.BackupDatabase();
        return Ok(backupFileName);
    }

    [HttpPost("Restore")]
    public async Task<IActionResult> Restore(RestoreRequest restoreRequest)
    {
        await _databaseService.RestoreDatabase(restoreRequest.BackupFileName);

        return Ok();
    }

    [HttpGet("GetBackupFileNames")]
    public IActionResult GetBackupFileNames()
    {
        var files = Directory.GetFiles(_backupDirectory);

        var fileNamesWithoutExtension = files.Select(Path.GetFileNameWithoutExtension);

        return Ok(fileNamesWithoutExtension);
    }
}