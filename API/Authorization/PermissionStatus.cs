namespace API.Authorization;

public class PermissionStatus
{
    public string Value { get; set; } = null!;
    public bool IsUserHasClaim { get; set; }
}